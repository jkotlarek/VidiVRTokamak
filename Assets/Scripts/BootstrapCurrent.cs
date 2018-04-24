using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootstrapCurrent : MonoBehaviour {

    PopulateParticles particleScript;
    SerializeData particleData;

    GameObject trapped;
    GameObject passing;

    List<GameObject> tSlice;
    List<GameObject> pSlice;

    int numSlices;
    int particleTimestep = 0;
    int pastTimestep = 0;
    int currentTimestep = 0;
    
	// Use this for initialization
	void Start () {
        particleScript = FindObjectOfType<PopulateParticles>();
        particleData = FindObjectOfType<SerializeData>();

        trapped = transform.Find("Trapped").gameObject;
        passing = transform.Find("Passing").gameObject;

        tSlice = GetAndDisableChildren(trapped.transform);
        pSlice = GetAndDisableChildren(passing.transform);
        
        numSlices = Mathf.Min(tSlice.Count, pSlice.Count);
    }
	
	// Update is called once per frame
	void Update () {
        if (particleData.isReady && !particleScript.paused)
        {
            if (particleTimestep == 0) particleTimestep = particleData.timestepCount;
            pastTimestep = currentTimestep;
            currentTimestep = particleScript.currentTimestep * numSlices / particleTimestep;

            if (trapped.activeInHierarchy && pastTimestep != currentTimestep)
            {
                tSlice[pastTimestep].SetActive(false);
                tSlice[currentTimestep].SetActive(true);
            }
            if (passing.activeInHierarchy && pastTimestep != currentTimestep)
            {
                pSlice[pastTimestep].SetActive(false);
                pSlice[currentTimestep].SetActive(true);
            }
        }
    }

    public void SetActiveTrapped(bool isActive)
    {
        trapped.SetActive(isActive);
        //tSlice.ForEach((GameObject g) => g.SetActive(false));
    }

    public void SetActivePassing(bool isActive)
    {
        passing.SetActive(isActive);
        //pSlice.ForEach((GameObject g) => g.SetActive(false));
    }

    List<GameObject> GetAndDisableChildren(Transform t)
    {
        List<GameObject> list = new List<GameObject>();
        
        for (int i = 0; i < t.childCount; i++)
        {
            GameObject g = t.GetChild(i).gameObject;
            list.Add(g);
            g.SetActive(false);
        }

        return list;
    }
}
