using UnityEngine;
using BzKovSoft.ObjectSlicer;

public class SnakePart : MonoBehaviour
{
    private Player player;

    public void AddToSnake()
    {
        gameObject.layer = LayerMask.NameToLayer("Snake");
        transform.tag = "Snake";
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bamboo"))
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

        if (other.CompareTag("SnakePart"))
        {
            player.NewSnakePart(other.gameObject);
        }
    }
}