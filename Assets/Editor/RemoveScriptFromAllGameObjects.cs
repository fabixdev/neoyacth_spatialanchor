using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class RemoveScriptFromAllGameObjects : EditorWindow
{
    private string scriptName;

    [MenuItem("Tools/Remove Script From All GameObjects")]
    public static void ShowWindow()
    {
        GetWindow<RemoveScriptFromAllGameObjects>("Remove Script");
    }

    private void OnGUI()
    {
        GUILayout.Label("Remove Script From All GameObjects", EditorStyles.boldLabel);

        scriptName = EditorGUILayout.TextField("Script Name", scriptName);

        if (GUILayout.Button("Remove Script"))
        {
            RemoveScript();
        }
    }

    private void RemoveScript()
    {
        if (string.IsNullOrEmpty(scriptName))
        {
            UnityEngine.Debug.Log("Script name is empty or null.");
            return;
        }

        MonoBehaviour[] scripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        int count = 0;

        foreach (MonoBehaviour script in scripts)
        {
            if (script.GetType().Name == scriptName)
            {
                DestroyImmediate(script, true);
                count++;
            }
        }

        UnityEngine.Debug.Log($"Removed {scriptName} from {count} GameObjects.");
    }
}
