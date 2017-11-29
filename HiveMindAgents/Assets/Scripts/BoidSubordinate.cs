using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSubordinate : MonoBehaviour {

    private BoidManager _boidManager;
    private Rigidbody _rigidbody;

    private void OnEnable () {
        _rigidbody = GetComponent<Rigidbody> ();
    }

    // Update is called once per frame
    void FixedUpdate () {
        // Get nieghbors
        var neighborColliders = Physics.OverlapSphere (transform.position, _boidManager.NeighborRange);
        var neighborList = new List<GameObject> ();
        foreach (var col in neighborColliders) {
            var go = col.gameObject;
            var subordinate = go.GetComponent<BoidSubordinate> ();
            // TO DO: Add in list of non boid neighbors that will add to the respulse obstacle avoidance

            // Skip over objects that are not other boids and skip over self
            if (subordinate != null && subordinate != this) {
                neighborList.Add (go);
            }
        }

        var neighbors = neighborList.ToArray ();
        // Calculate neighbor forces
        var finalForce = Vector3.zero;

        finalForce += _CalculateCohesionForce (neighbors).normalized * _boidManager.CohesionWeight;
        finalForce += _CalculateAlignmentForce (neighbors).normalized * _boidManager.AlignmentWeight;
        finalForce += _CalculateSeparationForce (neighbors).normalized * _boidManager.SeparationWeight;
        finalForce += _CalculateLeaderForce ().normalized * _boidManager.LeadederWeight;

        //Debug.Log (finalForce);
        // apply force to RB
        _rigidbody.AddForce (finalForce);

        // set forward vector direction to current velocity
        transform.rotation = Quaternion.LookRotation (_rigidbody.velocity.normalized);

        // Limit Speed
        if (Vector3.Magnitude (_rigidbody.velocity) > _boidManager.MaximumSpeed) {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _boidManager.MaximumSpeed;
        }
	}



    public void SetBoidManager (BoidManager bm) {
        _boidManager = bm;
    }

    private void _BoidMovementLoop () {
        
    }

    private Vector3 _CalculateCohesionForce (GameObject[] neighbors) {
        if (neighbors.Length == 0) return Vector3.zero;

        var cohesivePos = new Vector3 ();

        foreach (var boid in neighbors) {
            cohesivePos += boid.transform.position;
        }

        cohesivePos /= neighbors.Length;

        var result = cohesivePos - transform.position;
        return result;
    }

    private Vector3 _CalculateSeparationForce (GameObject[] neighbors) {
        if (neighbors.Length == 0) return Vector3.zero;

        var repulsiveForce = Vector3.zero;

        foreach (var boid in neighbors) {
            float distance = Vector3.Magnitude (boid.transform.position - transform.position);
            if (distance < _boidManager.SeparationRange) {
                repulsiveForce += transform.position - boid.transform.position;
            }
        }

        return repulsiveForce;
    }

    private Vector3 _CalculateAlignmentForce (GameObject[] neighbors) {
        if (neighbors.Length == 0) return Vector3.zero;

        var steerVector = Vector3.zero;

        foreach (var boid in neighbors) {
            steerVector += boid.GetComponent<Rigidbody> ().velocity;
        }

        steerVector /= neighbors.Length;

        return steerVector;
    }

    private Vector3 _CalculateLeaderForce () {
        if (_boidManager.BoidLeader == null)
            return Vector3.zero;
        
        return (_boidManager.BoidLeader.transform.position - transform.position).normalized;
    }
}
