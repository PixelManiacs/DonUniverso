using System.IO;
using UnityEngine;
using UnityEditor;

public class BuildSystem : MonoBehaviour
{
    [MenuItem("File/Build WebGL")]
    static public void WebGL()
    {

        Directory.CreateDirectory("Export/WebGL");

        PlayerSettings.WebGL.memorySize = 512;
        PlayerSettings.SplashScreen.show = false;

        string[] scenes = new string[] { "Assets/_Game/SpaceShip.unity" };

        string buildError = BuildPipeline.BuildPlayer(scenes,
                                                  "Export/WebGL",
                                                       BuildTarget.WebGL,
                                                      BuildOptions.Development);

        Debug.Log("Don Universo YEAH --- " + buildError);
    }

    [MenuItem("File/Build WINDOWs")]
    static public void BuildWindows()
    {

        Directory.CreateDirectory("Export/Windows");

        PlayerSettings.SplashScreen.show = false;

        string[] scenes = new string[] { "Assets/_Game/SpaceShip.unity" };

        string buildError = BuildPipeline.BuildPlayer(scenes,
                                                  "Export/Windows/DonUniverso.exe",
                                                      BuildTarget.StandaloneWindows,
                                                      BuildOptions.Development);

        Debug.Log("Don Universo YEAH --- " + buildError);
    }
}
