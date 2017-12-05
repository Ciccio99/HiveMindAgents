using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour {

    public Vector3[] BezierVertices { get { return _bezierVertices; } private set { _bezierVertices = value; } }

    // Visible in inspector
	public Vector3[] points;
	public int lineSteps = 15;

	private LineRenderer _lineRenderer;
    private Vector3[] _bezierVertices;

	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer> () ?? gameObject.AddComponent<LineRenderer> ();
	}

	void Start()
	{
		SetLineRendererCurve ();
	}

	public void Reset () {
		points = new Vector3[] {
			new Vector3 (1f, 0f, 0f),
			new Vector3 (2f, 0f, 0f),
			new Vector3 (3f, 0f, 0f)
		};
		_lineRenderer = GetComponent<LineRenderer> () ?? gameObject.AddComponent<LineRenderer> ();
	}

	public Vector3 GetPoint (float t) {
		return transform.TransformPoint (Bezier.GetPoint (points[0], points[1], points[2], t));
	}

	public void SetLineRendererCurve () {
        _lineRenderer = GetComponent<LineRenderer> () ?? gameObject.AddComponent<LineRenderer> ();
		_bezierVertices = new Vector3[lineSteps + 1];
		_lineRenderer.positionCount = _bezierVertices.Length;

		_bezierVertices[0] = GetPoint (0f);
		for (int i = 1; i<= lineSteps; i++) {
			Vector3 lineEnd = GetPoint (i / (float) lineSteps);
			_bezierVertices[i] = lineEnd;
		}

		_lineRenderer.SetPositions (_bezierVertices);
	}

	public void SetMainPoints (Vector3 start, Vector3 mid, Vector3 end) {
		points[0] = start;
		points[1] = mid;
		points[2] = end;
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var p in points) {
            Gizmos.DrawSphere (p + transform.position, 0.2f);
        }
    }
}
