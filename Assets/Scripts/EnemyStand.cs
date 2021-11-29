using UnityEngine;
using System.Collections;

public class EnemyStand : MonoBehaviour
{
    private Animator enemyAnimator;
    private Rigidbody rb;
    [SerializeField] GameObject katana;

    private bool isTouch = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTouch)
        {
            isTouch = true;
            katana.GetComponent<Rigidbody>().useGravity = true;
            other.GetComponent<Player>().EnemyStandKill(gameObject);
        }
    }

    public void DeadStart(bool _bounce)
    {
        enemyAnimator.SetBool("Death", true);

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
        }
    }
}