using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainUI
{
    private Text _speedText;
    private Text _distanceText;
    private Text _life_timeText;
    private Transform _transform;
    public MainUI(Transform transform)
    {
        _transform = transform;
        _speedText = _transform.Find("speed").GetComponent<Text>();
        _distanceText = _transform.Find("distance").GetComponent<Text>();
        _life_timeText = _transform.Find("life_time").GetComponent<Text>();
    }

    public void RefreshSpeedText(string speedText)
    {
        _speedText.text = speedText;
    }
    public void RefreshDistanceText(string text)
    {
        _distanceText.text = text;
    }
    public void RefreshLiftTimeText(string text)
    {
        _life_timeText.text = text;
    }
}
