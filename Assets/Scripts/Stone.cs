using UnityEngine;
using DG.Tweening;

public class Stone : MonoBehaviour
{
    private bool isTouch = false;
    [SerializeField] ParticleSystem stoneFX;
    [SerializeField] GameObject shuriken;
    private Player player;

    private float moveLimit = 1f;
    private float startPosition;
    private float finishPosition;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        float gatePosition = transform.position.x;
        switch (gatePosition)
        {
            case -1:
                startPosition = -moveLimit;
                finishPosition = moveLimit;
                break;
            case 0:
                startPosition = 0f;
                finishPosition = moveLimit;
                break;
            case 1:
                startPosition = moveLimit;
                finishPosition = -moveLimit;
                break;
        }

        transform.position = new Vector3(startPosition, 0, transform.position.z);

        if (startPosition != 0)
        {
            transform.DOMoveX(finishPosition, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !isTouch)
        {
            isTouch = true;

            GetComponent<BoxCollider>().isTrigger = true;

            stoneFX.Play();

            player.StoneTouch();

            GameObject _shuriken = Instantiate(shuriken);
            _shuriken.transform.SetParent(transform, false);

            Vector3 _pos = new Vector3(0, other.GetContact(0).point.y, 0);
            _shuriken.transform.position = other.GetContact(0).point;
        }
    }
}