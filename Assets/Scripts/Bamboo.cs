using UnityEngine;
using Lofelt.NiceVibrations;

public class Bamboo : MonoBehaviour
{
    [HideInInspector] public bool isSliced = false;

    public void Sliced(bool _bounce)
    {
        isSliced = true;

        Destroy(gameObject, 4f);

        if(_bounce)
        {
            gameObject.layer = LayerMask.NameToLayer("BambooSliced");

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<MeshCollider>().isTrigger = false;
            GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f)) * 0.5f, ForceMode.Impulse);
            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 0.25f), 1f, Random.Range(0, 0.25f)) * 0.3f, ForceMode.Impulse);
        }

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }
}