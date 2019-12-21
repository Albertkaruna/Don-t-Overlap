//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(DotAlignmenter))]
//public class DotAlignmentEditor : Editor {

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        DotAlignmenter alignmenter = (DotAlignmenter)target;

//        GUILayout.BeginHorizontal();
//        if(GUILayout.Button("Align"))
//        {
//            alignmenter.AlignDots();
//        }
//        if(GUILayout.Button("Exit"))
//        {
//            DestroyImmediate(Selection.activeTransform.GetComponent<DotAlignmenter>());
//        }
//        GUILayout.EndHorizontal();
//    }

//}
