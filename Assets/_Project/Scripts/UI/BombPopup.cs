using System;
using UnityEngine;
using UnityEngine.UI;

// Shown when the wheel lands on the bomb. It only announces the restart; the GameController decides what restarting means.
public class BombPopup : PopupView
{
    private const string RestartButtonName = "ui_button_restart";

    [SerializeField] private Button restartButton;

    public event Action RestartClicked;

    // Runs in the editor whenever this component changes: the button reference is found by name, never dragged by hand.
    private void OnValidate()
    {
        restartButton = UIChildFinder.Find<Button>(transform, RestartButtonName);
    }

    protected override void Awake()
    {
        restartButton.onClick.AddListener(HandleRestartClicked);

        // The parent's Awake hides the popup until it is needed.
        base.Awake();
    }

    private void HandleRestartClicked()
    {
        RestartClicked?.Invoke();
    }
}
