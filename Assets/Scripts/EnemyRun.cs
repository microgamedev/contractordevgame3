using System.Collections;
using UnityEngine;

public class EnemyRun : MonoBehaviour
{
    [SerializeField] float enemyRunSpeed;
    [SerializeField] GameObject shurikenPrefab;
    private Rigidbody rb;
    private Player player;
    private Animator enemyAnimator;

    private bool isDeath = false;
    private bool isRun = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(!isDeath)
        {
            if (!isRun)
            {
                ReadyToRun();
            }
            if (isRun)
            {
                ReadyToStop();
            }
        }
    }

    private void Update()
    {
        if (isRun && !isDeath)
        {
            transform.Translate(Vector3.forward * enemyRunSpeed * Time.deltaTime);
        }
    }

    private void ReadyToRun()
    {
        if(transform.position.z - player.gameObject.transform.position.z < 8f)
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
                Death();

                player.EnemyRunKill(gameObject);
            }
        }
    }

    public void DeadStart(bool _bounce)
    {
        isDeath = true;

        gameObject.layer = LayerMask.NameToLayer("Default");

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 0.3f, ForceMode.Impulse);
        }
    }

    private void Death()
    {
        isDeath = true;
        Destroy(shurikenPrefab);
        enemyAnimator.SetBool("Death", true);
    }
}