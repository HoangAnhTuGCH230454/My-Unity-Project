using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    GameManager game;

    private void OnEnable()
    {
        game = target as GameManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Mathf.Approximately(game.defaultRespawnpoint.sqrMagnitude, 0))
        {
            EditorGUILayout.HelpBox("Default respawn point is not set. This will break the death scene", MessageType.Warning);

            PlayerController play = FindObjectOfType<PlayerController>();
            if (play)
            {
                if (GUILayout.Button("Use player scene transition"))
                {
                    Undo.RecordObject(game, "Update game manager default respawn position");
                    game.defaultRespawnpoint = play.transform.position;
                    EditorUtility.SetDirty(game);
                }
            }
        }
    }
}
