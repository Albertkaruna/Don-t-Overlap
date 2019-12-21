using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Renamer : MonoBehaviour
{
	string nm = "cpu";
	// Use this for initialization
	void Start ()
	{
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) != null)
                transform.GetChild(i).name = "Level_"+(i + 1).ToString();
        }
        //transform.name = "e15";
    }

    // Update is called once per frame
    void Update ()
	{
		
	}
}
