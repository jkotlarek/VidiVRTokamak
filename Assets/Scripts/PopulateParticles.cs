using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ParticleDisplayMode
{
    Normal, Flat
}

public class PopulateParticles : MonoBehaviour {

    public GameObject particlePrefab;
    public GameObject highlightedParticlePrefab;
    public GameObject origin;
    public int framesPerTimestep = 5;
    public int maxFramesPerTimestep = 20;
    public Material material;
    public Color normalColor;
    public Color shiftedColor;
    public Color outlineColor;
    public Color trappedColor;
    public Color passingColor;
    //public Gradient trailGradient;
    public float maxSpeed = 0.075f;
    public List<GameObject> highlightedParticles;
    public bool paused = false;
    public bool trails = true;
    public bool serialize = false;
    public bool speedshift = true;
    public bool flushTrails = false;
    public bool classify = false;
    public List<int> particleMask;
    public int taskNum;
    public string username;
    public ParticleDisplayMode currentMode;
    public List<Vector3> positions;

    string filename = "sd";
    float flatSpeedFactor = 5f;

    SerializeData DataReader;
    List<GameObject> particles;
    Color noOutline;
    TrailRenderer defaultTrail;
    TrailRenderer highlightTrail;
    Vector3 startPosition;
    DateTime startTime;

    public int currentTimestep = 0;
    int frameCount = 0;
    

    // Use this for initialization
    void Start () {
        DataReader = GetComponent<SerializeData>();
        noOutline = new Color(0, 0, 0, 0);
        defaultTrail = particlePrefab.GetComponent<TrailRenderer>();
        highlightTrail = highlightedParticlePrefab.GetComponent<TrailRenderer>();
        startPosition = origin.transform.position;
        positions = new List<Vector3>();
    }


    void OnApplicationQuit()
    {
        positions.Add(origin.transform.position);

        DateTime now = DateTime.Now;
        string file = Application.dataPath + "/Tests/" + now.ToString("s") + ".json";
        Debug.Log("Outputting Run Statistics to " + file);

        var rs = new RunStatistics();
        rs.Date = now;
        rs.UserName = username;
        rs.TaskNum = taskNum;
        rs.Environment = SceneManager.GetActiveScene().name.Split('_')[1];
        rs.TaskTime = now - startTime;
        rs.Particles = SerializableParticle.fromGOList(particles);
        rs.UserPath = SerializableVector3.fromList(positions);

        string json = JsonUtility.ToJson(rs, true);
    }

    // Update is called once per frame
    void Update () {

        if (DataReader.isReady && !paused)
        {
            if (particles == null)
            {
                particles = new List<GameObject>();

                if (particleMask.Count == 0)
                {
                    foreach (Particle ts in DataReader.data[(int)currentMode])
                    {
                        particles.Add(Instantiate(particlePrefab, ts.timesteps[currentTimestep], Quaternion.identity, transform));
                        particles[particles.Count - 1].GetComponent<Renderer>().material = new Material(material);
                        particles[particles.Count - 1].GetComponent<TrailRenderer>().Clear();
                        particles[particles.Count - 1].GetComponent<TrailRenderer>().enabled = trails;
                    }
                }
                else
                {
                    foreach (int i in particleMask)
                    {
                        particles.Add(Instantiate(particlePrefab, DataReader.data[(int)currentMode][i].timesteps[currentTimestep], Quaternion.identity, transform));
                        particles[particles.Count - 1].GetComponent<Renderer>().material = new Material(material);
                        particles[particles.Count - 1].GetComponent<TrailRenderer>().Clear();
                        particles[particles.Count - 1].GetComponent<TrailRenderer>().enabled = trails;
                    }
                }
            }
            else
            {
                if (frameCount % framesPerTimestep == 0)
                {
                    currentTimestep = (currentTimestep + 1) % DataReader.timestepCount;
                    frameCount = 0;
                }

                float t = (frameCount % framesPerTimestep) / (float)framesPerTimestep;
                if (particleMask.Count == 0)
                {
                    for (int i = 0; i < DataReader.particleCount; i++)
                    {
                        particles[i].transform.localPosition = Vector3.Lerp(
                            DataReader.data[(int)currentMode][i].timesteps[currentTimestep],
                            DataReader.data[(int)currentMode][i].timesteps[(currentTimestep + 1) % DataReader.timestepCount],
                            t);

                        if (speedshift)
                        {
                            particles[i].GetComponent<Renderer>().material.SetColor("_Color", SpeedHueShift(
                                    DataReader.data[(int)currentMode][i].timesteps[currentTimestep],
                                    DataReader.data[(int)currentMode][i].timesteps[(currentTimestep + 1) % DataReader.timestepCount]));
                        }
                        else
                        {
                            particles[i].GetComponent<Renderer>().material.SetColor("_Color", ColorClassifier(DataReader.data[(int)currentMode][i].trapped[currentTimestep]));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < particleMask.Count; i++)
                    {
                        int j = particleMask[i];
                        particles[i].transform.localPosition = Vector3.Lerp(
                            DataReader.data[(int)currentMode][j].timesteps[currentTimestep],
                            DataReader.data[(int)currentMode][j].timesteps[(currentTimestep + 1) % DataReader.timestepCount],
                            t);

                        if (speedshift)
                        {
                            particles[i].GetComponent<Renderer>().material.SetColor("_Color", SpeedHueShift(
                                    DataReader.data[(int)currentMode][j].timesteps[currentTimestep],
                                    DataReader.data[(int)currentMode][j].timesteps[(currentTimestep + 1) % DataReader.timestepCount]));
                        }
                        else if (classify)
                        {
                            particles[i].GetComponent<Renderer>().material.SetColor("_Color", ColorClassifier(DataReader.data[(int)currentMode][j].trapped[currentTimestep]));
                        }
//                      else if (identify) {
//                          do nothing;
//                      }
                    }
                }
                frameCount++;
            }
            if (flushTrails || currentTimestep == 2999 || currentTimestep == 0)
            {
                FlushTrails();
                flushTrails = false;
            }
        }
	}

    /// <summary>
    /// Interpolates a color based on distance travelled between timesteps
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    private Color SpeedHueShift(Vector3 v1, Vector3 v2)
    {
        float t = Mathf.Sqrt((v2- v1).sqrMagnitude) / maxSpeed;
        if (currentMode == ParticleDisplayMode.Flat) t *= flatSpeedFactor;
        return Color.Lerp(normalColor, shiftedColor, t);
    }
    
    /// <summary>
    /// Returns the color for a particle based on whether it is passing or trapped
    /// </summary>
    /// <param name="trapped"></param>
    /// <returns></returns>
    private Color ColorClassifier(bool trapped)
    {
        return trapped ? trappedColor : passingColor;
    }

    /// <summary>
    /// Toggles Highlighting on a particle object
    /// </summary>
    /// <param name="particle"></param>
    public void HighlightParticle(GameObject particle)
    {
        if (highlightedParticles.Contains(particle))
        {
            highlightedParticles.Remove(particle);
            particle.GetComponent<Renderer>().material.SetColor("_OutlineColor", noOutline);
            TrailRenderer tr = particle.GetComponent<TrailRenderer>();
            tr.Clear();
            tr.widthMultiplier = defaultTrail.widthMultiplier;
            tr.time = defaultTrail.time;
            tr.startColor = defaultTrail.startColor;
            tr.endColor = defaultTrail.endColor;
            tr.enabled = trails;
        }
        else
        {
            highlightedParticles.Add(particle);
            particle.GetComponent<Renderer>().material.SetColor("_OutlineColor", outlineColor);
            TrailRenderer tr = particle.GetComponent<TrailRenderer>();
            tr.Clear();
            tr.widthMultiplier = highlightTrail.widthMultiplier;
            tr.time = highlightTrail.time;
            tr.startColor = highlightTrail.startColor;
            tr.endColor = highlightTrail.endColor;
            tr.enabled = true;
        }
    }

    public void ClearHighlight()
    {
        foreach(GameObject particle in highlightedParticles)
        {
            particle.GetComponent<Renderer>().material.SetColor("_OutlineColor", noOutline);
            TrailRenderer tr = particle.GetComponent<TrailRenderer>();
            tr.Clear();
            tr.widthMultiplier = defaultTrail.widthMultiplier;
            tr.time = defaultTrail.time;
            tr.startColor = defaultTrail.startColor;
            tr.endColor = defaultTrail.endColor;
            tr.enabled = trails;
        }
        highlightedParticles.Clear();
    }

    public void ToggleTrails()
    {
        trails = !trails;
        foreach (GameObject p in particles)
        {
            p.GetComponent<TrailRenderer>().enabled = trails;
        }
        foreach (GameObject p in highlightedParticles)
        {
            var tr = p.GetComponent<TrailRenderer>();
            tr.enabled = true;
            if (trails) tr.Clear();
        }

    }

    public void ToggleSpeedshift()
    {
        speedshift = !speedshift;
        foreach (GameObject p in particles)
        {
            var tr = p.GetComponent<TrailRenderer>();
            tr.Clear();
            //tr.colorGradient = speedshift ? defaultTrail.colorGradient : trailGradient;
        }
    }

    public void FramesPerTimestep(int delta)
    {
        framesPerTimestep = Mathf.Clamp(framesPerTimestep+delta, 1, maxFramesPerTimestep);
    }

    public void FlushTrails()
    {
        Debug.Log("flush trails");
        foreach (GameObject p in particles)
        {

            p.GetComponent<TrailRenderer>().Clear();
        }
    }
    
}

[Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public static List<SerializableVector3> fromList(List<Vector3> list)
    {
        var newList = new List<SerializableVector3>();
        foreach (var v in list) newList.Add(new SerializableVector3(v));
        return newList;
    }

    public Vector3 toVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class SerializableParticle
{
    public bool Touched;
    public SerializableVector3 Position;

    public SerializableParticle(bool t, SerializableVector3 p)
    {
        Touched = t;
        Position = p;
    }

    public SerializableParticle(bool t, Vector3 p)
    {
        Touched = t;
        Position = new SerializableVector3(p);
    }

    public static List<SerializableParticle> fromGOList(List<GameObject> list)
    {
        var newList = new List<SerializableParticle>();
        foreach(var go in list)
        {
            newList.Add(new SerializableParticle(
                go.GetComponent<ParticleData>().touched, 
                go.transform.position));
        }
        return newList;
    }

}

[Serializable]
public class RunStatistics
{
    public int TaskNum;
    public string Environment;
    public string UserName;
    public DateTime Date;
    public TimeSpan TaskTime;
    public List<SerializableParticle> Particles;
    public List<SerializableVector3> UserPath;
}