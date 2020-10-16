using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="GameData", menuName ="ScriptableObjects/GameDataSO")]
public class GameDataSO : ScriptableObject
{
    public int CurrentLevel;
    public int UsedHintCount;
    public int CurrentRolledOutCount;
    public int UptoHintCount;

    public LevelSO[] levelSOs;

    public void ResetOnNextLevel()
    {
        UsedHintCount = 0;
        CurrentRolledOutCount = 0;
        UptoHintCount = 0;
    }
}
[CustomEditor(typeof(GameDataSO))]
public class GameDataSOCE: Editor
{
    GameDataSO gameDataSO { get { return target as GameDataSO; } }

    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Reset"))
        {
            gameDataSO.ResetOnNextLevel();
            
            gameDataSO.CurrentLevel = 0;

            EditorUtility.SetDirty(gameDataSO);
        }

        base.OnInspectorGUI();

    }
}