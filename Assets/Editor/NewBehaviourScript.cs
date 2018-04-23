using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript {

	[MenuItem("Tools/Set Bootstrap Materials")]
    private static void SetMaterial()
    {
        var objs = GameObject.FindGameObjectsWithTag("Slice");
        foreach (GameObject obj in objs)
        {
            string i = "000";
            if (obj.name.Length > 7)
            {
                i = int.Parse(obj.name.Substring(7, obj.name.Length - 8)).ToString("000");
            }

            string type = obj.transform.parent.name;
            
            var path = "Assets/Textures/BootstrapCurrent/" + type + "/Materials/" + type.ToLower() + "_" + i + ".mat";
            Debug.Log(path);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            Debug.Log(mat);
            obj.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
        
    }
}
