using UnityEngine;
using BzKovSoft.ObjectSlicer;

public class SnakeSegment : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private Animation _animation;
    private Player player;
    private bool isActive = true;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        sphereCollider = GetComponent<SphereCollider>();
        _animation = GetComponentInChildren<Animation>();
    }

    public void AddToSnake()
    {
        isActive = false;
        transform.SetParent(null);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        gameObject.layer = LayerMask.NameToLayer("Player");
        transform.tag = "Snake";
        player.SnakeSegmentAdd(gameObject);
    }

    public void SnakeSegmentIsInactive(bool isAnimation)
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollider");
        isActive = false;
        sphereCollider.enabled = false;

        if (!isAnimation)
        {
            _animation.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bamboo"))
        {
            player.SliceBamboo(other.gameObject);
        }

        if (other.CompareTag("Lamp"))
        {
            player.SliceLamp(other.gameObject);
        }

        if (isActive)
        {
            if (other.CompareTag("Snake") || other.CompareTag("Player"))
            {
                AddToSnake();
            }
        }
    }
}