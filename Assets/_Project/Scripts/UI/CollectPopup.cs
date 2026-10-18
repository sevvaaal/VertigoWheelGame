using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Shown when the player leaves: lists every reward taken home, and offers a new run.
public class CollectPopup : PopupView
{
    private const string PlayAgainButtonName = "ui_button_play_again";

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Transform rowContainer;
    [SerializeField] private RewardRowView rowPrefab;

    private readonly List<RewardRowView> rows = new List<RewardRowView>();

    public event Action PlayAgainClicked;

    private void OnValidate()
    {
        playAgainButton = UIChildFinder.Find<Button>(transform, PlayAgainButtonName);
    }

    protected override void Awake()
    {
        playAgainButton.onClick.AddListener(HandlePlayAgainClicked);
        base.Awake();
    }

    public void Show(IReadOnlyDictionary<RewardTypeSO, int> collected)
    {
        ClearRows();

        // The same row prefab as the side panel: one row per reward type.
        foreach (KeyValuePair<RewardTypeSO, int> reward in collected)
        {
            RewardRowView row = Instantiate(rowPrefab, rowContainer);
            row.Setup(reward.Key.Icon);
            row.SetAmount(reward.Value);
            rows.Add(row);
        }

        Show();
    }

    private void ClearRows()
    {
        foreach (RewardRowView row in rows)
        {
            Destroy(row.gameObject);
        }

        rows.Clear();
    }

    private void HandlePlayAgainClicked()
    {
        PlayAgainClicked?.Invoke();
    }
}
