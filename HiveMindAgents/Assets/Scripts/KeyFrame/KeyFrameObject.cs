using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyFrameObject : MonoBehaviour {
    [SerializeField]
    private bool loop = false;
	[SerializeField]
	private List<KeyFrame> _inputKeyFrames;
    private List<Keyframe> _processKeyFrames;
	private LinkedList<KeyFrame> _keyFrames;
	private float _previousKeyFrameTime;
	private Vector3 _previousPosition;
	private Quaternion _previousRotation;
    private int _currentKeyFrameIndex;

	void Awake () {
		_keyFrames = new LinkedList<KeyFrame> ();
		if (_inputKeyFrames.Count > 0) {
			foreach (var keyFrame in _inputKeyFrames) {
				_keyFrames.AddLast (keyFrame);
			}


            //var first = _keyFrames.First.Value;
            //if (first.time == 0f) {
            //	transform.position = first.position;
            //	transform.rotation = Quaternion.AngleAxis (first.angle, first.axis);
            //	_previousPosition = transform.position;
            //	_previousRotation = transform.rotation;
            //	_previousKeyFrameTime = first.time;

            //	_keyFrames.RemoveFirst ();
            //}
            _currentKeyFrameIndex = 0;
			StartCoroutine (KeyFrameLoop ());
		}
	}
	
	private IEnumerator KeyFrameLoop () {
		if (_currentKeyFrameIndex >= _inputKeyFrames.Count) {
            if (loop) {
                _currentKeyFrameIndex = 0;
                StartCoroutine (KeyFrameLoop ());
            }
            
    		yield break;
		}

		var keyFrame = _inputKeyFrames[_currentKeyFrameIndex];
		float deltaTime = keyFrame.time - _previousKeyFrameTime;
		float timePassed = 0f;

		while (timePassed < deltaTime) {
            float ratio;
            if (deltaTime == 0f)
                ratio = 0f;
            else
                ratio = timePassed / deltaTime;
            
			InterpPosition (ratio, keyFrame);
			InterpRotation (ratio, keyFrame);
			timePassed += Time.deltaTime;
			yield return null;
		}
		_previousKeyFrameTime = keyFrame.time;
		_previousPosition = transform.position;
		_previousRotation = transform.rotation;
        //_keyFrames.RemoveFirst ();
        _currentKeyFrameIndex++;
		StartCoroutine (KeyFrameLoop());
	}

	private void InterpPosition (float ratio, KeyFrame keyFrame) {
		if (_previousPosition == null) {
			throw new Exception ("Previous transform to interpolate from is Null");
		}

		transform.position = Vector3.Lerp (_previousPosition, keyFrame.position, ratio);
	}

	private void InterpRotation (float ratio, KeyFrame keyFrame) {
		if (_previousRotation == null) {
			throw new Exception ("Previous transform to interpolate from is Null");
		}
		Quaternion finalRotation = Quaternion.AngleAxis (keyFrame.angle, -keyFrame.axis.normalized);
		transform.rotation = Quaternion.Slerp (_previousRotation, finalRotation, ratio);

	}
}
