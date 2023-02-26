using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeMove
{
    private Transform _transform;
    private float _zMoveSpeed;
    private float _xMoveSpeed;

    public Transform Trans => _transform;
    private Vector3 _startPos;
    private float _startTime;
    private bool _isGameRunning;
    
    public CubeMove(Transform transform)
    {
        _transform = transform;
        GameManager.Instance.Updater.Add(Update);
        _zMoveSpeed = 5f;
        _xMoveSpeed= 5f;
       
        _startPos= _transform.position;
        StartGame();
        InitEvent();
    }
    private void StartGame()
    {
        _isGameRunning = true;
        _transform.position = _startPos;
        _startTime = Time.realtimeSinceStartup;


    }

    private void InitEvent()
    {
       
        //EventCenter.Instance.Test.AddListener(HitCallback);
        EventCenter.Instance.OnCollisionEnter.AddListener(HitCallback);
        EventCenter.Instance.GameReStart.AddListener(GameReStartCallback);

    }
    private void HitCallback(GameObject gameObject,Collision collision)
    {
        _isGameRunning = false;
        //Debug.Log($"Hit----- ，obj:{gameObject.name}, coll:{collision.gameObject.name}");
        GameManager.Instance.GameOverUI.SetActive(true);
        var now = Time.realtimeSinceStartup;
        var lifeTime = now- _startTime;
        string liftTimeText = $"存活时间:{(int)lifeTime}s";
        GameManager.Instance.GameOverUI.RefreshLifeTimeText(liftTimeText);
    }
    private void GameReStartCallback()
    {
        GameManager.Instance.GameOverUI.SetActive(false);
       StartGame();
    }
    private void Update()
    {
        if(_isGameRunning)
        {
            MoveX();
            MoveZ();
            UpdateMainUI();
        }
    }

    private void MoveZ()
    {
        var oldPos = _transform.position;
        oldPos.z += _zMoveSpeed * Time.deltaTime;
        _transform.position = oldPos;
    }
    private void MoveX()
    {
       

        var dir = Vector3.zero;
        if(Input.GetKey(KeyCode.A))
        {
            dir = new Vector3(-1, 0,0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir = new Vector3(1, 0,0);
        }

        var deltaPos =dir * _xMoveSpeed * Time.deltaTime;
        var oldPos = _transform.position;
        oldPos += deltaPos;
        _transform.position = oldPos;
    }
    
    private void UpdateMainUI()
    {
        string speedText = $"当前速度：{_zMoveSpeed}m/s";
        GameManager.Instance.MainUI.RefreshSpeedText(speedText);
        var nowPos = -_transform.position;
        var distance = Vector3.Distance(nowPos, -_startPos);
        string distanceText = $"通过距离:{(int)distance}m";
        GameManager.Instance.MainUI.RefreshDistanceText(distanceText);
        var nowTime = Time.realtimeSinceStartup;
        float lifeTime = nowTime - _startTime;
        string lifeTimeText = $"存活时间:{(int)lifeTime}s";
        GameManager.Instance.MainUI.RefreshLiftTimeText(lifeTimeText);
    }
}


