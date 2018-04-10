using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SerializeData : MonoBehaviour
{

    public List<List<Particle>> data;
    public int particleCount;
    public int timestepCount;
    public string file;
    public bool runOnce = false;
    public bool isReady = false;

    Task read;
    Task scale;
    Task convert;
    Task classify;
    string fullPath;
    bool printInfo = true;
    float scaleFactor;

    void Start()
    {
        scaleFactor = transform.parent.localScale.x;
    }

    void FixedUpdate()
    {

        if (runOnce)
        {
            runOnce = false;
            Read();
        }
        /*
        if (isReady && serialize)
        {
            string fullpath = Application.dataPath + "/ParticleData/" + filename + ".dat";
            DateTime start = DateTime.Now;
            serialize = false;
            Task serializeTask = Task.Run(() => Serializer.Serialize(data, fullpath));
            await serializeTask;
            Debug.Log("Serialize Complete in " + (DateTime.Now - start).ToString());
        }
        */
    }

    public async void Read()
    {
        fullPath = Application.dataPath + "/StreamingAssets/" + file;

        data = new List<List<Particle>>();
        foreach(ParticleDisplayMode pdm in Enum.GetValues(typeof(ParticleDisplayMode)))
        {
            data.Add(new List<Particle>());
        }
        //Read Data from File
        read = Task.Run(() => FromFile(fullPath));
        await read;

        //Scale/Transform data to fit model
        scale = Task.Run(() => Transform(0.8f, 0.99f, 0.78f, 2.42f));
        await scale;

        classify = Task.Run(() => Classify());
        await classify;

        //Convert polar->rectangular coordinates
        convert = Task.Run(() => ToRectangular(true));
        await convert;

        particleCount = data[0].Count;
        timestepCount = data[0][0].timesteps.Count;

        if (printInfo)
        {
            Debug.Log("Completed Read");
            Debug.Log("There are " + particleCount + " particles, with  " + timestepCount + " timesteps each.");
        }
        isReady = true;
    }

    public void FromFile(string path, int downsampleParticles = 1, int downsampleTimesteps = 1)
    {
        Debug.Log(path);
        StreamReader sr = new StreamReader(path);
        char[] delimiter = new char[] { ' ' };

        DateTime start = DateTime.Now;
        int pID = 0;

        while (sr.Peek() > 0)
        {
            int tStep = 0;
            string[] line = sr.ReadLine().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            //Line contains particleID
            if (line.Length == 1)
            {
                pID = int.Parse(line[0]);
                if (pID % downsampleParticles == 0)
                {
                    //Debug.Log("Adding Particle " + pID + " from line " + line[0]);
                    data[0].Add(new Particle());
                    tStep = 0;
                }
            }
            //Data saved in "\t r \t z \t phi"
            else if (line.Length == 3 && pID % downsampleParticles == 0)
            {
                if (tStep == 0)
                {
                    //Debug.Log("Adding Timestep to particle " + pID + ", iteration " + count);
                    data[0][pID / downsampleParticles].timesteps.Add(new Vector3(
                        float.Parse(line[0]),
                        float.Parse(line[1]),
                        float.Parse(line[2])
                        ));
                }
                tStep = (tStep + 1) % downsampleTimesteps;
            }
        }

        if (printInfo) Debug.Log("ReadParticleData.FromFile: " + (DateTime.Now - start).ToString());
    }

    public void Transform(float scaleR, float transR, float scaleZ, float transZ)
    {
        DateTime start = DateTime.Now;
        foreach (Particle particle in data[0])
        {
            for (int i = 0; i < particle.timesteps.Count; i++)
            {
                Vector3 timestep = particle.timesteps[i];
                timestep.x = timestep.x * scaleR + transR;
                timestep.y = timestep.y * scaleZ + transZ;
                particle.timesteps[i] = timestep;
            }
        }
        if (printInfo) Debug.Log("ReadParticleData.Transform: " + (DateTime.Now - start).ToString());
    }

    public void Classify()
    {
        DateTime start = DateTime.Now;
        foreach (Particle p in data[0])
        {
            List<int> inflectionPtIndex = new List<int>();
            List<bool> inflectionPtValue = new List<bool>();
            float prevDelta = 0f;
            float prevTheta = 0f;
            float distance = 0f;
            for (int i = 0; i < p.timesteps.Count; i++)
            {
                float theta = p.timesteps[i].z;
                float delta = theta - prevTheta;
                if (i > 1)
                {
                    //if different signs OR if this is last iteration
                    if ((delta < 0 && prevDelta > 0) || (delta > 0 && prevDelta < 0) || (i == p.timesteps.Count-1))
                    {
                        //if particle has moved enough since last inflection point, it is passing: (false), otherwise it is trapped: (true)
                        inflectionPtValue.Add(distance < 3.14f);
                        inflectionPtIndex.Add(i);
                        distance = 0;
                    }

                    distance += Mathf.Abs(delta);
                }


                prevDelta = delta;
                prevTheta = theta;
            }

            int inflectEnd = 0;
            int j = 0;
            bool inflectVal = true;
            
            for (int i = 0; i < p.timesteps.Count; i++)
            {
                if (inflectEnd == i && i != p.timesteps.Count-1)
                {
                    inflectEnd = inflectionPtIndex[j];
                    inflectVal = inflectionPtValue[j];
                    j++;

                }
                p.trapped.Add(inflectVal);
            }
        }

        if (printInfo) Debug.Log("ReadParticleData.Classify: " + (DateTime.Now - start).ToString());
    }

    public void ToRectangular(bool flat = true)
    {
        DateTime start = DateTime.Now;
        
        if (flat)
        {
            foreach( Particle particle in data[0])
            {
                data[1].Add(new Particle(particle.trapped));
                foreach(Vector3 v in particle.timesteps)
                {
                    data[1][data[1].Count - 1].timesteps.Add(new Vector3(v.x, v.y, 0.0f));
                }
            }
        }
        
        foreach (Particle particle in data[0])
        {
            for (int i = 0; i < particle.timesteps.Count; i++)
            {
                //timeStep.x: r * cos(phi)
                //timeStep.y: z
                //timeStep.z: r * sin(phi) 

                Vector3 timestep = particle.timesteps[i];
                float x = timestep.x * Mathf.Cos(timestep.z);
                float y = timestep.y;
                float z = timestep.x * Mathf.Sin(timestep.z);
                particle.timesteps[i] = new Vector3(x, y, z);
            }
        }
        if (printInfo) Debug.Log("ReadParticleData.ToRectangular: " + (DateTime.Now - start).ToString());
    }
    
}

public class Particle
{
    public List<Vector3> timesteps;
    public List<bool> trapped;

    public Particle()
    {
        timesteps = new List<Vector3>();
        trapped = new List<bool>();
    }

    public Particle(List<bool> b)
    {
        timesteps = new List<Vector3>();
        trapped = b;
    }
}