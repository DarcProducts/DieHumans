using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Range(5f, 30f)] [SerializeField] private float maxLiDARSensorDistance;
    [SerializeField] private float distanceToAttackPlayer;
    [SerializeField] private float decisionTime;
    private EnemyManager enemyManager;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 nextTravelDirection;
    [SerializeField] private LiDARSensor[] sensors;
    [SerializeField] private bool[] directionsAbleToTravel;
    [SerializeField] private float distanceToChangeDirection;
    private GameObject playerShip;

    [Tooltip("Shows the distances of each sensor; " +
        "(front, back, left, right, up, down, front left, front right, front up, front down) " +
        "in that order")]
    [SerializeField] private float[] sensorDistances;

    private void Awake()
    {
        if (enemyManager == null)
            enemyManager = FindObjectOfType<EnemyManager>();
        playerShip = GameObject.FindObjectOfType<PlayerShip>().gameObject;
    }

    private void Start()
    {
        InitializeLiDAR();
        sensorDistances = new float[sensors.Length];
        directionsAbleToTravel = new bool[sensors.Length];
        StartCoroutine(MakeADecisionThenWait(decisionTime));
    }

    private void FixedUpdate()
    {
        ProjectLiDAR(); 
        if (playerShip != null)
            transform.Rotate(transform.InverseTransformDirection(nextTravelDirection) * rotateSpeed * Time.fixedDeltaTime);
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.InverseTransformDirection(nextTravelDirection), moveSpeed * Time.fixedDeltaTime);
    }

    private void Attack()
    {
        if (enemyManager != null)
            if (enemyManager.CheckIfPathClear(gameObject, distanceToAttackPlayer))
                enemyManager.LaunchRocket(gameObject);
    }

    private IEnumerator MakeADecisionThenWait(float duration)
    {
        if (enemyManager != null)
        {
            if (enemyManager.CheckPlayerWithinRange(gameObject, distanceToAttackPlayer))
                Attack();
            if (GetShortestSensor().currentDistance < maxLiDARSensorDistance - distanceToChangeDirection)
            {
                Vector3[] directions = GetDirectionsAbleToTravel();
                if (directions.Length > 0)
                {
                    if (GetDirection(directions).Equals(transform.forward))
                        nextTravelDirection = Vector3.forward;
                    else
                    {
                        int randomDirection = Random.Range(0, directions.Length - 1);
                        nextTravelDirection = directions[randomDirection];
                    }
                }
            }
        }
        yield return new WaitForSeconds(duration);
        StopCoroutine(MakeADecisionThenWait(duration));
        StartCoroutine(MakeADecisionThenWait(duration));
    }

    private Vector3 GetDirection(Vector3[] directions)
    {
        foreach (Vector3 d in directions)
            if (d.Equals(Vector3.forward))
                return d;
        return transform.position;
    }

    private bool ProjectLiDAR()
    {
        for (int i = 0; i < sensors.Length; i++)
        {
            sensorDistances[i] = sensors[i].ShootRay(transform);
            if (sensorDistances[i] < distanceToChangeDirection)
                return false;
        }
        return true;
    }

    private Vector3[] GetDirectionsAbleToTravel()
    {
        List<Vector3> dAT = new List<Vector3>();
        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensorDistances[i] > maxLiDARSensorDistance - distanceToChangeDirection)
            {
                directionsAbleToTravel[i] = true;
                dAT.Add(sensors[i].direction);
            }
            else
                directionsAbleToTravel[i] = false;
        }
        return dAT.ToArray();
    }

    public LiDARSensor GetShortestSensor()
    {
        LiDARSensor testSensor = new LiDARSensor(Vector3.zero, maxLiDARSensorDistance, Color.black);
        testSensor.ShootRay(transform);
        foreach (LiDARSensor sensor in sensors)
            if (sensor.currentDistance < testSensor.currentDistance)
                testSensor = sensor;
        return testSensor;
    }

    private void InitializeLiDAR()
    {
        sensors = new LiDARSensor[10];
        LiDARSensor sensor0 = new LiDARSensor(transform.forward, maxLiDARSensorDistance, Color.blue); // forward sensor
        sensors[0] = sensor0;
        LiDARSensor sensor1 = new LiDARSensor(-transform.forward, maxLiDARSensorDistance - 4, Color.black); // backward sensor
        sensors[1] = sensor1;
        LiDARSensor sensor2 = new LiDARSensor(Vector3.Cross(transform.forward, Vector3.up).normalized, maxLiDARSensorDistance - 2, Color.cyan); // left sensor
        sensors[2] = sensor2;
        LiDARSensor sensor3 = new LiDARSensor(-Vector3.Cross(transform.forward, Vector3.up).normalized, maxLiDARSensorDistance - 2, Color.red); // right sensor
        sensors[3] = sensor3;
        LiDARSensor sensor4 = new LiDARSensor(Vector3.up, maxLiDARSensorDistance - 3, Color.yellow); // up sensor
        sensors[4] = sensor4;
        LiDARSensor sensor5 = new LiDARSensor(Vector3.down, maxLiDARSensorDistance - 5, Color.grey); // down sensor
        sensors[5] = sensor5;
        LiDARSensor sensor6 = new LiDARSensor(Vector3.Cross(transform.forward, Vector3.up).normalized + Vector3.forward, maxLiDARSensorDistance - 1, new Color(.6f, .6f, 1f)); // front left sensor
        sensors[6] = sensor6;
        LiDARSensor sensor7 = new LiDARSensor(-Vector3.Cross(transform.forward, Vector3.up).normalized + Vector3.forward, maxLiDARSensorDistance - 1, new Color(1f, .1f, 1f)); // front right sensor
        sensors[7] = sensor7;
        LiDARSensor sensor8 = new LiDARSensor(transform.forward + Vector3.up, maxLiDARSensorDistance - 2, Color.green); // front up sensor
        sensors[8] = sensor8;
        LiDARSensor sensor9 = new LiDARSensor(transform.forward + Vector3.down, maxLiDARSensorDistance - 4, new Color(.2f, .2f, .3f)); // front down sensor
        sensors[9] = sensor9;
    }
}