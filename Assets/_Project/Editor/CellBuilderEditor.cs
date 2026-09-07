using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellBuilder))]
[CanEditMultipleObjects]
public class CellBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("CELL EDITOR", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Red Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Red);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Green Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Green);

        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Blue Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Blue);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Yellow Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Yellow);

        GUI.backgroundColor = Color.orange;
        if (GUILayout.Button("Orange Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Orange);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Cyan Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Cyan);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.deepPink;
        if (GUILayout.Button("Pink Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Pink);

        GUI.backgroundColor = Color.brown;
        if (GUILayout.Button("Brown Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Brown);

        GUI.backgroundColor = Color.purple;
        if (GUILayout.Button("Purple Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.Purple);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("White Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.White);

        

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        GUI.backgroundColor = Color.black;
        if (GUILayout.Button("Clear Knox", GUILayout.Height(35)))
            SetupKnox(KnoxColorType.None);
    }

    private void SetupKnox(KnoxColorType knoxColorType)
    {
        foreach (var obj in targets)
        {
            if (obj is CellBuilder cell)
            {
                cell.SetKnox(knoxColorType);
                EditorUtility.SetDirty(cell);
            }
        }
    }
}
