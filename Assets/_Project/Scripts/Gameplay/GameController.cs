using System.Collections;
using DG.Tweening;
using UnityEngine;
using Vertigo.Gameplay;
using Vertigo.UI;

// The game's referee: tracks the zone, decides what a spin result means, keeps the inventory and decides when leaving is allowed.
public class GameController : MonoBehaviour
{
    private const int FirstZone = 1;

    [SerializeField] private WheelSpinner wheelSpinner;
    [SerializeField] private WheelView wheelView;
    [SerializeField] private RewardListView rewardList;
    [SerializeField] private BombPopup bombPopup;
    [SerializeField] private CollectPopup collectPopup;
    [SerializeField] private ButtonView spinButton;
    [SerializeField] private ButtonView leaveButton;
    [SerializeField] private ZoneBarView zoneBar;
    [SerializeField] private RewardFlyEffect rewardFly;
    [SerializeField] private ZoneConfigSO zoneConfig;

    [Tooltip("For testing: the zone a run starts at. Set to 29 to reach the super zone in one spin.")]
    [SerializeField] private int startZone = FirstZone;

    private Inventory inventory;
    private ZoneRules zoneRules;
    private SpinResolver spinResolver;
    private WheelSO currentWheel;
    private int currentZone;
    private int winningSliceIndex;

    // True while a spin's result is still playing (flash, flying icon, shake). Spin and Leave wait for it.
    private bool isShowingResult;

    private void Awake()
    {
        inventory = new Inventory();
        zoneRules = new ZoneRules(zoneConfig.SafeZoneInterval, zoneConfig.SuperZoneInterval, zoneConfig.RewardGrowthPerZone);
        spinResolver = new SpinResolver(new UnityRandomSource());
        rewardList.Bind(inventory);

        spinButton.Clicked += Spin;
        wheelSpinner.SpinStarted += UpdateLeaveButton;
        wheelSpinner.SpinCompleted += HandleSpinCompleted;
        bombPopup.RestartClicked += Restart;
        collectPopup.PlayAgainClicked += Restart;
        leaveButton.Clicked += Leave;
    }

    // Showing the first zone talks to other objects (wheel, zone bar, buttons), so it waits for Start:
    // by then every object has finished its own Awake.
    private void Start()
    {
        currentZone = startZone;
        ShowZone(false);
    }

    // SPIN is only allowed when the wheel is still and the last result has finished playing.
    private void Spin()
    {
        if (wheelSpinner.IsSpinning || isShowingResult)
        {
            return;
        }

        spinButton.SetAvailable(false);

        // Decide first, then let the wheel rotate to the slice that was chosen.
        int sliceCount = currentWheel.Slices.Count;
        winningSliceIndex = spinResolver.PickSliceIndex(sliceCount);
        wheelSpinner.SpinTo(winningSliceIndex, sliceCount);
    }

    private void HandleSpinCompleted()
    {
        SliceData slice = currentWheel.Slices[winningSliceIndex];
        isShowingResult = true;
        StartCoroutine(slice.IsBomb ? PlayBombSequence() : PlayRewardSequence(slice));
    }

    // Shake the wheel, then lose everything.
    private IEnumerator PlayBombSequence()
    {
        yield return wheelView.PlayBombShake().WaitForCompletion();

        inventory.Clear();
        bombPopup.Show();
        isShowingResult = false;
    }

    // Flash and punch the winning slice, fly its icon into the panel, and only then add the reward and advance.
    private IEnumerator PlayRewardSequence(SliceData slice)
    {
        Vector3 iconStart = wheelView.GetSlicePosition(winningSliceIndex);

        yield return wheelView.PlayWinEffect(winningSliceIndex).WaitForCompletion();

        Transform flyTarget = rewardList.GetFlyTarget(slice.Reward.Type);
        yield return rewardFly.Fly(slice.Reward.Type.Icon, iconStart, flyTarget).WaitForCompletion();

        // The slice stores the base amount; the reward actually won grows with the zone.
        int amount = zoneRules.ScaleAmount(slice.Reward.Amount, currentZone);
        inventory.Add(new Reward(slice.Reward.Type, amount));

        currentZone++;
        ShowZone(true);

        isShowingResult = false;
        spinButton.SetAvailable(true);
        UpdateLeaveButton();
    }

    // Puts the right wheel on screen for the current zone: bronze with a bomb, silver every 5th, golden every 30th.
    // animate: slide the zone bar when advancing; jump straight there at the start and on restart.
    private void ShowZone(bool animate)
    {
        ZoneType type = zoneRules.GetZoneType(currentZone);
        currentWheel = zoneConfig.GetWheel(type);

        wheelView.Show(currentWheel, zoneRules, currentZone);
        zoneBar.Show(currentZone, zoneRules, animate);
        UpdateLeaveButton();
    }

    // Leave is possible before any spin, never while the wheel is spinning or a result is still playing.
    private void UpdateLeaveButton()
    {
        leaveButton.SetAvailable(CanLeave());
    }

    private bool CanLeave()
    {
        return !isShowingResult && !wheelSpinner.IsSpinning;
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

        isShowingResult = false;
        spinButton.SetAvailable(true);
    }
}
