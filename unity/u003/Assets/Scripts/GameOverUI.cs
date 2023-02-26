using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI
{
    private Transform _trans;
    private Button _btnRestart;
    private Text _textLifetime;
    public GameOverUI(Transform tans)
    {
        _trans= tans;
        _btnRestart = _trans.Find("btn_restart").GetComponent<Button>();
        _btnRestart.onClick.AddListener(OnClickBtnReStart);
        _textLifetime = _trans.Find("life_time").GetComponent<Text>();
    }

    private void OnClickBtnReStart()
    {
        EventCenter.Instance.GameReStart.Broadcast();
    }
    public void SetActive(bool state)
    {
        _trans.gameObject.SetActive(state);
    }
    public void RefreshLifeTimeText(String lifetime)
    {
        _textLifetime.text = lifetime;
    }
}

