using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    [SerializeField] Animator allyAnimator;
    [SerializeField] Material allyMaterial;
    [SerializeField] SkinnedMeshRenderer allyRender;
    [SerializeField] GameObject allyObject;
    [SerializeField] GameObject katanaObject;
    [SerializeField] ParticleSystem allyBloodFX;
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public void AllyMake()
    {
        gameObject.layer = LayerMask.NameToLayer("Ally");
        allyRender.material = allyMaterial;
        allyAnimator.SetBool("Run", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ally"))
        {
            player.AllyAdd(other.gameObject);
        }

        if (other.CompareTag("Saw"))
        {
            transform.SetParent(null);
            allyObject.SetActive(false);
            katanaObject.SetActive(false);
            allyBloodFX.Play();
            Destroy(gameObject,1f);
        }
    }
}
