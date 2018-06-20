using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayAreaResize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        OnEnable();
	}

    private void OnEnable()
    {
        var corners = VRTK_SDK_Bridge.GetPlayAreaVertices();
        Vector3 newScale = new Vector3(
            Mathf.Abs(corners[0].x - corners[2].x), 
            this.transform.localScale.y, 
            Mathf.Abs(corners[0].z - corners[2].z));
        
        //return without doing anything if failed
        if (newScale.x == 0 || newScale.y == 0)
        {
            Debug.LogError("Failed to get play area dimensions!");
            return;
        }

        //set scale of object to match play area
        this.transform.localScale = newScale/10f;
    }
}
