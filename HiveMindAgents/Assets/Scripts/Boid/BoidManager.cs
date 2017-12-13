/*
    Author: Alberto Scicali
    Central boid manager that controls the weights and values associated with boids
*/

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

    // Serialized fields to be exposed to inspector
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
    [SerializeField]
    private Camera _boidFollowCamera;

    [Space(5)]
    [Header("Boid Range Values")]
    [SerializeField]
    [Range(0,1000)]
    private float _boidNeighborRange;
    [SerializeField]
    [Range (0, 1000)]
    private float _boidSeperationRange, _boidAvoidanceDetectionRange, _boidLeaderArrivalRange;

    [Space (5)]
    [Header ("Boid Weights")]
    [SerializeField]
    [Range (0, 1000)]
    private float _boidSeparationWeight;
    [SerializeField]
    [Range (0, 1000)]
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

    private void OnEnable () {
        _CreateBoids();
    }

    /// <summary>
    /// Notifies that a boid has arrived at the leader. If the leader is associated with a path then the leader will be swapped with the next path vertex
    /// 
    /// </summary>
    public void NotifyLeaderArrival () {
        if (_boidPather == null || _boidPather.pathVertices.Length <= 0) return;

        if (currentPathPointIndex >= _boidPather.pathVertices.Length && _boidPather.looping)
            currentPathPointIndex = 0;
        else if (currentPathPointIndex >= _boidPather.pathVertices.Length)
            return;

        _boidLeader.transform.position = _boidPather.pathVertices[currentPathPointIndex++];
        
    }

    /// <summary>
    /// Resets the boids.
    /// </summary>
    public void ResetBoids () {
        _DestroyBoids();
        _CreateBoids();
    }

    /// <summary>
    /// Creates and instantiates the boid objects.
    /// THe boids are spawned randomly around a 4x4x4 cube around the manager.
    /// </summary>
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

        if (_boidFollowCamera != null) {
            var folowCam = _boidFollowCamera.gameObject.AddComponent <BoidFollowCamera>();
            folowCam.SetFollowTargetObject (_managedBoids[0].go);
        }

       
    }

    /// <summary>
    /// Gets a random prefab from the list of boids prefabs.
    /// </summary>
    /// <returns>The BOID prefab.</returns>
    /// <param name="index">Index.</param>
    private GameObject _GetBoidPrefab (int index = -1) {
        int randIndex = UnityEngine.Random.Range (0, _boidPrefabs.Length);
        return _boidPrefabs[randIndex];
    }

    /// <summary>
    /// Destroys the boids.
    /// </summary>
    private void _DestroyBoids () {
        foreach (var boid in _managedBoids) {
            Destroy (boid.go);
        }
        _managedBoids = new BoidObject[_boidCount];
    }

}

/// <summary>
/// Maintains information about a spawned boid
/// </summary>
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
