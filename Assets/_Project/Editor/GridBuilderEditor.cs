using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridBuilder))]
public class GridBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GridBuilder gridBuilder = (GridBuilder)target;

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("LEVEL EDITOR", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Generate Grid", GUILayout.Height(35)))
            gridBuilder.GenerateGrid();

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Reset Grid", GUILayout.Height(35)))
            gridBuilder.ResetGrid();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save Grid", GUILayout.Height(35)))
            gridBuilder.SaveToLevelSO();

        GUI.backgroundColor = Color.darkGreen;
        if (GUILayout.Button("Load Grid", GUILayout.Height(35)))
            gridBuilder.LoadFromeLevelSO();

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
    }
}
