using UnityEngine;
using DG.Tweening;
using Lofelt.NiceVibrations;

public class Lamp : MonoBehaviour
{
    private ParticleSystem fx;
    private Rigidbody rb;
    public bool isActive = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        fx = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        if (isActive)
        {
            LampActive();
        }
    }

    private void LampActive()
    {
        transform.DOMoveY(0.75f, Random.Range(1f, 2.5f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void LampShowFX()
    {
        fx.Play();
    }

    public void Sliced(bool _bounce)
    {
        Destroy(gameObject, 3f);
        DOTween.Kill(transform);
        gameObject.layer = LayerMask.NameToLayer("NoCollider");

        if (_bounce)
        {
            rb.isKinematic = false;
            rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
            //rb.AddForce(new Vector3(Random.Range(0, 0.25f), 1f, 1f) * 0.75f, ForceMode.Impulse);
            rb.AddForce(new Vector3(Random.Range(0, 0.25f), 1f, Random.Range(0, 0.25f)) * 0.35f, ForceMode.Impulse);

            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        }
    }
}