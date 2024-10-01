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


    public GameObject pantallaFibrilarVentriculationInDefibrilator;
    public GameObject pantallaSynusRythmInDefibrilator;
    public GameObject pantallaFibrilarVentriculationPlayer;
    public GameObject pantallaSynusRythmPlayer;

    public GameObject askThemToComePanel;

    public GameObject parentPanelSalir;

    public string finalTime;
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
            case SceneName.Briefing:
                return "Briefing";
            case SceneName.BoxVital:
                return "BoxVital";
        }
        return "";
    }

    public void FinishPatientStabilization() 
    {
        GameManager.GetInstance().pantallaFibrilarVentriculationInDefibrilator.SetActive(false);
        GameManager.GetInstance().pantallaFibrilarVentriculationPlayer.SetActive(false);
        GameManager.GetInstance().pantallaSynusRythmInDefibrilator.SetActive(true);
        GameManager.GetInstance().pantallaSynusRythmPlayer.SetActive(true);
        parentPanelSalir.SetActive(true);
    }

    public enum SceneName
    {
        Lobby,
        Briefing,
        BoxVital
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