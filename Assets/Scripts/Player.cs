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

    [Space]
    [SerializeField] GameManager gameManager;
    [SerializeField] CoinManager coinManager;

    [Space]
    [SerializeField] GameObject snakePartPrefab;
    [SerializeField] GameObject playerGFX;
    [SerializeField] TextMeshPro snakePartsCountText;

    private float cameraRaycast = 15f;
    private float moveLimitX = 1.75f;
    private float snakePartDistance = 0.1f;
    private float playerSpeedZ = 5f;
    private float playerSpeedFast = 11f;

    private float playerTiltSpeed = 35f;
    private float playerTiltAngle = 30f;
    public List<GameObject> snakeParts = new List<GameObject>();

    private float touchPositionStart, touchPositionNow, playerOriginalX, playerNowX, playerNowRotation;

    private bool isTouch = false;
    private bool isPreFinish = false;
    private bool isTilt = false;

    private int bambooCount;

    private void Start()
    {
        snakeParts.Add(gameObject);
        SnakePartsTextUpdate();

        bambooCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPreFinish)
        {
            PlayerMoveStart();
        }

        if (Input.GetMouseButton(0) && !isPreFinish)
        {
            PlayerMove();
        }
    }

    private void FixedUpdate()
    {
        if(isTouch)
        {
            PlayerRun();
        }
    }

    private void PlayerMoveStart()
    {
        if (!isTouch)
        {
            gameManager.HideHowToPlay();
            isTouch = true;
        }
        touchPositionStart = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(cameraRaycast).x;
        playerOriginalX = transform.position.x;
    }

    private void PlayerMove()
    {
        touchPositionNow = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(cameraRaycast).x;
        playerNowX = playerOriginalX + (touchPositionNow - touchPositionStart);
        playerNowX = Mathf.Clamp(playerNowX, -moveLimitX, moveLimitX);

        playerNowRotation += (touchPositionNow - touchPositionStart);
        playerNowRotation = Mathf.Clamp(playerNowRotation, -playerTiltAngle, playerTiltAngle);
    }

    private void PlayerRun()
    {
        float tempZ = transform.position.z + (Time.fixedDeltaTime * playerSpeedZ);

        if (isPreFinish)
        {
            playerNowX = 0f;
        }
        tempZ = 0f;
        rb.MovePosition(new Vector3(playerNowX, 0f, tempZ));

        if(playerNowRotation < 0f)
        {
            playerNowRotation += Time.fixedDeltaTime * playerTiltSpeed;
        }
        else if (playerNowRotation > 0f)
        {
            playerNowRotation -= Time.fixedDeltaTime * playerTiltSpeed;
        }

        rb.rotation = Quaternion.Euler(0f, 0f, playerNowRotation);

        if (snakeParts.Count > 1)
        {
            for (int i = 1; i < snakeParts.Count; i++)
            {
                Vector3 _tempPos = snakeParts[i - 1].transform.position;
                _tempPos.z -= snakePartDistance;
                snakeParts[i].transform.position = Vector3.Lerp(snakeParts[i].transform.position, _tempPos, Time.fixedDeltaTime * 20f);
                snakeParts[i].transform.rotation = Quaternion.Lerp(snakeParts[i].transform.rotation, snakeParts[i - 1].transform.rotation, Time.fixedDeltaTime * 20f);
            }
        }
    }

    private void PlayerStop()
    {
        isTouch = false;
        rb.velocity = Vector3.zero;
    }

    public void AddSnakePart(GameObject _newPart, int count)
    {
        GameObject newPart;

        for (int i = 0; i < count; i++)
        {
            if (_newPart == null)
            {
                newPart = Instantiate(snakePartPrefab);
            }
            else
            {
                newPart = _newPart;
            }

            newPart.GetComponent<SnakePart>().AddToSnake();

            Vector3 newPartPosition = snakeParts[snakeParts.Count - 1].transform.position;
            newPartPosition.z -= snakePartDistance;
            newPart.transform.position = newPartPosition;

            snakeParts.Add(newPart);

            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        }

        SnakePartsTextUpdate();
    }

    public void RemoveSnakePart(int count)
    {
        if(!isPreFinish)
        {
            if(count >= snakeParts.Count)
            {
                count = snakeParts.Count - 1;
            }

            if(count > 0)
            {
                ShowSnakeFXText("-" + count);

                for (int i = 0; i < count; i++)
                {
                    GameObject _part = snakeParts[snakeParts.Count - 1];
                    snakeParts.Remove(_part);
                    Destroy(_part);
                }
            }
        }

        if(isPreFinish)
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
                PlayerDeath();
            }
        }

        SnakePartsTextUpdate();
    }

    private void SnakePartsTextUpdate()
    {
        snakePartsCountText.text = "" + snakeParts.Count;

        if (isPreFinish && snakeParts.Count == 1)
        {
            snakePartsCountText.text = "";
        }
    }

    public void NewSnakePart(GameObject _part)
    {
        ShowSnakeFX();
        ShowSnakeFXText("+1");
        AddSnakePart(_part, 1);
    }

    public void AllyAdd(GameObject _ally)
    {
        _ally.GetComponent<Ally>().AllyMake();
        _ally.transform.SetParent(transform, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SnakePart"))
        {
            NewSnakePart(other.gameObject);
        }

        if (other.CompareTag("Ally"))
        {
            AllyAdd(other.gameObject);
            
        }

        if(other.CompareTag("PreFinish") && !isPreFinish)
        {
            playerSpeedZ = playerSpeedFast;
            isPreFinish = true;
            isTouch = true;
        }

        if (other.CompareTag("Bamboo"))
        {
            if(!other.GetComponent<Bamboo>().isSliced)
            {
                other.GetComponentInChildren<ParticleSystem>().Play();

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
    }

    public void EnemyRunKill(GameObject enemy)
    {
        //gameManager.SlowMoStart();
        coinManager.CoinsAdd(transform.position, 5);

        NewSnakePart(null);

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
        RemoveSnakePart(1);
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
        RemoveSnakePart(1);
        coinManager.CoinsAdd(transform.position, 20);

        gameManager.GameFinish(transform.position.z);

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
    }

    public void StoneTouch()
    {
        RemoveSnakePart(1);
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

    private void PlayerDeath()
    {
        PlayerStop();

        playerGFX.SetActive(false);

        SnakePartsTextUpdate();

        if (!gameManager.isBossKill)
        {
            gameManager.GameFinish(transform.position.z);
        }
    }
}