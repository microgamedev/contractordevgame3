using Dreamteck.Splines;
using System.Collections;
using UnityEngine;

public class EnemyRun : MonoBehaviour
{
    public enum RunMode
    {
        GlobalZ,
        Spline,
    }

    private float enemyRunSpeed = 4f;
    private Rigidbody rb;
    private Player player;
    private Animator enemyAnimator;
    [SerializeField] GameObject snakeSegmentPrefab;
    [SerializeField] RunMode runMode;

    private bool isDeath = false;
    private bool isRun = false;
    SplineComputer spline;
    GameManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (GetComponent<Animator>() != null)
        {
            enemyAnimator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if(!isDeath)
        {
            snakeSegmentPrefab.GetComponent<SnakeSegment>().SnakeSegmentIsInactive(false);
        }
        spline = manager.splineComputer;
    }

    private void FixedUpdate()
    {
        if(!isDeath)
        {
            if (!isRun)
            {
                ReadyToRun();
            }
            if (isRun && !isDeath)
            {
                ReadyToStop();
            }
        }

        if (isRun && !isDeath)
        {
            Vector3 _forward = new Vector3(0, 0, 1);
            _forward = _forward.normalized * enemyRunSpeed * Time.deltaTime;

            var nextPosition = rb.position + _forward;
            var splineSample = spline.Project(nextPosition);

            switch (runMode)
            {
                case RunMode.GlobalZ:
                    break;
                case RunMode.Spline:
                    var splineDirection = splineSample.forward;
                    _forward = splineDirection;
                    splineDirection.y = 0;
                    transform.forward = splineDirection;
                    break;
            }

            float distance = Vector3.Distance(nextPosition, splineSample.position);

            if (distance > manager.RoadWidth)
            {
                var correctionDelta = distance - manager.RoadWidth;
                var correctionVector = (splineSample.position - nextPosition).normalized * correctionDelta;
                correctionVector.y = 0; // Проверить на холмах
                nextPosition += correctionVector;
            }

            rb.MovePosition(nextPosition + _forward);
        }
    }


    private void ReadyToRun()
    {
        if(transform.position.z - player.gameObject.transform.position.z < 6f)
        {
            RunAway();
        }
    }

    private void ReadyToStop()
    {
        if (transform.position.z < player.gameObject.transform.position.z)
        {
            RunStop();
        }
    }

    private void RunAway()
    {
        isRun = true;
        transform.rotation = Quaternion.Euler(0, 0f, 0);
        enemyAnimator.SetBool("Run", true);
    }

    private void RunStop()
    {
        isRun = false;
        enemyAnimator.SetBool("Run", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("Snake"))
        {
            if(!isDeath)
            {
                isDeath = true;
                snakeSegmentPrefab.SetActive(false);
                player.EnemyKillPlane(gameObject, true, 0);
            }
        }
    }

    public void EnemyDeath(bool _bounce)
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollider");

        isDeath = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        snakeSegmentPrefab.SetActive(false);

        rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
        }
    }
}