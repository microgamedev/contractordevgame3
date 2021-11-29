using UnityEngine;
using BzKovSoft.ObjectSlicer;

public class SnakeSegment : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private Player player;
    private bool isActive = true;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void AddToSnake()
    {
        isActive = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        transform.tag = "Snake";
        player.SnakeSegmentAdd(gameObject);
    }

    public void AddToStone()
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollider");
        isActive = false;
        sphereCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bamboo"))
        {
            other.GetComponentInChildren<ParticleSystem>().Play();

            var sliceable = other.GetComponent<IBzSliceable>();
            Plane plane = new Plane(transform.up, (-transform.position.y + 0.05f));
            sliceable.Slice(plane, r =>
            {
                if (!r.sliced)
                {
                    return;
                }

                r.outObjectPos.gameObject.GetComponent<Bamboo>().Sliced(true);
                r.outObjectNeg.gameObject.GetComponent<Bamboo>().Sliced(false);
            }
            );
        }

        if(isActive)
        {
            if (other.CompareTag("Snake") || other.CompareTag("Player"))
            {
                AddToSnake();
            }
        }
    }
}