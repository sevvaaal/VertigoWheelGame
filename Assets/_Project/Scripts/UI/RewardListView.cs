using System.Collections.Generic;
using UnityEngine;

// The collected rewards panel. It listens to the inventory and keeps one row per reward type.
public class RewardListView : MonoBehaviour
{
    [SerializeField] private Transform rowContainer;
    [SerializeField] private RewardRowView rowPrefab;

    private readonly Dictionary<RewardTypeSO, RewardRowView> rows = new Dictionary<RewardTypeSO, RewardRowView>();

    public void Bind(Inventory inventory)
    {
        inventory.AmountChanged += HandleAmountChanged;
        inventory.Cleared += HandleCleared;
    }

    // Where a won reward's icon should fly to: its own row, or the panel itself if this type has no row yet.
    public Transform GetFlyTarget(RewardTypeSO type)
    {
        return rows.TryGetValue(type, out RewardRowView row) ? row.transform : rowContainer;
    }

    private void HandleAmountChanged(RewardTypeSO type, int total)
    {
        if (!rows.TryGetValue(type, out RewardRowView row))
        {
            // First reward of this type: add a new row. The layout group places it under the others.
            row = Instantiate(rowPrefab, rowContainer);
            row.Setup(type.Icon);
            rows[type] = row;
        }

        row.SetAmount(total);
    }

    private void HandleCleared()
    {
        foreach (RewardRowView row in rows.Values)
        {
            Destroy(row.gameObject);
        }

        rows.Clear();
    }
}
