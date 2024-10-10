using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceSettings : MonoBehaviour
{
    private const string KEY_UseVoice = "UseVoice";

    public void UseVoice(bool voice)
    {
        PlayerPrefs.SetInt(KEY_UseVoice, voice ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool IsUsingVoice()
    {
        return PlayerPrefs.GetInt(KEY_UseVoice, 0) == 1;
    }
}
