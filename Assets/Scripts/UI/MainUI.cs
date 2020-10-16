using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] GameDataManager gameDataManager;
    [SerializeField] Button hintButton;
    [SerializeField] Text hintButtonText;
    
    [SerializeField] MainGame mainGame;

    public Action HintButtonClicked = delegate { };


    private void Awake()
    {
        mainGame.NewLevelLoaded += _=>Show();
        mainGame.HintNumChanged += UpdateHintButton;
        mainGame.LevelCompleted += HandleLevelCompleted;
    }

    //private void OnEnable()
    //{
    //    mainGame.HintNumChanged += UpdateHintButton;
    //    mainGame.LevelCompleted += HandleLevelCompleted;
    //}
    //private void OnDisable()
    //{
    //    mainGame.HintNumChanged -= UpdateHintButton;
    //    mainGame.LevelCompleted -= HandleLevelCompleted;
    //}

    private void HandleLevelCompleted(int level)
    {
        Hide();
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void OnHintButtonClicked()
    {
        //provide some UI visual effects

        if (gameDataManager.GameDataSO.UsedHintCount < GlobalAccess.Current.ConstantsSO.MaxHint)
        {
            //and then signal out to any one who are of interest
            HintButtonClicked?.Invoke();
        }
        else
        {
            ShakeHintButton();
        }
    }

    public void UpdateHintButton(int hintNum)
    {
        hintButtonText.text = $"Hint {Mathf.Max(0, hintNum)}";
    }

    public void ShakeHintButton()
    {
        var animator = hintButton.GetComponent<Animator>();

        animator?.SetTrigger("Shake");
    }
}
