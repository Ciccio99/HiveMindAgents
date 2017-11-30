using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyFrame {
	public float time;
	public Vector3 position;
	public Vector3 axis;
	public float angle;

	public KeyFrame (float time, Vector3 position, Vector3 axis, float angle) {
		this.time = time;
		this.position = position;
		this.axis = axis;
		this.angle = angle;
	}
}
