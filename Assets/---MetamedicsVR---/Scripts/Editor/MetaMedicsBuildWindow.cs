using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class MetaMedicsBuildWindow : EditorWindow
{
    private string buildSettingsFilePath = "Assets/Resources/MetaMedicsBuildSettings.asset";
    private MetaMedicsBuildSettings buildSettings;
    private int selectedDeviceIndex = 0;
    private string[] allDeviceIndex;
    private int selectedTrackingIndex = 0;
    private string[] allTrackingIndex;
    private int selectedLanguageIndex = 0;
    private string[] allLanguageIndex;

    private string packageNamesFilePath = "packagenames.nogit.txt";
    private List<PackageInfo> packagesInfo = new List<PackageInfo>();
    private int selectedPackageInfoIndex = 0;
    private string[] packageInfoAlias;

    private string packageName;  
    private string version;
    private int bundleVersionCode;

    private string keysInfoFilePath = "keystores.nogit.txt";
    private List<KeyInfo> keysInfo = new List<KeyInfo>();
    private int selectedKeyInfoIndex = 0;
    private string[] keysInfoAlias;

    private string keystorePath = "";
    private string keystorePassword = "";
    private string keyAlias = "";
    private string keyPassword = "";

    private struct PackageInfo
    {
        public string packageAlias;
        public string packageName;
    }

    private struct KeyInfo
    {
        public string keyInfoAlias;
        public string keystorePath;
        public string keystorePassword;
        public string keyAlias;
        public string keyPassword;
    }

    public const int windowWidth = 600;
    public const int windowHeight = 420;

    private void OnEnable()
    {
        packageName = PlayerSettings.applicationIdentifier;
        version = PlayerSettings.bundleVersion;
        bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
        LoadOrCreateGameSettings();
        LoadPackageNames();
        LoadKeysInfo();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Build Type", EditorStyles.boldLabel);
        GUILayout.Space(5);



        if (allDeviceIndex.Length > 0)
        {
            int newSelectedIndex = EditorGUILayout.Popup("Device", selectedDeviceIndex, allDeviceIndex);
            if (newSelectedIndex != selectedDeviceIndex)
            {
                selectedDeviceIndex = newSelectedIndex;
                buildSettings.targetDevice = (GameManager.Device)selectedDeviceIndex;
            }
        }

        if (allTrackingIndex.Length > 0)
        {
            int newSelectedIndex = EditorGUILayout.Popup("Tracking", selectedTrackingIndex, allTrackingIndex);
            if (newSelectedIndex != selectedTrackingIndex)
            {
                selectedTrackingIndex = newSelectedIndex;
                buildSettings.targetTracking = (GameManager.Tracking)selectedTrackingIndex;
            }
        }

        if (allLanguageIndex.Length > 0)
        {
            int newSelectedIndex = EditorGUILayout.Popup("Language", selectedLanguageIndex, allLanguageIndex);
            if (newSelectedIndex != selectedLanguageIndex)
            {
                selectedLanguageIndex = newSelectedIndex;
                buildSettings.targetLanguage = (LanguageManager.Language)selectedLanguageIndex;
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("App Version", EditorStyles.boldLabel);
        GUILayout.Space(5);

        if (packageInfoAlias != null && packageInfoAlias.Length > 0)
        {
            int newSelectedIndex = EditorGUILayout.Popup("Select Package Name", selectedPackageInfoIndex, packageInfoAlias);
            if (newSelectedIndex != selectedPackageInfoIndex)
            {
                selectedPackageInfoIndex = newSelectedIndex;
                packageName = packagesInfo[selectedPackageInfoIndex].packageName;
            }
        }
        packageName = EditorGUILayout.TextField("Package Name", packageName);
        version = EditorGUILayout.TextField("Version", version);
        bundleVersionCode = EditorGUILayout.IntField("Bundle Version Code", bundleVersionCode);

        GUILayout.Space(20);
        GUILayout.Label("Key", EditorStyles.boldLabel);
        GUILayout.Space(5);

        if (keysInfoAlias != null && keysInfoAlias.Length > 0)
        {
            int newSelectedIndex = EditorGUILayout.Popup("Select Keystore", selectedKeyInfoIndex, keysInfoAlias);
            if (newSelectedIndex != selectedKeyInfoIndex)
            {
                selectedKeyInfoIndex = newSelectedIndex;
                keystorePath = keysInfo[selectedKeyInfoIndex].keystorePath;
                keystorePassword = keysInfo[selectedKeyInfoIndex].keystorePassword;
                keyAlias = keysInfo[selectedKeyInfoIndex].keyAlias;
                keyPassword = keysInfo[selectedKeyInfoIndex].keyPassword;
            }
        }
        GUILayout.BeginHorizontal();
        keystorePath = EditorGUILayout.TextField("Keystore Path", keystorePath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            keystorePath = EditorUtility.OpenFilePanel("Select Keystore", "", "keystore");
        }
        GUILayout.EndHorizontal();
        keystorePassword = EditorGUILayout.PasswordField("Keystore Password", keystorePassword);
        keyAlias = EditorGUILayout.TextField("Key Alias", keyAlias);
        keyPassword = EditorGUILayout.PasswordField("Key Password", keyPassword);

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build", GUILayout.Width(120)))
        {
            PerformBuild();
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Build and Run", GUILayout.Width(120)))
        {
            PerformBuildAndRun();
        }
        GUILayout.EndHorizontal();
    }

    private void LoadOrCreateGameSettings()
    {
        buildSettings = AssetDatabase.LoadAssetAtPath<MetaMedicsBuildSettings>(buildSettingsFilePath);
        if (buildSettings == null)
        {
            buildSettings = CreateInstance<MetaMedicsBuildSettings>();
            AssetDatabase.CreateAsset(buildSettings, buildSettingsFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        selectedDeviceIndex = (int)buildSettings.targetDevice;
        allDeviceIndex = Enum.GetNames(typeof(GameManager.Device));
        selectedTrackingIndex = (int)buildSettings.targetTracking;
        allTrackingIndex = Enum.GetNames(typeof(GameManager.Tracking));
        selectedLanguageIndex = (int)buildSettings.targetLanguage;
        allLanguageIndex = Enum.GetNames(typeof(LanguageManager.Language));
    }

    private void LoadPackageNames()
    {
        if (File.Exists(packageNamesFilePath))
        {
            string[] lines = File.ReadAllLines(packageNamesFilePath);
            PackageInfo packageInfo = new PackageInfo();
            string currentLine;
            for (int i = 0; i < lines.Length; i++)
            {
                currentLine = lines[i].Trim();
                if (currentLine != "")
                {
                    if (packageInfo.packageAlias == null)
                    {
                        packageInfo.packageAlias = currentLine;
                    }
                    else if (packageInfo.packageName == null)
                    {
                        packageInfo.packageName = currentLine;
                        packagesInfo.Add(packageInfo);
                        packageInfo = new PackageInfo();
                    }
                }
            }
            if (packagesInfo.Count > 0)
            {
                selectedPackageInfoIndex = 0;
                packageName = packagesInfo[0].packageName;
                packageInfoAlias = packagesInfo.Select(x => x.packageAlias).ToArray();
            }
        }
    }

    private void LoadKeysInfo()
    {
        if (File.Exists(keysInfoFilePath))
        {
            string[] lines = File.ReadAllLines(keysInfoFilePath);
            KeyInfo keyInfo = new KeyInfo();
            string currentLine;
            for (int i = 0; i < lines.Length; i++)
            {
                currentLine = lines[i].Trim();
                if (currentLine != "")
                {
                    if (keyInfo.keyInfoAlias == null)
                    {
                        keyInfo.keyInfoAlias = currentLine;
                    }
                    else if (keyInfo.keystorePath == null)
                    {
                        keyInfo.keystorePath = currentLine;
                    }
                    else if (keyInfo.keystorePassword == null)
                    {
                        keyInfo.keystorePassword = currentLine;
                    }
                    else if (keyInfo.keyAlias == null)
                    {
                        keyInfo.keyAlias = currentLine;
                    }
                    else if (keyInfo.keyPassword == null)
                    {
                        keyInfo.keyPassword = currentLine;
                        keysInfo.Add(keyInfo);
                        keyInfo = new KeyInfo();
                    }
                }
            }
            if (keysInfo.Count > 0)
            {
                selectedKeyInfoIndex = 0;
                keystorePath = keysInfo[0].keystorePath;
                keystorePassword = keysInfo[0].keystorePassword;
                keyAlias = keysInfo[0].keyAlias;
                keyPassword = keysInfo[0].keyPassword;
                keysInfoAlias = keysInfo.Select(x => x.keyInfoAlias).ToArray();
            }
        }
    }

    private void PerformBuild()
    {
        PlayerSettings.applicationIdentifier = packageName;
        PlayerSettings.bundleVersion = version;
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keystorePass = keystorePassword;
        PlayerSettings.Android.keyaliasName = keyAlias;
        PlayerSettings.Android.keyaliasPass = keyPassword;

        string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = scenes;
        string outputPath = EditorUtility.SaveFilePanel("Save Build", "", PlayerSettings.productName + "_" + bundleVersionCode, "apk");
        if (string.IsNullOrEmpty(outputPath))
        {
            return;
        }
        buildOptions.locationPathName = outputPath;
        buildOptions.target = GetBuildTarget();
        buildOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildOptions);
    }

    private void PerformBuildAndRun()
    {
        BuildTarget buildTarget = GetBuildTarget();
        string tempBuildPath = "Temp/Build.apk";
        if (buildTarget != BuildTarget.Android && buildTarget != BuildTarget.StandaloneWindows64)
        {
            Debug.Log("Can't build and run in this platform");
            return;
        }
        PlayerSettings.applicationIdentifier = packageName;
        PlayerSettings.bundleVersion = version;
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keystorePass = keystorePassword;
        PlayerSettings.Android.keyaliasName = keyAlias;
        PlayerSettings.Android.keyaliasPass = keyPassword;

        string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = scenes;
        buildOptions.locationPathName = tempBuildPath;
        buildOptions.target = buildTarget;
        buildOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        if (report.summary.result == BuildResult.Succeeded)
        {
            switch (buildOptions.target)
            {
                case BuildTarget.Android:
                    string adbPath = EditorPrefs.GetString("AndroidSdkRoot") + "/platform-tools/adb";
                    string packageName = PlayerSettings.applicationIdentifier;
                    System.Diagnostics.Process.Start(adbPath, "install -r " + tempBuildPath);
                    System.Diagnostics.Process.Start(adbPath, "shell am start -n " + packageName + "/.MainActivity");
                    break;
                case BuildTarget.StandaloneWindows64:
                    System.Diagnostics.Process.Start(tempBuildPath);
                    break;
            }
        }
        else
        {
            Debug.LogError(report);
        }
    }

    private BuildTarget GetBuildTarget()
    {
        switch (buildSettings.targetDevice)
        {
            case GameManager.Device.OculusQuest2:
            case GameManager.Device.Mobile:
                return BuildTarget.Android;
            case GameManager.Device.PC:
                return BuildTarget.StandaloneWindows64;
            case GameManager.Device.Web:
                return BuildTarget.WebGL;

        }
        return BuildTarget.NoTarget;
    }
}