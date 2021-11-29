using UnityEngine;
using BzKovSoft.ObjectSlicer;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] SphereCollider sphereCollider;
    private Player player;
    private bool isActive = true;

    public void AddToSnake()
    {
        if(isActive)
        {
            gameObject.layer = LayerMask.NameToLayer("Snake");
            transform.tag = "Snake";
            player = FindObjectOfType<Player>();
        }
    }

    public void AddToStone()
    {
        gameObject.layer = LayerMask.NameToLayer("Solid");
        isActive = false;
        sphereCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bamboo") && isActive)
        {
            if (!other.GetComponent<Bamboo>().isSliced)
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
        }

        if (other.CompareTag("SnakePart") && isActive)
        {
            player.SnakeSegmentAdd(other.gameObject);
        }
    }
}