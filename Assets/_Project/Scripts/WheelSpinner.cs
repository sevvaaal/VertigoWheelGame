using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WheelSpinner : MonoBehaviour
{
    [SerializeField] private Button spinButton;
    [SerializeField] private RectTransform wheel;
    [SerializeField] private float spinDuration = 3f;
    [SerializeField] private int fullTurns = 5;
    [SerializeField] private Ease spinEase = Ease.OutCubic;

    private void Awake()
    {
        spinButton.onClick.AddListener(Spin);
    }

    private void Spin()
    {
        spinButton.interactable = false;

        float extraAngle = Random.Range(0f, 360f);
        float totalAngle = fullTurns * 360f + extraAngle;

        wheel.DORotate(new Vector3(0f, 0f, -totalAngle), spinDuration, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(spinEase)
            .OnComplete(OnSpinFinished);
    }

    private void OnSpinFinished()
    {
        spinButton.interactable = true;
    }
}