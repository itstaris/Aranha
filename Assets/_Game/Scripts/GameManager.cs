using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // variáveis de dificuldade
    public float spiderSpeed;
    public int aranhasCount;
    public int score;
    public int highScore;
    public int gameLevel;
    public int HP = 3;

    public bool iniciado = false;
    public GameObject playerGame;
    public GameObject textoCanvas;
    public GameObject textoPause;
    public bool pausado = false;

    public TextMeshProUGUI scoregame;
    public TextMeshProUGUI highScoreGame;

    public GameObject spiderSpamGame;
    public GameObject gameoverText;

    public TextMeshProUGUI levelTxt;
    public int level;

    public bool nextLevelCheck;

    IEnumerator nextLevel()
    {
        spiderSpeed -= 1f;
        nextLevelCheck = true;
        level++;
        levelTxt.text = level.ToString();
        aranhasCount = 0;
        yield return new WaitForSeconds(3f);

        nextLevelCheck = false;
        // deleta aranhas
        // spawna aranhas
        StartCoroutine(spiderSpamGame.GetComponent<SpiderSpawn>().SpawnPrefabsCoroutine());
    }

    private void Start()
    {
        level = 1;
        spiderSpeed = 8f;
        highScoreUpdate();

        // Jogo começa pausado
        Time.timeScale = 0;
        iniciado = false;

        textoCanvas.SetActive(true); // mostra "aperte Enter"
        playerGame.GetComponent<Player>().enabled = false;
        playerGame.GetComponent<BoxCollider>().enabled = false;
    }

    void highScoreUpdate()
    {
        highScore = PlayerPrefs.GetInt("HighScore");

        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        highScoreGame.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    public IEnumerator gameOver()
    {
        // Tela de gameOver
        gameoverText.SetActive(true);
        highScoreUpdate();
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(1f);

        // resetar o jogo
        Time.timeScale = 1;
        resetaJogo();
    }

    void resetaJogo()
    {
        Debug.Log("Resetou");
        SceneManager.LoadScene(0);
    }

    public void atualizaScore()
    {
        scoregame.text = score.ToString();
    }

    private void iniciaJogo()
    {
        StartCoroutine(spiderSpamGame.GetComponent<SpiderSpawn>().SpawnPrefabsCoroutine());
        iniciado = true;
        playerGame.GetComponent<Player>().enabled = true;
        playerGame.GetComponent<BoxCollider>().enabled = true;
        textoCanvas.SetActive(false);

        Time.timeScale = 1; // libera o jogo
    }

    private IEnumerator PausarCoroutine()
    {
        textoPause.SetActive(true);
        pausado = true;
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.2f);

        // espera até que aperte Enter novamente
        while (pausado)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Time.timeScale = 1;
                pausado = false;
                textoPause.SetActive(false);
                break;
            }
            yield return null; // espera próximo frame
        }
    }

    void Update()
    {
        if (nextLevelCheck == false && aranhasCount == 10)
        {
            StartCoroutine(nextLevel());
        }

        // Pressiona Enter para começar
        if (Input.GetKeyDown(KeyCode.Return) && !iniciado)
        {
            iniciaJogo();
        }
        // Pausa / Despausa durante o jogo
        else if (Input.GetKeyDown(KeyCode.Return) && iniciado && !pausado && HP > 0)
        {
            StartCoroutine(PausarCoroutine());
        }
    }
}
