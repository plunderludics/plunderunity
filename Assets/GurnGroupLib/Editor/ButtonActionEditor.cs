using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Events;
 
[CustomEditor(typeof(ButtonAction))]
public class ButtonActionEditor : Editor {
    override public void  OnInspectorGUI () {
        DrawDefaultInspector();
        if (GUILayout.Button("Do")) {
            ((ButtonAction)target).Do();
        }
    }
}