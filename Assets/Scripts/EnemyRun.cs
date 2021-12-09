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

    protected float enemyRunSpeed = 4f;
    protected Rigidbody rb;
    protected Player player;
    protected Animator enemyAnimator;
    [SerializeField] GameObject snakeSegmentPrefab;
    [SerializeField] RunMode runMode;

    protected bool isDeath = false;
    protected bool isRun = false;
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
        if(!isDeath && snakeSegmentPrefab != null)
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
            Vector3 _forward = Vector3.zero;

            var splineSample = spline.Project(transform.position);

            switch (runMode)
            {
                case RunMode.GlobalZ:
                    _forward = Vector3.forward;
                    break;
                case RunMode.Spline:
                    _forward = splineSample.forward;
                    break;
            }
            _forward = _forward.normalized * enemyRunSpeed * Time.fixedDeltaTime;

            var nextPosition = rb.position + _forward;
            float distance = Vector3.Distance(nextPosition, splineSample.position);

            if (distance > manager.RoadWidth)
            {
                var correctionDelta = distance - manager.RoadWidth;
                var correctionVector = (splineSample.position - nextPosition).normalized * correctionDelta;
                correctionVector.y = 0; // Проверить на холмах
                nextPosition += correctionVector;
            }

            rb.MovePosition(nextPosition);
            var rotationLook = _forward;
            rotationLook.y = 0.0f;
            rb.MoveRotation(Quaternion.LookRotation(rotationLook.normalized));
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("Snake"))
        {
            if(!isDeath)
            {
                isDeath = true;
                if (snakeSegmentPrefab != null)
                {
                    snakeSegmentPrefab.SetActive(false);
                }
                player.EnemyKillPlane(gameObject, true, 0);
            }
        }
    }

    public virtual void EnemyDeath(bool _bounce)
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollider");

        isDeath = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        if (snakeSegmentPrefab != null)
        {
            snakeSegmentPrefab.SetActive(false);
        }
        rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
        }

        Destroy(gameObject, 3f);
    }
}