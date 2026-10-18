using UnityEngine;

// The game's referee: decides what a spin result means and keeps the inventory.
public class GameController : MonoBehaviour
{
    [SerializeField] private WheelSpinner wheelSpinner;
    [SerializeField] private RewardListView rewardList;

    private Inventory inventory;

    private void Awake()
    {
        inventory = new Inventory();
        rewardList.Bind(inventory);

        wheelSpinner.SpinCompleted += HandleSpinCompleted;
    }

    private void HandleSpinCompleted(SliceData slice)
    {
        if (slice.IsBomb)
        {
            inventory.Clear();
        }
        else
        {
            inventory.Add(slice.Reward);
        }
    }
}
