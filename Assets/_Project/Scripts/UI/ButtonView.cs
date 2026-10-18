using System;
using UnityEngine;
using UnityEngine.UI;

// A game button (Spin, Leave). It only turns itself on or off and announces clicks;
// the GameController decides when each action is allowed and what it does.
[RequireComponent(typeof(Button))]
public class ButtonView : MonoBehaviour
{
    [SerializeField] private Button button;

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
    }

    private void HandleClicked()
    {
        Clicked?.Invoke();
    }
}
