using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] MainGame mainGame;
    [SerializeField] Text mainText;
    [SerializeField] Button okButton;

    Action Done = delegate { };

    private void Awake()
    {
        mainGame.LevelCompleted += HandleLevelCompleted;

        Done += mainGame.LoadNextLevel;

        Hide();
    }


    private void HandleLevelCompleted(int level)
    {
        mainText.text = $"Level {level+1} Completed!";

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void HandleOkButtonClicked()
    {
        Hide();

        Done?.Invoke();
    }
}
