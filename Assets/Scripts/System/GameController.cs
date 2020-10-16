using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MainUI mainUI;
    [SerializeField] MainGame mainGame;

    private void OnEnable()
    {
        WireUpEvents();
    }
    private void OnDisable()
    {
        UnwireEvents();
    }

    private void WireUpEvents()
    {
        mainUI.HintButtonClicked += mainGame.ShowHint;
    }
    private void UnwireEvents()
    {
        mainUI.HintButtonClicked -= mainGame.ShowHint;

    }
}
