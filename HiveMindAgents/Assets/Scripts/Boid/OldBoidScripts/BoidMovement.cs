using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour {
	[SerializeField]
	private float _cohesionRadius;

	[SerializeField]
	private float _separationDistance;
	[SerializeField]
	private float _cohesionForce;
	[SerializeField]
	private float _separationForce;
	[SerializeField]
	private float _wayPointForce;
	[SerializeField]
	private float _velocityMax;

	private Transform _wayPointTransform;
	private Rigidbody _rigidbody;
	

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody> ();
		_wayPointTransform = GameObject.FindGameObjectWithTag ("Waypoint").transform;
	}

	
	// Update is called once per frame
	void FixedUpdate()
	{	

		Collider[] hitColliders = Physics.OverlapSphere (transform.position, _cohesionRadius);

		var boids = new List<GameObject> ();
		foreach (Collider col in hitColliders) {
			boids.Add (col.gameObject);
		}

		Vector3 finalForceVector = Vector3.zero;

		finalForceVector += GetCohesionVector (boids) * _cohesionForce;
		finalForceVector += GetSeparationVector (boids) * _separationForce;
		finalForceVector += (_wayPointTransform.position - transform.position) * _wayPointForce;

		_rigidbody.AddForce (finalForceVector);
		transform.rotation = Quaternion.LookRotation (_rigidbody.velocity.normalized);

		if (Vector3.Magnitude (_rigidbody.velocity) > _velocityMax) {
			_rigidbody.velocity = _velocityMax * _rigidbody.velocity.normalized;
		}
	}

	private Vector3 GetCohesionVector (List<GameObject> boids) {
		Vector3 cohesivePos = new Vector3 ();

		foreach (var boid in boids) {
			cohesivePos += boid.transform.position;
		}

		cohesivePos /= boids.Count;

		var resultVector = cohesivePos - transform.position;

		return resultVector;
	}

	private Vector3 GetSeparationVector (List<GameObject> boids) {
		Vector3 repulsiveVector = Vector3.zero;

		foreach (var boid in boids) {
			float distance = Vector3.Magnitude (boid.transform.position - transform.position);
			if (distance < _separationDistance) {
				repulsiveVector += transform.position - boid.transform.position;
			}
		}

		return repulsiveVector;
	}


}
