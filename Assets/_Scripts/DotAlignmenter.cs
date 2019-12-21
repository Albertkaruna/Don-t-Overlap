//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class DotAlignmenter : MonoBehaviour {

//    public int hDistance=75;
//    public int vDistance=54;
//    public int hCount;
//    public int vCount;

//    [MenuItem("Karuna/Renamer %#R")]
//    public static void AddComponent()
//    {
//        try
//        {
//            Selection.activeTransform.gameObject.AddComponent(typeof(DotAlignmenter));
//        }
//        catch (System.Exception)
//        {
//            Debug.Log("Select the parent to align childs.");
//        }
//    }


//    public void AlignDots()
//    {
//        Debug.Log("Aligned");
//        GameObject dot = Selection.activeTransform.GetChild(0).gameObject;
//        float x = -(hDistance / hCount);
//        float y = vDistance / vCount;
//        float name = 1;

//        for (int i = 0; i < vCount; i++)
//        {

//            for (int j = 0; j < hCount; j++)
//            {
//                Vector3 dotPos = new Vector3(x, y, 0);
//                GameObject go = Instantiate(dot, dotPos, Quaternion.identity) as GameObject;
//                go.transform.SetParent(dot.transform.parent);
//                go.name = name.ToString();

//                name++;
//                x += hDistance / hCount;
//            }
//            x = -(hDistance / hCount);
//            y -= (vDistance / hCount);
//        }

//        DestroyImmediate(dot);
//    }
//}
