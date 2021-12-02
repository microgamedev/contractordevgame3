using UnityEngine;
using System.Collections;

public class EnemyStand : MonoBehaviour
{
    private Rigidbody rb;
    private Player player;
    [SerializeField] GameObject snakeSegmentPrefab;

    private bool isDeath = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        if (!isDeath)
        {
            snakeSegmentPrefab.GetComponent<SnakeSegment>().SnakeSegmentIsInactive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Snake"))
        {
            if (!isDeath)
            {
                isDeath = true;
                snakeSegmentPrefab.SetActive(false);
                player.EnemyKillPlane(gameObject, true);
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