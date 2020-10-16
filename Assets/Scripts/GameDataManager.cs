using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField] private GameDataSO gameDataSO;

    public GameDataSO GameDataSO { get => gameDataSO; set => gameDataSO = value; }

    public Action<int> LevelChanged = delegate { };
    public Action<int> HintNumChanged = delegate { };

    public bool IsLevelValid()
    {
        return gameDataSO.CurrentLevel < gameDataSO.levelSOs.Length;
    }

    public bool IncrementLevel()
    {
        if (gameDataSO.CurrentLevel < gameDataSO.levelSOs.Length-1)
        {
            gameDataSO.CurrentLevel++;

            LevelChanged?.Invoke(gameDataSO.CurrentLevel);

            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void IncreaseUsedHintCount()
    {
        if (gameDataSO.UsedHintCount < GlobalAccess.Current.ConstantsSO.MaxHint)
        {
            gameDataSO.UsedHintCount++;
        }

        HintNumChanged?.Invoke(gameDataSO.UsedHintCount);
    }

    public void SetUsedHintCount(int n)
    {
        gameDataSO.UsedHintCount = n;

        HintNumChanged?.Invoke(gameDataSO.UsedHintCount);
    }
}
