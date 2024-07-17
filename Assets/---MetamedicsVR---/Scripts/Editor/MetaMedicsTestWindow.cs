using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MetaMedicsTestWindow : EditorWindow
{
    public const int windowWidth = 400;
    public const int windowHeight = 200;

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Current scene");
        GUILayout.Space(5);
        if (GUILayout.Button("Perform all tests"))
        {
            AllTests();
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Check audios"))
        {
            CheckMissingAudios();
        }
    }

    private void AllTests()
    {
        CheckMissingAudios();
    }

    private void CheckMissingAudios()
    {
        string sufix;
        foreach (LanguageManager.Language language in Enum.GetValues(typeof(LanguageManager.Language)))
        {
            sufix = "_" + LanguageManager.GetLanguageCode(language);
            foreach (AudioManager.AudioName name in Enum.GetValues(typeof(AudioManager.AudioName)))
            {
                if (Resources.Load<AudioClip>("Audio/" + (name + sufix)) == null)
                {
                    Debug.Log((name + sufix) + " not found");
                }
            }
        }
    }
}