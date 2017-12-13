using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (BoidPather))]
public class BoidPatherInspector : Editor {

    private BoidPather _pather;
    private Transform _handleTransform;
    private Quaternion _handleRotation;

    private void OnSceneGUI() {
        _pather = target as BoidPather;

        _handleTransform = _pather.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                               _handleTransform.rotation : Quaternion.identity;

        Vector3[] points = ShowPoints ();

        Handles.color = Color.cyan;
        for (int i = 0; i < points.Length - 1; i++) {
            Handles.DrawLine (points[i], points[i + 1]);
        }
    }



    /// <summary>
    /// Shows the points.
    /// </summary>
    /// <returns>The points.</returns>
    private Vector3[] ShowPoints () {
        for (int i = 0; i < _pather.pathVertices.Length; i++) {
            //var p = _handleTransform.TransformPoint (_pather.pathVertices[i]);
            var p = _pather.pathVertices[i];
            // Called so that Unity "Remembers" the last actions so you can undo
            EditorGUI.BeginChangeCheck ();

            p = Handles.DoPositionHandle (p, _handleRotation);
            if (EditorGUI.EndChangeCheck ()) {
                Undo.RecordObject (_pather, "Move Points");
                EditorUtility.SetDirty (_pather);
                _pather.pathVertices[i] = p;
            }
        }

        return _pather.pathVertices;
    } 

}
