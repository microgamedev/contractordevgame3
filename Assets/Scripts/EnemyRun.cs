using System.Collections;
using UnityEngine;

public class EnemyRun : MonoBehaviour
{
    private float enemyRunSpeed = 4f;
    private Rigidbody rb;
    private Player player;
    private Animator enemyAnimator;
    [SerializeField] GameObject snakeSegmentPrefab;

    private bool isDeath = false;
    private bool isRun = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();

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
    }

    private void Update()
    {
        if (isRun && !isDeath)
        {
            Vector3 _forward = new Vector3(0, 0, 1);
            _forward = _forward.normalized * enemyRunSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + _forward);
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