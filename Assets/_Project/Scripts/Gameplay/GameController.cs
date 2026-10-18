using UnityEngine;

// The game's referee: tracks the zone, decides what a spin result means, keeps the inventory and decides when leaving is allowed.
public class GameController : MonoBehaviour
{
    private const int FirstZone = 1;

    [SerializeField] private WheelSpinner wheelSpinner;
    [SerializeField] private WheelView wheelView;
    [SerializeField] private RewardListView rewardList;
    [SerializeField] private BombPopup bombPopup;
    [SerializeField] private CollectPopup collectPopup;
    [SerializeField] private LeaveButtonView leaveButton;
    [SerializeField] private ZoneBarView zoneBar;
    [SerializeField] private ZoneConfigSO zoneConfig;

    [Tooltip("For testing: the zone a run starts at. Set to 29 to reach the super zone in one spin.")]
    [SerializeField] private int startZone = FirstZone;

    private Inventory inventory;
    private ZoneRules zoneRules;
    private int currentZone;

    private void Awake()
    {
        inventory = new Inventory();
        zoneRules = new ZoneRules(zoneConfig.SafeZoneInterval, zoneConfig.SuperZoneInterval, zoneConfig.RewardGrowthPerZone);
        rewardList.Bind(inventory);

        wheelSpinner.SpinStarted += UpdateLeaveButton;
        wheelSpinner.SpinCompleted += HandleSpinCompleted;
        bombPopup.RestartClicked += Restart;
        collectPopup.PlayAgainClicked += Restart;
        leaveButton.Clicked += Leave;

        currentZone = startZone;
        ShowZone(false);
    }

    private void HandleSpinCompleted(SliceData slice)
    {
        if (slice.IsBomb)
        {
            inventory.Clear();
            bombPopup.Show();
            return;
        }

        // The slice stores the base amount; the reward actually won grows with the zone.
        int amount = zoneRules.ScaleAmount(slice.Reward.Amount, currentZone);
        inventory.Add(new Reward(slice.Reward.Type, amount));

        currentZone++;
        ShowZone(true);
    }

    // Puts the right wheel on screen for the current zone: bronze with a bomb, silver every 5th, golden every 30th.
    // animate: slide the zone bar when advancing; jump straight there at the start and on restart.
    private void ShowZone(bool animate)
    {
        ZoneType type = zoneRules.GetZoneType(currentZone);
        WheelSO wheel = zoneConfig.GetWheel(type);

        wheelView.Show(wheel, zoneRules, currentZone);
        wheelSpinner.SetWheel(wheel);
        zoneBar.Show(currentZone, zoneRules, animate);
        UpdateLeaveButton();
    }

    // Leave is only possible on a safe or super zone, and never while the wheel is spinning.
    private void UpdateLeaveButton()
    {
        leaveButton.SetAvailable(CanLeave());
    }

    private bool CanLeave()
    {
        return zoneRules.CanLeave(zoneRules.GetZoneType(currentZone), wheelSpinner.IsSpinning);
    }

    // Walk away with everything collected so far.
    private void Leave()
    {
        if (!CanLeave())
        {
            return;
        }

        collectPopup.Show(inventory.Amounts);
    }

    // Start a new run from scratch: no rewards, back to the first zone.
    private void Restart()
    {
        bombPopup.Hide();
        collectPopup.Hide();
        inventory.Clear();
        currentZone = FirstZone;
        ShowZone(false);
    }
}
