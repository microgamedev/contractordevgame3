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
            _snakeSegment.GetComponent<SnakeSegment>().SnakeSegmentIsInactive(false);
            _snakeSegment.transform.SetParent(transform, false);

            Vector3 _pos = other.GetContact(0).point;
            _pos.y -= 0.025f;
            _snakeSegment.transform.position = _pos;

            _snakeSegment.transform.rotation = Quaternion.Euler(0,0,0);

            gameObject.layer = LayerMask.NameToLayer("NoCollider");
            rb.isKinematic = false;
            rb.AddForce(0f, -0.25f, 0.5f, ForceMode.Impulse);

            stoneFX.Play();

            player.StoneTouch();


        }
    }
}