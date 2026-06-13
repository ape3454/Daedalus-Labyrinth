using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager), true)]

public class GameManagerEditor : Editor
{
    GameManager gameManager;

    private void Awake()
    {
        gameManager = (GameManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Restart"))
        {
            gameManager.RestartScene();
        }
        if (GUILayout.Button("Reset Elements"))
        {
            gameManager.ResetElements();
        }
    }
}
