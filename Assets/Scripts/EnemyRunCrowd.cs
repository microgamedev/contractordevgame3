using System.Collections;
using UnityEngine;

public class EnemyRunCrowd : EnemyRun
{

    public bool isChest = false;
    [SerializeField] GameObject chestObject;

    //TODO: needs refactor
    protected override void OnTriggerEnter(Collider other)
    {
        //TODO: no base method call
        if (other.CompareTag("Player") || other.CompareTag("Snake"))
        {
            if(!isDeath)
            {
                isDeath = true;

                if(isChest)
                {
                    Destroy(chestObject);
                    player.EnemyKillPlane(gameObject, false, 50);
                }
                else
                {
                    player.EnemyKillPlane(gameObject, false, 0);
                }
            }
        }
    }

    //TODO: needs refactor
    public override void EnemyDeath(bool _bounce)
    {
        //TODO: no base method call
        gameObject.layer = LayerMask.NameToLayer("NoCollider");

        isDeath = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        Destroy(chestObject);

        rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);

        if (_bounce)
        {
            rb.AddForce(new Vector3(Random.Range(0.25f, 0.5f), 1f, Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
        }
    }
}