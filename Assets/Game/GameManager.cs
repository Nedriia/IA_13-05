using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public List<SpaceShip> SpaceShips = new List<SpaceShip>();
    public List<WayPoint> WayPoints = new List<WayPoint>();
    public List<Asteroid> Asteroids = new List<Asteroid>();
    public List<Mine> Mines = new List<Mine>();
    public List<Bullet> Bullets = new List<Bullet>();

    public float timeLeft = 0.0f;

    public SpaceShip GetSpaceShipForOwner(int owner)
    {
        return SpaceShips.Find(x => x.Owner == owner);
    }
}

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        RUNNING,
        ENDED,
    }

    [System.Serializable]
    public struct PlayerSetup
    {
        public SpaceShip spaceShip;
        public BaseSpaceShipController controller;
    }

    public static GameManager Instance = null;

    public List<PlayerSetup> playerSetups;
    public GameObject mapRoot;
    public float gameDuration = 60.0f;
    public float chrono = 0.0f;

    private GameState _gameState = GameState.RUNNING;

    private GameData _gameData = new GameData();

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        int i = 0;
        _gameData.timeLeft = gameDuration;
        foreach (PlayerSetup setup in playerSetups)
        {
            if (setup.spaceShip != null)
            {
                setup.spaceShip.Initialize(setup.controller, i);
            }
            ++i;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameState == GameState.RUNNING)
        {
            _gameData.timeLeft -= Time.deltaTime;            
            if (_gameData.timeLeft <= 0.0f)
            {
                _gameData.timeLeft = 0.0f;
                _gameState = GameState.ENDED;
                foreach (PlayerSetup setup in playerSetups)
                {
                    if(setup.controller != null) {
                        setup.controller.enabled = false;
                    }
                }
                Time.timeScale = 0.0f;
            }
        }
    }

    public GameData GetGameData()
    {
        return _gameData;
    }

    public int GetScoreForPlayer(int id)
    {
        int score = 0;
        foreach(WayPoint wayPoint in _gameData.WayPoints)
        {
            if(wayPoint.Owner == id)
            {
                score++;
            }
        }
        foreach (PlayerSetup setup in playerSetups)
        {
            if (setup.spaceShip == null || setup.spaceShip.Owner == id)
                continue;
            score += setup.spaceShip.HitCount;
        }

        return score;
    }

    public bool IsGameFinished()
    {
        return _gameState == GameState.ENDED;
    }
}
