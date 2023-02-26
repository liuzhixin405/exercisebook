using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class OtherCubeManager
{
    private Transform _trans;
    private GameObject _otherCubeGo;
    private List<GameObject> _unUsedotherCubes = new();
    private List<GameObject> _usedotherCubes = new();
    private Vector3 _lastPosition;
    public OtherCubeManager(Transform transform)
    {
        _trans = transform;
        _otherCubeGo = Resources.Load<GameObject>("OtherCube");
        //for (int i = 0; i < 100; i++)
        //{
        //	var cubeGo = GameObject.Instantiate(_otherCubeGo);
        //	_otherCubes.Add(cubeGo);
        //	float x = UnityEngine.Random.Range(-5.0f,5.0f);
        //	float z = -0.16f + i * 3.0f;
        //	cubeGo.transform.position = new Vector3(x,0,z);
        //}

        for (int i = 0; i < 10; i++)
        {
            var cubeGo = GameObject.Instantiate(_otherCubeGo);
            cubeGo.transform.SetParent(_trans, false);
            _unUsedotherCubes.Add(cubeGo);

        }
        GameManager.Instance.Updater.Add(Update);
    }

    private void Update()
    {
        if (GameManager.Instance.CubeMove != null)
        {
            //回收已经在屏幕外的红色方块
            var playerPos = GameManager.Instance.CubeMove.Trans.position;
            for(int i = _usedotherCubes.Count - 1; i >= 0; --i)
            {
                var cubeGo = _usedotherCubes[i];
                var pos = cubeGo.transform.position;
                if(playerPos.z-pos.z > 5.0f)
                {
                    _unUsedotherCubes.Add(cubeGo);
                    cubeGo.gameObject.SetActive(false);
                    _usedotherCubes.RemoveAt(i);
                }
            }
            //将空置的重新设置位置
            for (int i = _unUsedotherCubes.Count - 1; i >= 0; --i)
            {
                var cubeGo = _unUsedotherCubes[i];
                _lastPosition.x = UnityEngine.Random.Range(-5.0f, 5.0f);
                _lastPosition.y = 0;
                _lastPosition.z += 4f;

             cubeGo.transform.position = _lastPosition;
                cubeGo.gameObject.SetActive(true);
                _unUsedotherCubes.RemoveAt(i);
                _usedotherCubes.Add(cubeGo);
            }
          
        }
    }
}