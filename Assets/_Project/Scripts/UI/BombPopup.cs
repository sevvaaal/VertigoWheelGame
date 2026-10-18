using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// Shown when the wheel lands on the bomb. It only announces the restart; the GameController decides what restarting means.
public class BombPopup : MonoBehaviour
{
    private const string RestartButtonName = "ui_button_restart";

    [SerializeField] private RectTransform panel;
    [SerializeField] private Button restartButton;
    [SerializeField] private float popDuration = 0.3f;

    public event Action RestartClicked;

    // Runs in the editor whenever this component changes: the button reference is found by name, never dragged by hand.
    private void OnValidate()
    {
        restartButton = UIChildFinder.Find<Button>(transform, RestartButtonName);
    }

    private void Awake()
    {
        restartButton.onClick.AddListener(HandleRestartClicked);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // Grow from nothing to full size with a little overshoot.
        panel.localScale = Vector3.zero;
        panel.DOScale(1f, popDuration).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HandleRestartClicked()
    {
        RestartClicked?.Invoke();
    }
}
