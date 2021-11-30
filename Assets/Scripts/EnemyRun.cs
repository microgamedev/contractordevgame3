using System.Collections;
using UnityEngine;

public class EnemyRun : MonoBehaviour
{
    private float enemyRunSpeed = 4;
    private Rigidbody rb;
    private Player player;
    private Animator enemyAnimator;

    private bool isDeath = false;
    private bool isRun = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();

        if(GetComponent<Animator>() != null)
        {
            enemyAnimator = GetComponent<Animator>();
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
                player.EnemyRunKill(gameObject);
            }
        }
    }

    public void EnemyDeath(bool _bounce)
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollider");

        isDeath = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
        }
    }
}