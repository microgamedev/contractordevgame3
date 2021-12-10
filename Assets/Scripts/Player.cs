using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BzKovSoft.ObjectSlicer;
using Lofelt.NiceVibrations;
using TMPro;
using Dreamteck.Splines;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera mainCamera;
    private float cameraRaycast = 12f;
    private float moveLimitX = 1.5f;

    [Space]
    [SerializeField] GameManager gameManager;
    [SerializeField] CoinManager coinManager;

    [Space]
    [SerializeField] GameObject playerGFX;
    [SerializeField] TextMeshPro snakePartsCountText;

    private SplineComputer spline;
    [SerializeField] float splineYOffset = 2f; //Сдвиг игрока по Y относительно кривой
    [SerializeField] float inputSensetivity = 1f;

    private float snakePartDistance = 0.125f;

    private float playerSpeedZ;
    public float playerSpeedPlay = 7.5f;

    private float playerSpeedFinishEnter = 3f;
    private float playerSpeedFinishExit = 12f;

    private float playerTiltAngle = 30f;
    private float playerDynamicsSmoothTime = 0.06f;
    private float playerTiltPower = 2.0f;
    private float playerTiltSensetivity = 10.0f;
    private float playerTiltResetSpeed = 3.0f;

    public List<GameObject> snakeParts = new List<GameObject>();

    private float touchPositionStart, touchPositionNow, lastTouchPosition;
    private float playerTargetX, playerNowX, playerVelocityX;
    private float playerTargetRotation, playerRotationVelocity, playerNowRotation;
    private bool isStart = false;
    private bool isDead = false;
    private bool isStop = false;
    private bool isFinishEnter = false;
    private bool isFinishExit = false;

    float movementT = 0; //Позиция на кривой
    float splineLength = 0; //Сохраненная длина кривой

    public float InputX { get; set; }
    public SplineSample SplineSample { get; private set; }
    public float SplineYOffset => splineYOffset;

    private bool isEvenSnakeSegmentsCount = false;

    private void Start()
    {
        snakeParts.Add(gameObject);
        SnakePartsTextUpdate();

        playerSpeedZ = playerSpeedPlay;

        spline = gameManager.splineComputer;
        //Считаем длину кривой и сохраняем ее в переменную, операция дорогая поэтому делаем ее на старте один раз
        //это нужно будет пересчитывать если кривая будет меняться
        splineLength = spline.CalculateLength();

        transform.position = new Vector3(spline.GetPointPosition(0).x, spline.GetPointPosition(0).y + 2f, spline.GetPointPosition(0).z);
        SplineSample = spline.Evaluate(0f);
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
        touchPositionNow = GetTouchPosition().x;

        lastTouchPosition = touchPositionStart;
    }

    private void PlayerMove()
    {
        //touchPositionNow = GetTouchPosition().x;
        float input = InputX * inputSensetivity;
        playerTargetX += input;
        playerTargetX = Mathf.Clamp(playerTargetX, -moveLimitX, moveLimitX);

        playerTargetRotation += input * playerTiltSensetivity; 
        playerTargetRotation = Mathf.Clamp(playerTargetRotation, -1f, 1f);
        lastTouchPosition = touchPositionNow;
    }


    // -------------------------------------------------
    private void PlayerMoveFinish()
    {
        touchPositionNow = GetTouchPosition().x;
        playerTargetX += touchPositionNow - lastTouchPosition;
        playerTargetX = Mathf.Abs(playerTargetX) * 0.5f;
        playerTargetX = Mathf.Clamp(playerTargetX, 0.1f, 1.75f);
    }
    // -------------------------------------------------

    //Получаем позицию на луче в локальном пространстве
    Vector3 GetTouchPosition() 
    {
        var worldPoint = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(cameraRaycast);
        var localPoint = mainCamera.transform.InverseTransformPoint(worldPoint);
        return localPoint;
    }


    private void PlayerRun()
    {
        float tempZ = transform.position.z + (Time.fixedDeltaTime * playerSpeedZ);
        movementT += Time.fixedDeltaTime * playerSpeedZ / splineLength; //Обновляем позицию t на сплайне
        movementT = Mathf.Clamp01(movementT); //Ограничеваем значение t от 0 до 1, иначе при взятии позиции со сплайна будет ошибка
        var sample = spline.Evaluate(movementT); //Берем мировую позицию на сплайне по t
        SplineSample = sample;
        if (!isFinishEnter)
        {
            playerNowX = Mathf.SmoothDamp(playerNowX, playerTargetX, ref playerVelocityX, playerDynamicsSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);
            var position = sample.position + Vector3.up * splineYOffset; // складываем позицию на сплайне и сдвиг по Y
            position += sample.rotation * (Vector3.right * playerNowX); //сдвигаем позицию направо относительно вращения кривой
            rb.MovePosition(position); //назначаем позицию телу

            playerTargetRotation = Mathf.MoveTowards(playerTargetRotation, 0f, Time.fixedDeltaTime * playerTiltResetSpeed);
            playerNowRotation = Mathf.SmoothDamp(playerNowRotation, playerTargetRotation, ref playerRotationVelocity, playerDynamicsSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);

            float playerRotation = Mathf.Pow(Mathf.Abs(playerNowRotation), playerTiltPower) * Mathf.Sign(playerNowRotation);

            rb.rotation = sample.rotation * Quaternion.Euler(0f, 0f, playerRotation * playerTiltAngle);

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

                if(playerTargetX < 0.125f)
                {
                    playerTargetX = 0.125f;
                }

                float _stepNextX = playerTargetX;
                float _stepX = playerTargetX;

                float _stepZ = 0.05f;

                for (int i = 1; i < snakeParts.Count; i++)
                {
                     Vector3 _tempPos = snakeParts[i - 1].transform.position;
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
            //PlayerDead();
        }

        SnakePartsTextUpdate();
    }

    private void SnakePartsTextUpdate()
    {
        snakePartsCountText.text = "" + snakeParts.Count;

        if (isDead || isFinishEnter || isEvenSnakeSegmentsCount)
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

        if (other.CompareTag("CheckSnake") && !isEvenSnakeSegmentsCount)
        {
            if (snakeParts.Count % 2 == 0)
            {
                gameManager.SnakeSegmentAddToSnake();
                isEvenSnakeSegmentsCount = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Finish") && isFinishEnter && !isFinishExit)
        {
            FinishExit();
        }
    }

    Plane GetSlicePlane()
    {
        return new Plane(transform.up, transform.position);
    }

    public void SliceBamboo(GameObject _bamboo)
    {
        _bamboo.GetComponent<Bamboo>().BambooShowFX();

        coinManager.CoinsAdd(transform.position, 1);

        var sliceable = _bamboo.GetComponent<IBzSliceable>();
        Plane plane = GetSlicePlane();
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

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

        if (lamp.isActive)
        {
            lamp.isActive = false;
        }

        coinManager.CoinsAdd(transform.position, 3);

        var sliceable = _lamp.GetComponent<IBzSliceable>();
        Plane plane = GetSlicePlane();
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

    public void EnemyKillPlane(GameObject enemy, bool AddSegment, int AddCoins)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

        if(AddSegment)
        {
            gameManager.SnakeSegmentAddToSnake();
        }

        if(AddCoins > 0)
        {
            coinManager.CoinsAdd(transform.position, AddCoins);
        }


        if (enemy.GetComponent<EnemyRunCrowd>() != null)
        {
            if (enemy.GetComponent<EnemyRunCrowd>().isChest)
            {
                enemy.GetComponent<EnemyRunCrowd>().isChest = false;
            }
        }

        var sliceable = enemy.GetComponent<IBzSliceable>();
        Plane plane = GetSlicePlane();
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
            else if (r.outObjectPos.gameObject.GetComponent<EnemyBoss>() != null)
            {
                r.outObjectPos.gameObject.GetComponent<EnemyBoss>().EnemyDeath(true);
                r.outObjectNeg.gameObject.GetComponent<EnemyBoss>().EnemyDeath(false);
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
        for(int i = 0; i < snakeParts.Count; i++)
        {
            snakeParts[i].GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        gameManager.GameFinish(transform.position.z);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
    }

    public void StoneTouch()
    {
        SnakeSegmentRemove();
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        gameManager.SlowMoStart();
    }

    public void WoodTouch()
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