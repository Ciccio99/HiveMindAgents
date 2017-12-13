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

    void FixedUpdate () {
        _BoidMovementLoop ();
	}

    public void SetBoidManager (BoidManager bm) {
        _boidManager = bm;
    }

    /// <summary>
    /// BOIDs' the movement loop.
    /// </summary>
    private void _BoidMovementLoop () {
        var neighbors = _GetNeighbors ();

        // Calculate neighbor forces
        var finalForce = Vector3.zero;

        finalForce += _CalculateSeparationForce (neighbors);
        finalForce += _CalculateCohesionForce (neighbors);
        finalForce += _CalculateAlignmentForce (neighbors);
        finalForce += _CalculateLeaderForce ();
        finalForce += _CalculateObjectAvoidanceForce ();

      
        // apply force to RB
        if (float.IsNaN (finalForce.x) || float.IsNaN (finalForce.y) || float.IsNaN (finalForce.z)) 
            finalForce = Vector3.zero;
        
        _rigidbody.AddForce (finalForce);

        // set forward vector direction to current velocity
        var lookRot = _rigidbody.velocity.normalized;
        if (lookRot != Vector3.zero)
            transform.rotation = Quaternion.LookRotation (lookRot);
        
        _ClampMaxSpeed (); 
    }

    /// <summary>
    /// Gets the neighbors with sphere colliders
    /// </summary>
    /// <returns>The neighbors.</returns>
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

    /// <summary>
    /// Gets the neighbors attached to the BoidManager
    /// </summary>
    /// <returns>The neighbors.</returns>
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

    /// <summary>
    /// Clamps the max speed of the boid's vector.
    /// </summary>
    private void _ClampMaxSpeed () {
        // Limit Speed
        if (Vector3.Magnitude (_rigidbody.velocity) > _boidManager.MaximumSpeed) {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _boidManager.MaximumSpeed;
        }
    }

    /// <summary>
    /// Calculates the cohesion force.
    /// </summary>
    /// <returns>The cohesion force.</returns>
    /// <param name="neighbors">Neighbors.</param>
    private Vector3 _CalculateCohesionForce (GameObject[] neighbors) {
        if (neighbors.Length == 0) return Vector3.zero;

        var cohesivePos = new Vector3 ();

        var cohesionDistSqr = _boidManager.NeighborRange * _boidManager.NeighborRange / 2;

        foreach (var boid in neighbors) {
            cohesivePos += boid.transform.position;
        }

        cohesivePos /= neighbors.Length;

        var distFromCenter = (cohesivePos - transform.position).sqrMagnitude;

        var weightPercentage = distFromCenter / cohesionDistSqr;

        var cohesiveForce = (cohesivePos - transform.position).normalized;

        cohesiveForce *= weightPercentage * _boidManager.CohesionWeight;

        return cohesiveForce;
    }

    /// <summary>
    /// Calculates the separation force.
    /// </summary>
    /// <returns>The separation force.</returns>
    /// <param name="neighbors">Neighbors.</param>
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

    /// <summary>
    /// Calculates the alignment force.
    /// </summary>
    /// <returns>The alignment force.</returns>
    /// <param name="neighbors">Neighbors.</param>
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

    /// <summary>
    /// Calculates the leader force.
    /// </summary>
    /// <returns>The leader force.</returns>
    private Vector3 _CalculateLeaderForce () {
        if (_boidManager.BoidLeader.transform.position == Vector3.zero)
            return Vector3.zero;
        var boidLeaderPos = _boidManager.BoidLeader.transform.position;

        var arrivalDistanceSqr = _boidManager.LeaderArrivalRange * _boidManager.LeaderArrivalRange;

        var deltaFromLeaderSqr = (transform.position - boidLeaderPos).sqrMagnitude;

        // Notify manager when close to leader so that it changes to the next part of path
        if (deltaFromLeaderSqr < arrivalDistanceSqr / 2f) {
            _boidManager.NotifyLeaderArrival ();
        }

        // The closer the boid gets, the less the leader weight will be applied
        var weightPercentage = deltaFromLeaderSqr / arrivalDistanceSqr;

        var leaderVect = (boidLeaderPos - transform.position).normalized;
        
        //var leaderForce = leaderVect * weightPercentage * _boidManager.LeadederWeight;
        var leaderForce = leaderVect * _boidManager.LeadederWeight;
        var final = leaderForce - _rigidbody.velocity;

        return final;
    }

    /// <summary>
    /// Calculates the object avoidance force.
    /// </summary>
    /// <returns>The object avoidance force.</returns>
    private Vector3 _CalculateObjectAvoidanceForce () {
        RaycastHit hit;

        if (Physics.Raycast (transform.position, transform.forward, out hit, _boidManager.AvoidanceRange)) {
            var avoidanceDistSqr = _boidManager.AvoidanceRange * _boidManager.AvoidanceRange;
            var vectSubtraction = transform.position - hit.point;
            var distFromObjectSqr = vectSubtraction.sqrMagnitude;

            // Get the reflection of the current forward vector and the normal of the object hit point
            var avoidVect = Vector3.Reflect (transform.forward, hit.normal).normalized;
            var weightPercentage = 1f - (distFromObjectSqr / avoidanceDistSqr);

                
            var force = avoidVect * weightPercentage * _boidManager.AvoidanceWeight;
            return force;
        }

        return Vector3.zero;
    }
}
