using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Rigidbody rb;
    private Player player;

    private bool isTouch = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Snake")) && !isTouch)
        {
            isTouch = true;
            player.EnemyStandKill(gameObject);
        }
    }

    public void DeadStart(bool _bounce)
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollider");

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
        }
    }
}