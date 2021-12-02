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
    private float cameraRaycast = 11f;
    private float moveLimitX = 1.5f;

    [Space]
    [SerializeField] GameManager gameManager;
    [SerializeField] CoinManager coinManager;

    [Space]
    [SerializeField] GameObject playerGFX;
    [SerializeField] TextMeshPro snakePartsCountText;

    private float snakePartDistance = 0.125f;

    private float playerSpeedZ;
    private float playerSpeedPlay = 7f;

    private float playerSpeedFinishEnter = 1f;
    private float playerSpeedFinishExit = 15f;

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
    private bool isFinishEnter = false;
    private bool isFinishExit = false;

    private void Start()
    {
        snakeParts.Add(gameObject);
        SnakePartsTextUpdate();

        playerSpeedZ = playerSpeedPlay;
    }

    private void Update()
    {
        if(!isDead || !isFinishExit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayerMoveStart();
            }

            if (Input.GetMouseButton(0))
            {
                if (!isFinishEnter)
                {
                    PlayerMove();
                }
                else if(isFinishEnter)
                {
                    PlayerMoveFinish();
                }
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


    // -------------------------------------------------
    private void PlayerMoveFinish()
    {
        touchPositionNow = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(cameraRaycast).x;
        playerTargetX += touchPositionNow - lastTouchPosition;
        playerTargetX = Mathf.Abs(playerTargetX) * 0.5f;
        playerTargetX = Mathf.Clamp(playerTargetX, 0.1f, 1.75f);
    }
    // -------------------------------------------------


    private void PlayerRun()
    {
        float tempZ = transform.position.z + (Time.fixedDeltaTime * playerSpeedZ);

        if(!isFinishEnter)
        {
            playerNowX = Mathf.SmoothDamp(playerNowX, playerTargetX, ref playerVelocityX, playerDynamicsSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);
            rb.MovePosition(new Vector3(playerNowX, 0f, tempZ));

            playerTargetRotation = Mathf.MoveTowards(playerTargetRotation, 0f, Time.fixedDeltaTime * playerTiltResetSpeed);
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

        if(isFinishEnter)
        {
            if(isFinishExit)
            {
                playerSpeedPlay *= 10f;
            }

            rb.MovePosition(new Vector3(0f, 0f, tempZ));

            if (snakeParts.Count > 1)
            {
                int _count = 1;
                Vector3 _tempPos;

                if(playerTargetX < 0.1f)
                {
                    playerTargetX = 0.1f;
                }

                float _stepNextX = playerTargetX;
                float _stepX = playerTargetX;

                float _stepZ = 0.15f;

                for (int i = 1; i < snakeParts.Count; i++)
                {
                    _tempPos = snakeParts[i - 1].transform.position;
                    _count = i;

                    if(_count % 2 == 0)
                    {
                        _tempPos.x = -_stepX;
                        _stepX += _stepNextX;

                    }
                    else
                    {
                        _tempPos.x = _stepX;
                        _tempPos.z -= _stepZ;
                    }
                    _count++;

                    snakeParts[i].transform.position = Vector3.Lerp(snakeParts[i].transform.position, _tempPos, Time.fixedDeltaTime * playerSpeedPlay);
                }
            }
        }
    }

    private void PlayerStop()
    {
        isStop = true;
        rb.velocity = Vector3.zero;
    }

    public void SnakeSegmentAdd(GameObject newSnakeSegment)
    {
        Vector3 newPartPosition = snakeParts[snakeParts.Count - 1].transform.position;
        newPartPosition.z -= snakePartDistance;
        newSnakeSegment.transform.position = newPartPosition;

        snakeParts.Add(newSnakeSegment);

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

        if (isDead || isFinishEnter)
        {
            snakePartsCountText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Finish") && !isFinishEnter)
        {
            FinishEnter();
        }

        if (other.CompareTag("FinishWall"))
        {
            FinishWall();
        }

        if (other.CompareTag("Bamboo"))
        {
            SliceBamboo(other.gameObject);
        }

        if (other.CompareTag("Lamp"))
        {
            SliceLamp(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Finish") && isFinishEnter && !isFinishExit)
        {
            FinishExit();
        }
    }

    public void SliceBamboo(GameObject _bamboo)
    {
        _bamboo.GetComponent<Bamboo>().BambooShowFX();

        coinManager.CoinsAdd(transform.position, 1);

        var sliceable = _bamboo.GetComponent<IBzSliceable>();
        Plane plane = new Plane(transform.up, (-transform.position.y + 0.1f));
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

    public void SliceLamp(GameObject _lamp)
    {
        Lamp lamp = _lamp.GetComponent<Lamp>();
        lamp.LampShowFX();
        if(lamp.isActive)
        {
            lamp.isActive = false;
        }

        coinManager.CoinsAdd(transform.position, 3);

        var sliceable = _lamp.GetComponent<IBzSliceable>();
        Plane plane = new Plane(transform.up, transform.position.y + 0.3f);
        sliceable.Slice(plane, r =>
        {
            if (!r.sliced)
            {
                return;
            }

            r.outObjectPos.gameObject.GetComponent<Lamp>().Sliced(true);
            r.outObjectNeg.gameObject.GetComponent<Lamp>().Sliced(false);
        }
        );
    }

    public void EnemyKillPlane(GameObject enemy, bool AddSegment)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

        if(AddSegment)
        {
            gameManager.SnakeSegmentAddToSnake();
        }

        if(enemy.GetComponent<EnemyRunCrowd>().isChest)
        {
            coinManager.CoinsAdd(transform.position, 100);
            enemy.GetComponent<EnemyRunCrowd>().isChest = false;
        }

        var sliceable = enemy.GetComponent<IBzSliceable>();
        Plane plane = new Plane(transform.up, (-transform.position.y + 0.05f));
        sliceable.Slice(plane, r =>
        {
            if (!r.sliced)
            {
                return;
            }

            if(r.outObjectPos.gameObject.GetComponent<EnemyStand>() != null)
            {
                r.outObjectPos.gameObject.GetComponent<EnemyStand>().EnemyDeath(true);
                r.outObjectNeg.gameObject.GetComponent<EnemyStand>().EnemyDeath(false);
            }
            else if (r.outObjectPos.gameObject.GetComponent<EnemyRun>() != null)
            {
                r.outObjectPos.gameObject.GetComponent<EnemyRun>().EnemyDeath(true);
                r.outObjectNeg.gameObject.GetComponent<EnemyRun>().EnemyDeath(false);
            }
            else if (r.outObjectPos.gameObject.GetComponent<EnemyRunCrowd>() != null)
            {
                r.outObjectPos.gameObject.GetComponent<EnemyRunCrowd>().EnemyDeath(true);
                r.outObjectNeg.gameObject.GetComponent<EnemyRunCrowd>().EnemyDeath(false);
            }
        }
        );
    }

    public void FinishEnter()
    {
        isFinishEnter = true;
        playerSpeedZ = playerSpeedFinishEnter;

        mainCamera.GetComponent<CameraFollow>().FinishGate();
        SnakePartsTextUpdate();
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }

    public void FinishExit()
    {
        isFinishExit = true;
        playerSpeedZ = playerSpeedFinishExit;

        mainCamera.GetComponent<CameraFollow>().StopFollow();
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }

    public void FinishWall()
    {
        PlayerStop();
        gameManager.GameFinish(transform.position.z);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }

    public void StoneTouch()
    {
        SnakeSegmentRemove();
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
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