using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.Start();
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.Updater.Update();
    }
    private void LateUpdate()
    {
        GameManager.Instance.LateUpdater.Update();
    }
}