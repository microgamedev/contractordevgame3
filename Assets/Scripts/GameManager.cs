using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Lofelt.NiceVibrations;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] GameObject howToPlay;
    [SerializeField] GameObject UI_Finish;

    [Space]
    [SerializeField] GameObject snakeSegmentPrefab;
    public Queue<GameObject> snakeSegmentQueue = new Queue<GameObject>();
    [SerializeField] GameObject[] levelPrefab;

    [Space]
    [SerializeField] ParticleSystem finalConfettiFX;
    [SerializeField] ParticleSystem snakeFX;
    [SerializeField] GameObject snakeFXText;

    [Space]
    [SerializeField] GameObject[] bossPrefab;

    private int _bossCount;
    [HideInInspector] public bool isBossKill = false;
    [HideInInspector] public bool isFirstTouch = false;

    private int Level;

    private void Start()
    {
        Application.targetFrameRate = 60;

        Level = PlayerPrefs.GetInt("level");
        if(Level == 0)
        {
            PlayerPrefs.SetInt("level", 1);
            Level = 1;
        }
        levelText.text = "LEVEL " + Level;

        int _levelLoad = Level - 1;
        if(_levelLoad > levelPrefab.Length - 1)
        {
            _levelLoad = Random.Range(2, levelPrefab.Length);
        }

        _levelLoad = 0;

        //Instantiate(levelPrefab[_levelLoad]);

        howToPlay.SetActive(false);
        StartCoroutine(ShowHowToPlay());

        UI_Finish.SetActive(false);

        SnakeSegmentsPrepare();
    }

    private void SnakeSegmentsPrepare()
    {
        GameObject _snake;
        for (int i = 0; i < 20; i++)
        {
            _snake = Instantiate(snakeSegmentPrefab);
            _snake.SetActive(false);
            snakeSegmentQueue.Enqueue(_snake);
        }
    }

    public void SnakeSegmentAddToSnake()
    {
        GameObject newSnakeSegment = snakeSegmentQueue.Dequeue();
        newSnakeSegment.SetActive(true);
        newSnakeSegment.GetComponent<SnakeSegment>().AddToSnake();
    }


    private IEnumerator ShowHowToPlay()
    {
        yield return new WaitForSeconds(3f);
        if(!isFirstTouch)
        {
            howToPlay.SetActive(true);
        }
    }

    public void HideHowToPlay()
    {
        howToPlay.SetActive(false);
        isFirstTouch = true;
    }

    public void ShowSnakeFX(Vector3 _position)
    {
        snakeFX.gameObject.transform.position = _position;
        snakeFX.Play();
    }

    public void ShowSnakeFXText(Vector3 _position, string _text)
    {
        Vector3 _pos = _position;
        _pos.y += 0.35f;
        _pos.z -= 0.5f;
        GameObject _snakeFXText = Instantiate(snakeFXText, _pos, Quaternion.Euler(30f, 0, 0), transform);
        _snakeFXText.GetComponent<TextMeshPro>().text = "" + _text;
        Destroy(_snakeFXText, 1f);
    }

    public void SlowMoStart()
    {
        Time.timeScale = 0.25f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        StartCoroutine(SlowMoStop());
    }

    private IEnumerator SlowMoStop()
    {
        yield return new WaitForSeconds(0.125f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    public void GameFinish(float positionZ)
    {
        if(isBossKill)
        {
            PlayerPrefs.SetInt("boss", _bossCount + 1);
        }

        PlayerPrefs.SetInt("level", Level + 1);

        finalConfettiFX.gameObject.transform.position = new Vector3(0f, 20f, positionZ - 10f);
        finalConfettiFX.Play();

        StartCoroutine(GameFinishUI());
    }

    private IEnumerator GameFinishUI()
    {
        yield return new WaitForSeconds(0.5f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);

        UI_Finish.SetActive(true);
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}