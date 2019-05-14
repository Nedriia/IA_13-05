using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public RectTransform p1;
    public RectTransform p2;
    public RectTransform gameOver;
    public Text countdown;

    private Slider slider1;
    private Slider slider2;
    private Text score1;
    private Text score2;

    void Awake()
    {
        slider1 = p1.GetComponentInChildren<Slider>();
        slider2 = p2.GetComponentInChildren<Slider>();
        score1 = p1.GetComponentInChildren<Text>();
        score2 = p2.GetComponentInChildren<Text>();
        gameOver.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GameData gameData = GameManager.Instance.GetGameData();
        int p1Score = GameManager.Instance.GetScoreForPlayer(0);
        int p2Score = GameManager.Instance.GetScoreForPlayer(1);
        score1.text = p1Score.ToString();
        score2.text = p2Score.ToString();
        slider1.value = gameData.GetSpaceShipForOwner(0).Energy;
        slider2.value = gameData.GetSpaceShipForOwner(1).Energy;
        countdown.text = ((int)gameData.timeLeft).ToString();
        if (!gameOver.gameObject.activeSelf && GameManager.Instance.IsGameFinished())
        {
            gameOver.gameObject.SetActive(true);
        }
    }
}
