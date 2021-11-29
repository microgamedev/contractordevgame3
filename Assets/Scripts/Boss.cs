using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] ParticleSystem bloodFX;
    [SerializeField] Animator enemyAnimator;

    private bool isTouch = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTouch)
        {
            isTouch = true;

            other.GetComponent<Player>().EnemyBossKill();

            Death();
        }
    }

    private void Death()
    {
        enemyAnimator.SetBool("Death", true);

        bloodFX.Play();
    }

    private IEnumerator DeathCooldown()
    {
        yield return new WaitForSeconds(0.75f);
        gameObject.SetActive(false);
    }
}