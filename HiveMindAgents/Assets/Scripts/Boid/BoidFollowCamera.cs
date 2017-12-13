using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFollowCamera : MonoBehaviour {

    private GameObject _followObject;
    private float _damping = 1f;
    private Vector3 _offsetDistance;


	// Use this for initialization
	void Start () {
        if (_followObject != null)
            _offsetDistance = transform.position - _followObject.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (_followObject == null) return;

        _offsetDistance = _followObject.transform.forward.normalized * -1f;

        Vector3 desiredPosition = _followObject.transform.position + _offsetDistance;
        Vector3 position = Vector3.Lerp (transform.position, desiredPosition, Time.deltaTime * _damping);
        transform.position = position;

        transform.LookAt (_followObject.transform.position);
	}

    public void SetFollowTargetObject (GameObject target) {
        _followObject = target;
    }
}
