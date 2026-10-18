using System;
using DG.Tweening;
using UnityEngine;

// Spins the wheel and announces which slice it landed on. It has no button and decides nothing about the result:
// the GameController asks it to spin and decides what the landed slice means.
public class WheelSpinner : MonoBehaviour
{
    [SerializeField] private RectTransform wheel;
    [SerializeField] private float spinDuration = 3f;
    [SerializeField] private int fullTurns = 5;
    [SerializeField] private Ease spinEase = Ease.OutCubic;

    private WheelSO wheelData;
    private SliceData winningSlice;

    public event Action SpinStarted;
    public event Action<SliceData> SpinCompleted;

    public bool IsSpinning { get; private set; }

    // Which slice the last spin landed on, so the view can highlight it.
    public int WinningIndex { get; private set; }

    // The wheel changes with the zone (bronze, silver, golden), so the winner is always picked from the current one.
    public void SetWheel(WheelSO wheel)
    {
        wheelData = wheel;
    }

    public void Spin()
    {
        IsSpinning = true;
        SpinStarted?.Invoke();

        // Decide first, then animate the wheel so the winning slice ends under the pointer.
        int sliceCount = wheelData.Slices.Count;
        WinningIndex = UnityEngine.Random.Range(0, sliceCount);
        winningSlice = wheelData.Slices[WinningIndex];

        float anglePerSlice = 360f / sliceCount;
        float targetAngle = WinningIndex * anglePerSlice - fullTurns * 360f;

        wheel.DORotate(new Vector3(0f, 0f, targetAngle), spinDuration, RotateMode.FastBeyond360)
            .SetEase(spinEase)
            .OnComplete(OnSpinFinished);
    }

    private void OnSpinFinished()
    {
        IsSpinning = false;
        SpinCompleted?.Invoke(winningSlice);
    }
}
