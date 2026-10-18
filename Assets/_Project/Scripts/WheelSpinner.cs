using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// Spins the wheel and announces which slice it landed on. It does not decide what the result means.
public class WheelSpinner : MonoBehaviour
{
    [SerializeField] private Button spinButton;
    [SerializeField] private RectTransform wheel;
    [SerializeField] private float spinDuration = 3f;
    [SerializeField] private int fullTurns = 5;
    [SerializeField] private Ease spinEase = Ease.OutCubic;

    private WheelSO wheelData;
    private SliceData winningSlice;

    public event Action SpinStarted;
    public event Action<SliceData> SpinCompleted;

    public bool IsSpinning { get; private set; }

    private void Awake()
    {
        spinButton.onClick.AddListener(Spin);
    }

    // The wheel changes with the zone (bronze, silver, golden), so the winner is always picked from the current one.
    public void SetWheel(WheelSO wheel)
    {
        wheelData = wheel;
    }

    private void Spin()
    {
        IsSpinning = true;
        spinButton.interactable = false;
        SpinStarted?.Invoke();

        // Decide first, then animate the wheel so the winning slice ends under the pointer.
        int sliceCount = wheelData.Slices.Count;
        int winningIndex = UnityEngine.Random.Range(0, sliceCount);
        winningSlice = wheelData.Slices[winningIndex];

        float anglePerSlice = 360f / sliceCount;
        float targetAngle = winningIndex * anglePerSlice - fullTurns * 360f;

        wheel.DORotate(new Vector3(0f, 0f, targetAngle), spinDuration, RotateMode.FastBeyond360)
            .SetEase(spinEase)
            .OnComplete(OnSpinFinished);
    }

    private void OnSpinFinished()
    {
        IsSpinning = false;
        spinButton.interactable = true;
        SpinCompleted?.Invoke(winningSlice);
    }
}
