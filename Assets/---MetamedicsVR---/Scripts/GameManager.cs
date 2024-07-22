using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourInstance<GameManager>
{
    public bool allowSceneLoading = true;
    public GameMode currentGameMode { get; protected set; }
    public PlayersMode currentPlayersMode { get; protected set; }
    public SceneName currentScene { get; protected set; } = SceneName.Lobby;

    private Coroutine loadingScene;
    private MetaMedicsBuildSettings settings;

    protected override void OnInstance()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeGameMode(GameMode gamemode) 
    {
        if (currentGameMode != gamemode)
        {
            currentGameMode = gamemode;
        }
    }

    public enum PlayersMode
    {
        SinglePlayer,
        MultiPlayer
    }

    public void ChangePlayersMode(PlayersMode playersMode)
    {
        if (currentPlayersMode != playersMode)
        {
            currentPlayersMode = playersMode;
        }
    }

    public enum GameMode
    {
        Practice,
        Exam
    }

    public void LoadScene(SceneName sceneName)
    {
        if (allowSceneLoading && loadingScene == null)
        {
            loadingScene = StartCoroutine(LoadingScene(sceneName));
        }
    }

    public MetaMedicsBuildSettings GetSettings()
    {
        if (settings == null)
        {
            settings = Resources.Load<MetaMedicsBuildSettings>("MetaMedicsBuildSettings");
        }
        return settings;
    }

    private IEnumerator LoadingScene(SceneName sceneName)
    {
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync(GetSceneString(sceneName));
        yield return asyncLoading;
        currentScene = sceneName;
        loadingScene = null;
    }

    public SceneName GetCurrentScene()
    {
        return currentScene;
    }

    public string GetSceneString(SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneName.Lobby:
                return "Lobby";
        }
        return "";
    }

    public enum SceneName
    {
        Lobby
    }

    public enum Device
    {
        OculusQuest2,
        PC,
        Mobile,
        Web
    }

    public enum Tracking
    {
        Any,
        Controllers,
        HandTracking,
        None
    }
}