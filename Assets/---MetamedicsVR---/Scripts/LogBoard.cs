using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogBoard : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI[] tmps;

    private void Start()
    {
        Logger logger = Logger.GetInstance();
        logger.OnLog.AddListener(Log);
        logger.OnLogWarning.AddListener(LogWarning);
        logger.OnLogError.AddListener(LogError);
    }

    private void Log(string s)
    {
        AddMessage(s);
    }

    private void LogWarning(string s)
    {
        AddMessage("<color=yellow>" + s + "</color>");
    }

    private void LogError(string s)
    {
        AddMessage("<color=red>" + s + "</color>");
    }

    private void AddMessage(string s)
    {
        int emptyLineIndex = tmps.Length;
        while (emptyLineIndex > 0 && tmps[emptyLineIndex - 1].text == "")
        {
            emptyLineIndex--;
        }
        if (emptyLineIndex < tmps.Length)
        {
            tmps[emptyLineIndex].text = s;
        }
        else
        {
            for (int i = 1; i < tmps.Length; i++)
            {
                tmps[i - 1].text = tmps[i].text;
            }
            tmps[tmps.Length - 1].text = s;
        }
    }
}
