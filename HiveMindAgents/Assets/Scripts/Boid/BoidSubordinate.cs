using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoidSubordinate : MonoBehaviour {

    private BoidManager _boidManager;
    private Rigidbody _rigidbody;

    private void OnEnable () {
        _rigidbody = GetComponent<Rigidbody> ();
    }

    // Update is called once per frame
    void FixedUpdate () {
        _BoidMovementLoop ();
	}

    public void SetBoidManager (BoidManager bm) {
        _boidManager = bm;
    }

    private void _BoidMovementLoop () {
        var neighbors = _GetNeighbors ();

        // Calculate neighbor forces
        var finalForce = Vector3.zero;

        finalForce += _CalculateCohesionForce (neighbors);
        finalForce += _CalculateAlignmentForce (neighbors);
        finalForce += _CalculateSeparationForce (neighbors);
        finalForce += _CalculateLeaderForce ();

      
        // apply force to RB
        if (float.IsNaN (finalForce.x) || float.IsNaN (finalForce.y) || float.IsNaN (finalForce.z)) 
            finalForce = Vector3.zero;
        
        _rigidbody.AddForce (finalForce);

        // set forward vector direction to current velocity
        if (_rigidbody.velocity.normalized != Vector3.zero)
            transform.rotation = Quaternion.LookRotation (_rigidbody.velocity.normalized);
        
        _ClampMaxSpeed (); 
    }

    //private GameObject[] _GetNeighbors () {
    //    // Get nieghbors
    //    var neighborColliders = Physics.OverlapSphere (transform.position, _boidManager.NeighborRange);

    //    var neighborList = new List<GameObject> ();
    //    foreach (var col in neighborColliders) {
    //        var go = col.gameObject;
    //        var subordinate = go.GetComponent<BoidSubordinate> ();
    //        // TO DO: Add in list of non boid neighbors that will add to the respulse obstacle avoidance

    //        // Skip over objects that are not other boids and skip over self
    //        if (subordinate != null && subordinate != this) {
    //            neighborList.Add (go);
    //        }
    //    }

    //    return neighborList.ToArray ();
    //}

    private GameObject[] _GetNeighbors () {
        // Get nieghbors
        var neighborList = new List<GameObject> ();
        var neighborRangeSqr = _boidManager.NeighborRange * _boidManager.NeighborRange;
        foreach (var boid in _boidManager.ManagedBoids) {
            
            // TO DO: Add in list of non boid neighbors that will add to the respulse obstacle avoidance

            // Skip over objects that are not other boids and skip over self
            var boidDistSqr = (boid.go.transform.position - transform.position).sqrMagnitude;
            if ( boidDistSqr > 0f && boidDistSqr < neighborRangeSqr)
                neighborList.Add (boid.go);
        }

        return neighborList.ToArray ();
    }

    private void _ClampMaxSpeed () {
        // Limit Speed
        if (Vector3.Magnitude (_rigidbody.velocity) > _boidManager.MaximumSpeed) {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _boidManager.MaximumSpeed;
        }
    }

    private Vector3 _CalculateCohesionForce (GameObject[] neighbors) {
        if (neighbors.Length == 0) return Vector3.zero;

        var cohesivePos = new Vector3 ();

        foreach (var boid in neighbors) {
            cohesivePos += boid.transform.position;
        }

        cohesivePos /= neighbors.Length;

        var result = cohesivePos - transform.position;
        return result.normalized;
    }

    private Vector3 _CalculateSeparationForce (GameObject[] neighbors) {
        if (neighbors.Length == 0) return Vector3.zero;

        var repulsiveForce = Vector3.zero;
        var avgDistance = 0f;
        int tooCloseCount = 0;
        var separationDistSqr = _boidManager.SeparationRange * _boidManager.SeparationRange;

        foreach (var boid in neighbors) {
            var vectSub = transform.position - boid.transform.position;
            float distanceSqr = vectSub.sqrMagnitude;
            if (distanceSqr < separationDistSqr) {
                repulsiveForce += vectSub;
                avgDistance += distanceSqr;
                tooCloseCount++;
            }
        }
        // Return if there are no other boids within the separation range
        if (tooCloseCount == 0) return Vector3.zero;

        // Get avgs
        repulsiveForce /= tooCloseCount;
        avgDistance /= tooCloseCount;

        var weightPercentage = 1f - (avgDistance / separationDistSqr);

        repulsiveForce = repulsiveForce.normalized * weightPercentage * _boidManager.SeparationWeight;

        return repulsiveForce;
    }

    private Vector3 _CalculateAlignmentForce (GameObject[] neighbors) {
        if (neighbors.Length == 0) return Vector3.zero;

        var steerVector = Vector3.zero;

        foreach (var boid in neighbors) {
            steerVector += boid.GetComponent<Rigidbody> ().velocity;
        }

        steerVector /= neighbors.Length;

        var steerForce = steerVector.normalized * _boidManager.AlignmentWeight;

        return steerForce;
    }

    private Vector3 _CalculateLeaderForce () {
        if (_boidManager.BoidLeader == null)
            return Vector3.zero;
        
        var leaderForce = (_boidManager.BoidLeader.transform.position - transform.position).normalized * _boidManager.LeadederWeight;

        return leaderForce;
    }
}
