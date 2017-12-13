using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
//[RequireComponent (typeof (LineRenderer))]
public class BoidPather : MonoBehaviour {

    public bool looping;
    public Vector3[] pathVertices;
   
    /// <summary>
    /// Reset this instance.
    /// </summary>
    public void Reset()
    {
        pathVertices = new Vector3[] {
            new Vector3 (1f, 0f, 0f),
            new Vector3 (2f, 0f, 0f),
            new Vector3 (3f, 0f, 0f)
        };
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pathVertices.Length - 1; i++) {
            Gizmos.DrawLine (pathVertices[i], pathVertices[i + 1]);
        }
        if (looping)
            Gizmos.DrawLine (pathVertices[0], pathVertices[pathVertices.Length - 1]);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere (pathVertices[0], 1f);

    }
}
