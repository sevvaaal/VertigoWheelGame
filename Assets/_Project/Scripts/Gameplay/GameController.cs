using UnityEngine;

// The game's referee: tracks the zone, decides what a spin result means and keeps the inventory.
public class GameController : MonoBehaviour
{
    private const int FirstZone = 1;

    [SerializeField] private WheelSpinner wheelSpinner;
    [SerializeField] private WheelView wheelView;
    [SerializeField] private RewardListView rewardList;
    [SerializeField] private BombPopup bombPopup;
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

        wheelSpinner.SpinCompleted += HandleSpinCompleted;
        bombPopup.RestartClicked += Restart;

        currentZone = startZone;
        ShowZone();
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
        ShowZone();
    }

    // Puts the right wheel on screen for the current zone: bronze with a bomb, silver every 5th, golden every 30th.
    private void ShowZone()
    {
        ZoneType type = zoneRules.GetZoneType(currentZone);
        WheelSO wheel = zoneConfig.GetWheel(type);

        wheelView.Show(wheel, zoneRules, currentZone);
        wheelSpinner.SetWheel(wheel);

        Debug.Log($"Zone {currentZone}: {type}");
    }

    // Start a new run from scratch: no rewards, back to the first zone.
    private void Restart()
    {
        bombPopup.Hide();
        inventory.Clear();
        currentZone = FirstZone;
        ShowZone();
    }
}
