using UnityEngine;
using Lofelt.NiceVibrations;

public class Lamp : MonoBehaviour
{
    private ParticleSystem fx;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        fx = GetComponentInChildren<ParticleSystem>();
    }

    public void LampShowFX()
    {
        fx.Play();
    }

    public void Sliced(bool _bounce)
    {
        Destroy(gameObject, 3f);

        if (_bounce)
        {
            gameObject.layer = LayerMask.NameToLayer("NoCollider");
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
            rb.AddForce(new Vector3(Random.Range(0, 0.25f), 1f, Random.Range(0, 0.25f)) * 0.35f, ForceMode.Impulse);
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        }
    }
}