using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BzKovSoft.ObjectSlicer;
using Lofelt.NiceVibrations;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera mainCamera;
    private float cameraRaycast = 10f;
    private float moveLimitX = 1.75f;

    [Space]
    [SerializeField] GameManager gameManager;
    [SerializeField] CoinManager coinManager;

    [Space]
    [SerializeField] GameObject playerGFX;
    [SerializeField] TextMeshPro snakePartsCountText;

    private float snakePartDistance = 0.125f;

    private float playerSpeedZ = 6f;
    private float playerSpeedFast = 11f;
    private float playerTiltAngle = 30f;
    private float playerDynamicsSmoothTime = 0.05f;
    private float playerTiltPower = 2.0f;
    private float playerTiltSensetivity = 10.0f;
    private float playerTiltResetSpeed = 3.0f;

    public List<GameObject> snakeParts = new List<GameObject>();

    private float touchPositionStart, touchPositionNow, lastTouchPosition;
    private float playerTargetX, playerNowX, playerVelocityX;
    private float  playerTargetRotation, playerRotationVelocity, playerNowRotation;
    private bool isStart = false;
    private bool isDead = false;
    private bool isStop = false;
    private bool isFinish = false;

    private int bambooCount;

    private void Start()
    {
        snakeParts.Add(gameObject);
        SnakePartsTextUpdate();

        bambooCount = 0;
    }

    private void Update()
    {
        if(!isDead || !isFinish)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayerMoveStart();
            }

            if (Input.GetMouseButton(0))
            {
                PlayerMove();
            }
        }
    }

    private void FixedUpdate()
    {
        if(isStart && !isStop)
        {
            PlayerRun();
        }
    }

    private void PlayerMoveStart()
    {
        if (!isStart)
        {
            gameManager.HideHowToPlay();
            isStart = true;
        }
        touchPositionStart = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(cameraRaycast).x;

        lastTouchPosition = touchPositionStart;
    }

    private void PlayerMove()
    {
        touchPositionNow = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(cameraRaycast).x;
        playerTargetX += (touchPositionNow - lastTouchPosition);
        playerTargetX = Mathf.Clamp(playerTargetX, -moveLimitX, moveLimitX);

        playerTargetRotation += (touchPositionNow - lastTouchPosition) * playerTiltSensetivity; 
        playerTargetRotation = Mathf.Clamp(playerTargetRotation, -1f, 1f);
        lastTouchPosition = touchPositionNow;
    }

    private void PlayerRun()
    {
        float tempZ = transform.position.z + (Time.fixedDeltaTime * playerSpeedZ);

        if (isFinish)
        {
            playerTargetX = 0f;
        }

        // For Test Only
        // --------------
        //tempZ = 0f;
        // --------------

        playerNowX = Mathf.SmoothDamp(playerNowX, playerTargetX, ref playerVelocityX, playerDynamicsSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        rb.MovePosition(new Vector3(playerNowX, 0f, tempZ));

        playerTargetRotation = Mathf.MoveTowards(playerTargetRotation, 0f,  Time.fixedDeltaTime * playerTiltResetSpeed);
        playerNowRotation = Mathf.SmoothDamp(playerNowRotation, playerTargetRotation, ref playerRotationVelocity, playerDynamicsSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);

        float playerRotation = Mathf.Pow(Mathf.Abs(playerNowRotation), playerTiltPower) * Mathf.Sign(playerNowRotation);

        rb.rotation = Quaternion.Euler(0f, 0f, playerRotation * playerTiltAngle);

        if (snakeParts.Count > 1)
        {
            for (int i = 1; i < snakeParts.Count; i++)
            {
                Vector3 _tempPos = snakeParts[i - 1].transform.position;
                _tempPos.z -= snakePartDistance;
                snakeParts[i].transform.position = Vector3.Lerp(snakeParts[i].transform.position, _tempPos, Time.fixedDeltaTime * 10f);
                snakeParts[i].transform.rotation = Quaternion.Lerp(snakeParts[i].transform.rotation, snakeParts[i - 1].transform.rotation, Time.fixedDeltaTime * 20f);
            }
        }
    }

    private void PlayerStop()
    {
        isStop = true;
        rb.velocity = Vector3.zero;
    }

    public void SnakeSegmentAdd(GameObject newPart)
    {
        Vector3 newPartPosition = snakeParts[snakeParts.Count - 1].transform.position;
        newPartPosition.z -= snakePartDistance;
        newPart.transform.position = newPartPosition;

        snakeParts.Add(newPart);

        ShowSnakeFX();
        ShowSnakeFXText("+1");
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);

        SnakePartsTextUpdate();
    }

    public void SnakeSegmentRemove()
    {
        if(snakeParts.Count > 1)
        {
            ShowSnakeFXText("-1");
            GameObject _part = snakeParts[snakeParts.Count - 1];
            snakeParts.Remove(_part);
            Destroy(_part);
        }
        else
        {
            PlayerDead();
        }

        SnakePartsTextUpdate();
    }

    private void SnakePartsTextUpdate()
    {
        snakePartsCountText.text = "" + snakeParts.Count;

        if (isDead)
        {
            snakePartsCountText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Finish") && !isFinish)
        {
            playerSpeedZ = playerSpeedFast;
            isFinish = true;
        }

        if (other.CompareTag("Bamboo"))
        {
            other.GetComponent<Bamboo>().BambooShowFX();

            bambooCount++;

            if(bambooCount >= 3)
            {
                coinManager.CoinsAdd(transform.position, 1);
                bambooCount = 0;
            }

            var sliceable = other.GetComponent<IBzSliceable>();
            Plane plane = new Plane(transform.up, (-transform.position.y + 0.05f));
            sliceable.Slice(plane, r =>
            {
                if (!r.sliced)
                {
                    return;
                }

                r.outObjectPos.gameObject.GetComponent<Bamboo>().Sliced(true);
                r.outObjectNeg.gameObject.GetComponent<Bamboo>().Sliced(false);
            }
            );
        }
    }

    public void EnemyRunKill(GameObject enemy)
    {
        coinManager.CoinsAdd(transform.position, 5);

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

        StartCoroutine(EnemyRunKillSlice(enemy));
    }

    private IEnumerator EnemyRunKillSlice(GameObject enemy)
    {
        yield return new WaitForSeconds(0.05f);

        var sliceable = enemy.GetComponent<IBzSliceable>();
        Plane plane = new Plane(transform.up, (-transform.position.y + 0.05f));
        sliceable.Slice(plane, r =>
        {
            if (!r.sliced)
            {
                return;
            }

            r.outObjectPos.gameObject.GetComponent<EnemyRun>().DeadStart(true);
            r.outObjectNeg.gameObject.GetComponent<EnemyRun>().DeadStart(false);
        }
        );
    }

    public void EnemyStandKill(GameObject enemy)
    {
        SnakeSegmentRemove();
        coinManager.CoinsAdd(transform.position, 5);

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

        var sliceable = enemy.GetComponent<IBzSliceable>();
        Plane plane = new Plane(transform.up, (-transform.position.y + 0.05f));
        sliceable.Slice(plane, r =>
        {
            if (!r.sliced)
            {
                return;
            }

            r.outObjectPos.gameObject.GetComponent<EnemyStand>().DeadStart(true);
            r.outObjectNeg.gameObject.GetComponent<EnemyStand>().DeadStart(false);
        }
        );
    }

    public void EnemyBossKill()
    {
        PlayerStop();

        gameManager.isBossKill = true;
        gameManager.SlowMoStart();
        SnakeSegmentRemove();
        coinManager.CoinsAdd(transform.position, 20);

        gameManager.GameFinish(transform.position.z);

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
    }

    public void StoneTouch()
    {
        SnakeSegmentRemove();
        gameManager.SlowMoStart();
    }

    public void ShowSnakeFX()
    {
        gameManager.ShowSnakeFX(transform.position);
    }

    public void ShowSnakeFXText(string _text)
    {
        gameManager.ShowSnakeFXText(transform.position, _text);
    }

    private void PlayerDead()
    {
        isDead = true;

        PlayerStop();

        playerGFX.SetActive(false);

        SnakePartsTextUpdate();

        if (!gameManager.isBossKill)
        {
            gameManager.GameFinish(transform.position.z);
        }
    }
}