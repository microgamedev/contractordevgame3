using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("Coins")]
    [SerializeField] Animator coinsAnimator;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform coinTarget;
    [SerializeField] int maxCoins;

    private Queue<GameObject> coinsQueue = new Queue<GameObject>();

    [SerializeField] [Range(0.25f, 0.75f)] float minCoinsAnimationDuration;
    [SerializeField] [Range(0.75f, 1.5f)] float maxCoinsAnimationDuration;
    [SerializeField] Ease easeType;
    private Vector3 coinsTargetPosition;

    private int _coins = 0;
    public int Coins
    {
        get { return _coins; }
        set
        {
            _coins = value;
            coinsText.text = "" + Coins;
            PlayerPrefs.SetInt("coins", Coins);
        }
    }

    private void Start()
    {
        Coins = PlayerPrefs.GetInt("coins");
        PrepareCoins();
    }

    private void PrepareCoins()
    {
        GameObject coin;
        for (int i = 0; i < maxCoins; i++)
        {
            coin = Instantiate(coinPrefab);
            coin.transform.SetParent(coinTarget.transform, true);
            coin.SetActive(false);
            coinsQueue.Enqueue(coin);
        }
    }

    public void CoinsAdd(Vector3 collectedCoinPosition, int amount)
    {
        CoinsAnimate(collectedCoinPosition, amount);
    }

    private void CoinsAnimate(Vector3 collectedCoinPosition, int amount)
    {
        coinsTargetPosition = Camera.main.transform.InverseTransformPoint(coinTarget.position);
        //collectedCoinPosition.z -= 0.5f;

        for (int i = 0; i < amount; i++)
        {
            if (coinsQueue.Count > 0)
            {
                GameObject coin = coinsQueue.Dequeue();
                coin.SetActive(true);

                coin.transform.position = collectedCoinPosition;

                float duration = Random.Range(minCoinsAnimationDuration, maxCoinsAnimationDuration);
                coin.transform.DOLocalMove(coinsTargetPosition, duration)
                    .SetEase(easeType)
                    .OnComplete(() =>
                    {
                        coin.SetActive(false);
                        coinsQueue.Enqueue(coin);
                        Coins++;
                        coinsAnimator.SetTrigger("Bounce");
                    });
            }
        }
    }
}
