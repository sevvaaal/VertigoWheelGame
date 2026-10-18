using UnityEngine;

// The game's referee: decides what a spin result means and keeps the inventory.
public class GameController : MonoBehaviour
{
    [SerializeField] private WheelSpinner wheelSpinner;
    [SerializeField] private RewardListView rewardList;
    [SerializeField] private BombPopup bombPopup;

    private Inventory inventory;

    private void Awake()
    {
        inventory = new Inventory();
        rewardList.Bind(inventory);

        wheelSpinner.SpinCompleted += HandleSpinCompleted;
        bombPopup.RestartClicked += Restart;
    }

    private void HandleSpinCompleted(SliceData slice)
    {
        if (slice.IsBomb)
        {
            inventory.Clear();
            bombPopup.Show();
        }
        else
        {
            inventory.Add(slice.Reward);
        }
    }

    // Start a new run from scratch. Zones will be reset here too once they exist.
    private void Restart()
    {
        bombPopup.Hide();
    }
}
