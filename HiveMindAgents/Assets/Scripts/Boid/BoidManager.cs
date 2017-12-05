using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoidManager : MonoBehaviour {

    // Accessible properties for the boids to access
    public float NeighborRange { get { return _boidNeighborRange; } private set { _boidNeighborRange = value; } }
    public float SeparationRange { get { return _boidSeperationRange; } private set { _boidSeperationRange = value; } }
    public float AvoidanceRange { get { return _boidAvoidanceDetectionRange; } private set { _boidAvoidanceWeight = value; } }
    public float LeaderArrivalRange { get { return _boidLeaderArrivalRange;  } private set { _boidLeaderArrivalRange = value; } }
    public float SeparationWeight { get { return _boidSeparationWeight; } private set { _boidSeparationWeight = value; } }
    public float AlignmentWeight { get { return _boidAlignmentWeight; } private set { _boidAlignmentWeight = value; } }
    public float CohesionWeight { get { return _boidCohesionWeight; } private set { _boidCohesionWeight = value; } }
    public float AvoidanceWeight { get { return _boidAvoidanceWeight; } private set { _boidAvoidanceWeight = value; } }
    public float LeadederWeight { get { return _boidLeaderWeight; } private set { _boidLeaderWeight = value; } }
    public float MaximumSpeed { get { return _boidMaximumSpeed; } set { _boidMaximumSpeed = value; } }
    public BoidObject[] ManagedBoids { get { return _managedBoids; } private set { _managedBoids = value; } }


    public Vector3[] BoidPathPoints { get { return _boidPather != null ?_boidPather.pathVertices : null; } private set { _boidPather.pathVertices = value; }}
    public GameObject BoidLeader { get { return _boidLeader; } private set { _boidLeader = value; } }

    [SerializeField]
    private int _boidCount;
    [SerializeField]
    private float _boidMaximumSpeed = 10f;
    [SerializeField]
    private BoidPather _boidPather;
    [SerializeField]
    private GameObject _boidLeader;
    [SerializeField]
    private GameObject[] _boidPrefabs;

    [Space(5)]
    [Header("Boid Range Values")]
    [SerializeField]
    private float _boidNeighborRange;
    [SerializeField]
    private float _boidSeperationRange, _boidAvoidanceDetectionRange, _boidLeaderArrivalRange;

    [Space (5)]
    [Header ("Boid Weights")]
    [SerializeField]
    private float _boidSeparationWeight;
    [SerializeField]
    private float _boidAlignmentWeight, _boidCohesionWeight, _boidAvoidanceWeight, _boidLeaderWeight;
   

    private BoidObject[] _managedBoids;

    private int currentPathPointIndex = 0;

    private void Awake () {
        if (_boidPrefabs.Length == 0)
            throw new NullReferenceException ("BoidManager: Missing boid prefab references...");

        if (_boidPather != null && _boidPather.pathVertices.Length > 0) {
            _boidLeader = new GameObject ("PathLeader");
            _boidLeader.transform.position = _boidPather.pathVertices[currentPathPointIndex++];
        }
    }

    // Mono Functions
    private void OnEnable () {
        _CreateBoids();
    }

    public void NotifyLeaderArrival () {
        if (_boidPather == null || _boidPather.pathVertices.Length <= 0) return;

        if (currentPathPointIndex >= _boidPather.pathVertices.Length && _boidPather.looping)
            currentPathPointIndex = 0;
        else if (currentPathPointIndex >= _boidPather.pathVertices.Length)
            return;

        _boidLeader.transform.position = _boidPather.pathVertices[currentPathPointIndex++];
        
    }

    public void ResetBoids () {
        _DestroyBoids();
        _CreateBoids();
    }

    private void _CreateBoids () {
        if (_managedBoids == null)
            _managedBoids = new BoidObject[_boidCount];

        for (int i = 0; i < _boidCount; i++) {
            var prefab = _GetBoidPrefab ();
            var startPos = new Vector3 (UnityEngine.Random.Range (-2f, 2f), UnityEngine.Random.Range (-2f, 2f), UnityEngine.Random.Range (-2f, 2f));
            var boid = Instantiate (prefab, transform.position + startPos, Quaternion.identity);
            boid.AddComponent<BoidSubordinate> ().SetBoidManager (this) ;

            _managedBoids[i] = new BoidObject (boid);
        }

        Camera.main.transform.SetParent (_managedBoids[0].go.transform);
        Camera.main.transform.localPosition = Vector3.forward * -4f;
    }

    private GameObject _GetBoidPrefab (int index = -1) {
        int randIndex = UnityEngine.Random.Range (0, _boidPrefabs.Length);
        return _boidPrefabs[randIndex];
    }

    private void _DestroyBoids () {
        foreach (var boid in _managedBoids) {
            Destroy (boid.go);
        }
        _managedBoids = new BoidObject[_boidCount];
    }

}

public struct BoidObject {
    public GameObject go;
    public BoidSubordinate boidSubordinate;
    public Rigidbody rigidbody;

    public BoidObject (GameObject boid) {
        go = boid;
        boidSubordinate = boid.GetComponent<BoidSubordinate> ();
        rigidbody = boid.GetComponent<Rigidbody> ();

        if (boidSubordinate == null)
            throw new NullReferenceException ("BoidObject: Gameobject does not have a BoidSubordinate component...");
        if (rigidbody == null)
            throw new NullReferenceException ("BoidObject: Gameobject does not have a Rigidbody component...");
    }


}
