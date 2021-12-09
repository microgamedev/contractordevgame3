using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    private Rigidbody rb;
    private Player player;
    [SerializeField] private ParticleSystem fx;

    private bool isDeath = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Snake"))
        {
            if (!isDeath)
            {
                isDeath = true;
                player.EnemyKillPlane(gameObject, false, 10);
            }
        }
    }

    public void EnemyDeath(bool _bounce)
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollider");

        isDeath = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        //rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 1f, ForceMode.Impulse);
            Destroy(gameObject, 3f);
        }
        else
        {
            Destroy(gameObject, 1f);
        }

        fx.Play();

    }
}
