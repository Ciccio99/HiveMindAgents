using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoidManager : MonoBehaviour {

    // Accessible properties for the boids to access
    public float NeighborRange { get { return _boidNeighborRange; } private set { _boidNeighborRange = value; } }
    public float SeparationRange { get { return _boidSeperationRange; } private set { _boidSeperationRange = value; } }
    public float SeparationWeight { get { return _boidSeparationWeight; } private set { _boidSeparationWeight = value; } }
    public float AlignmentWeight { get { return _boidAlignmentWeight; } private set { _boidAlignmentWeight = value; } }
    public float CohesionWeight { get { return _boidCohesionWeight; } private set { _boidCohesionWeight = value; } }
    public float AvoidanceWeight { get { return _boidAvoidanceWeight; } private set { _boidAvoidanceWeight = value; } }
    public float LeadederWeight { get { return _boidLeaderWeight; } private set { _boidLeaderWeight = value; } }
    public float MaximumSpeed { get { return _boidMaximumSpeed; } set { _boidMaximumSpeed = value; } }

    public GameObject BoidLeader { get { return _boidLeader; } private set { _boidLeader = value; } }

    [SerializeField]
    private int _boidCount;
    [SerializeField]
    private float _boidMaximumSpeed = 10f;
    [SerializeField]
    private GameObject _boidLeader;
    [SerializeField]
    private GameObject[] _boidPrefabs;
    [SerializeField]
    private float _boidNeighborRange, _boidSeperationRange, _boidSeparationWeight, _boidAlignmentWeight, _boidCohesionWeight, _boidAvoidanceWeight, _boidLeaderWeight;
   

    private GameObject[] _managedBoidGOs;
    private BoidSubordinate[] _managedBoidSubordinates;

    private void Awake () {
        if (_boidPrefabs.Length == 0)
            throw new NullReferenceException ("BoidManager: Missing boid prefab references...");
    }

    // Mono Functions
    private void OnEnable () {
        _CreateBoids();
    }

    public void ResetBoids () {
        _DestroyBoids();
        _CreateBoids();
    }

    private void _CreateBoids () {
        if (_managedBoidGOs == null)
            _managedBoidGOs = new GameObject[_boidCount];
        if (_managedBoidSubordinates == null)
            _managedBoidSubordinates = new BoidSubordinate[_boidCount];

        for (int i = 0; i < _boidCount; i++) {
            var prefab = _GetBoidPrefab ();
            var boid = Instantiate (prefab, transform.position, Quaternion.identity);
            var boidSub = boid.AddComponent<BoidSubordinate> ();

            boidSub.SetBoidManager (this);

            _managedBoidGOs[i] = boid;
            _managedBoidSubordinates[i] = boidSub;
        }
    }

    private GameObject _GetBoidPrefab (int index = -1) {
        int randIndex = UnityEngine.Random.Range (0, _boidPrefabs.Length);
        return _boidPrefabs[randIndex];
    }

    private void _DestroyBoids () {
        foreach (var boid in _managedBoidGOs) {
            Destroy (boid);
        }
        _managedBoidGOs = new GameObject[_boidCount];
        _managedBoidSubordinates = new BoidSubordinate[_boidCount];
    }
}
