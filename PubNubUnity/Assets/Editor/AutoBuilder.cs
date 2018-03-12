using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class AutoBuilder
{

    static string GetProjectName ()
    {
        string[] s = Application.dataPath.Split ('/');
        return s [s.Length - 2];
    }

    static string[] GetScenePaths ()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++) {
            scenes [i] = EditorBuildSettings.scenes [i].path;
        }

        return scenes;
    }

    [MenuItem ("File/AutoBuilder/iOS")]
    static void PerformiOSBuild ()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.iOS);
        BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/iOS", BuildTarget.iOS, BuildOptions.None);
    }
    
    [MenuItem ("File/AutoBuilder/StandaloneOSXUniversal")]
    static void PerformStandaloneOSXUniversal ()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneOSX);
        BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/StandaloneOSXUniversal.app", BuildTarget.StandaloneOSX, BuildOptions.None);
    }

    [MenuItem ("File/AutoBuilder/StandaloneWindows64")]
    static void PerformStandaloneWindows64 ()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows64);
        BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/StandaloneWindows64.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem ("File/AutoBuilder/Android")]
    static void PerformAndroidBuild ()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.Android);
        BuildPipeline.BuildPlayer (GetScenePaths (), "Builds/Android", BuildTarget.Android, BuildOptions.None);
    }
}
