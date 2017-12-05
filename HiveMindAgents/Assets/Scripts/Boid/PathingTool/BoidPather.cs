using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
//[RequireComponent (typeof (LineRenderer))]
public class BoidPather : MonoBehaviour {

    //public Vector3[] PathVertices { get { return _pathVertices; } private set { _pathVertices = value; }}

    public bool looping;
    public Vector3[] pathVertices;
    //private LineRenderer _lineRenderer;

    private void Awake() {
        //_lineRenderer = GetComponent<LineRenderer> () ?? gameObject.AddComponent<LineRenderer> ();
    }

    private void Start() {
        //SetLineRendererPoints ();
    }

    public void Reset()
    {
        pathVertices = new Vector3[] {
            new Vector3 (1f, 0f, 0f),
            new Vector3 (2f, 0f, 0f),
            new Vector3 (3f, 0f, 0f)
        };
        //_lineRenderer = GetComponent<LineRenderer> () ?? gameObject.AddComponent<LineRenderer> ();
    }

    //public void SetLineRendererPoints () {
    //    if (_lineRenderer == null)
    //        throw new NullReferenceException ("BoidPather: LineRenderer reference is missing...");

    //    _lineRenderer.positionCount = _pathVertices.Length;
    //    _lineRenderer.SetPositions (_pathVertices);
    //}

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
