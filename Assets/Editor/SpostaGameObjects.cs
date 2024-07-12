using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class SpostaGameObjects : EditorWindow
{
    private string searchPattern = "esempio";
    private string parentObjectName = "ParentObject";

    [MenuItem("Tools/Sposta GameObjects")]
    public static void ShowWindow()
    {
        GetWindow<SpostaGameObjects>("Sposta GameObjects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sposta GameObjects", EditorStyles.boldLabel);

        searchPattern = EditorGUILayout.TextField("Nome Pattern", searchPattern);
        parentObjectName = EditorGUILayout.TextField("Nome GameObject Padre", parentObjectName);

        if (GUILayout.Button("Sposta"))
        {
            Sposta();
        }
    }

    private void Sposta()
    {
        // Crea il GameObject padre se non esiste
        GameObject parentObject = GameObject.Find(parentObjectName);
        if (parentObject == null)
        {
            parentObject = new GameObject(parentObjectName);
            Undo.RegisterCreatedObjectUndo(parentObject, "Create Parent Object");
        }

        // Trova tutti i GameObject che corrispondono al pattern di ricerca
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int movedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (IsMatch(obj.name, searchPattern))
            {
                Undo.SetTransformParent(obj.transform, parentObject.transform, "Sposta GameObject");
                UnityEngine.Debug.Log($"Spostato {obj.name} sotto {parentObject.name}");
                movedCount++;
            }
        }

        EditorUtility.DisplayDialog("Completato", $"{movedCount} GameObject sono stati spostati.", "Ok");
    }

    // Funzione per controllare se il nome del GameObject corrisponde al pattern "esempioX"
    private bool IsMatch(string objectName, string pattern)
    {
        if (objectName.StartsWith(pattern))
        {
            string suffix = objectName.Substring(pattern.Length).Trim();
            int number;
            return int.TryParse(suffix, out number);
        }
        return false;
    }
}
