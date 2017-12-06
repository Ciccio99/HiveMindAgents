using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor {

	private BezierCurve curve;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private int lineSteps = 30;

	private void OnSceneGUI () {
		curve = target as BezierCurve;
		lineSteps = curve.lineSteps;
		handleTransform = curve.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ? 
			handleTransform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint (0);
		Vector3 p1 = ShowPoint (1);
		Vector2 p2 = ShowPoint (2);

		Handles.color = Color.gray;
		Handles.DrawLine (p0, p1);
		Handles.DrawLine (p1, p2);

		Handles.color = Color.white;
		Vector3 lineStart = curve.GetPoint (0f);
	
		for (int i = 1; i<= lineSteps; i++) {
			Vector3 lineEnd = curve.GetPoint (i / (float) lineSteps);
			Handles.DrawLine (lineStart, lineEnd);
			lineStart = lineEnd;
		}
		curve.SetLineRendererCurve ();
	}

    /// <summary>
    /// Shows the point and sets its position based on the handler position.
    /// </summary>
    /// <returns>The point.</returns>
    /// <param name="index">Index.</param>
	private Vector3 ShowPoint (int index) {
		Vector3 point = handleTransform.TransformPoint(curve.points[index]);
		EditorGUI.BeginChangeCheck ();
		point = Handles.DoPositionHandle (point, handleRotation);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (curve, "Move Point");
			EditorUtility.SetDirty (curve);
			curve.points[index] = handleTransform.InverseTransformPoint (point);
		}
		return point;
	}
}
