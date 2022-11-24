// Create a GameSettingsEditor 

using UnityEditor;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // serializedObject.Update();
        // EditorGUILayout.PropertyField(waterTilePrefab);
        // serializedObject.ApplyModifiedProperties();
    }

}

