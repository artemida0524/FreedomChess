using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneSwitcherWindow : EditorWindow
{
    private string scenesPath = "Assets/_Project/Scenes";

    [MenuItem("Tools/Scene Switcher")]
    public static void ShowWindow()
    {
        GetWindow<SceneSwitcherWindow>("Scene Switcher");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scenes", EditorStyles.boldLabel);

        if (!Directory.Exists(scenesPath))
        {
            EditorGUILayout.HelpBox("Folder not found!", MessageType.Error);
            return;
        }

        string[] sceneFiles = Directory.GetFiles(scenesPath, "*.unity", SearchOption.AllDirectories);

        foreach (var scene in sceneFiles)
        {
            string sceneName = Path.GetFileNameWithoutExtension(scene);

            if (GUILayout.Button(sceneName))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scene);
                }
            }
        }
    }
}