using System;
using UnityEngine;
using UnityEngine.UI;

// A game button (Spin, Leave). It only turns itself on or off and announces clicks;
// the GameController decides when each action is allowed and what it does.
[RequireComponent(typeof(Button))]
public class ButtonView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private float unavailableAlpha = 0.4f;

    // Fades the button and its text together, so "not available right now" is obvious.
    private CanvasGroup canvasGroup;

    public event Action Clicked;

    // The button is on this same object, so it is found automatically instead of being dragged in.
    private void OnValidate()
    {
        button = GetComponent<Button>();
    }

    private void Awake()
    {
        button.onClick.AddListener(HandleClicked);
    }

    public void SetAvailable(bool isAvailable)
    {
        button.interactable = isAvailable;

        // Fetched here rather than in Awake: the GameController may call this before this button's Awake has run.
        if (canvasGroup == null && !TryGetComponent(out canvasGroup))
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = isAvailable ? 1f : unavailableAlpha;
    }

    private void HandleClicked()
    {
        Clicked?.Invoke();
    }
}
