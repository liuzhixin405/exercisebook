using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


/// <summary>
/// 游戏管理
/// </summary>

public class GameManager
{
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    private static GameManager _instance;
    private Dictionary<string, GameObject> _dict = new();
    public Dictionary<string, GameObject> Dict => _dict;
    public Updater Updater;
    public Updater LateUpdater;
    public CameraControl _cameraControl;
    public CubeMove _cubeMove;
    public CubeMove CubeMove=>_cubeMove;
    private GameOverUI _gameOverUI;
    public GameOverUI GameOverUI => _gameOverUI;
    private MainUI _mainUI;
    public MainUI MainUI => _mainUI;

    private OtherCubeManager _otherCubeManager;
    public OtherCubeManager OtherCubeManager => _otherCubeManager;
    public GameManager()
    {
        Updater = new Updater();
        LateUpdater = new Updater();
        var activeScene = SceneManager.GetActiveScene();
        var rootGos = activeScene.GetRootGameObjects();
        foreach (var rootGo in rootGos)
        {
            _dict.Add(rootGo.name, rootGo);
            Debug.Log($"rootGo:{rootGo.name}");
        }
    }
    public void Start()
    {
        _cubeMove = new CubeMove(_dict["CubeMove"].transform);
        _cameraControl = new CameraControl(_dict["Main Camera"].transform);
        _otherCubeManager = new OtherCubeManager(_dict["OtherCubes"].transform);
        InitUI();
    }

    private void InitUI()
    {
        var uiCanvas = _dict["UICanvas"].transform;
        _gameOverUI = new GameOverUI(uiCanvas.Find("UI_Gameover"));
        _mainUI = new MainUI(uiCanvas.Find("UI_Main"));
    }
}
