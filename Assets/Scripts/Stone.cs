using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] ParticleSystem stoneFX;
    [SerializeField] GameObject snakeSegment;
    private Player player;
    private bool isTouch = false;
    private Rigidbody rb;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !isTouch)
        {
            isTouch = true;

            GameObject _snakeSegment = Instantiate(snakeSegment);
            _snakeSegment.GetComponent<SnakeSegment>().AddToStone();
            _snakeSegment.transform.SetParent(transform, false);
            _snakeSegment.transform.position = other.GetContact(0).point;

            gameObject.layer = LayerMask.NameToLayer("NoCollider");
            rb.isKinematic = false;
            rb.AddForce(0f, -0.25f, 0.5f, ForceMode.Impulse);

            stoneFX.Play();

            player.StoneTouch();


        }
    }
}