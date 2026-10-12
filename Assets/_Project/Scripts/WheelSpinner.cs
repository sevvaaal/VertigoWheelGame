using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WheelSpinner : MonoBehaviour
{
    [SerializeField] private Button spinButton;
    [SerializeField] private RectTransform wheel;
    [SerializeField] private WheelSO wheelData;                      
    [SerializeField] private float spinDuration = 3f;
    [SerializeField] private int fullTurns = 5;
    [SerializeField] private Ease spinEase = Ease.OutCubic;

    private SliceData winningSlice;                                 

    private void Awake()
    {
        spinButton.onClick.AddListener(Spin);
    }

    private void Spin()
    {
        spinButton.interactable = false;

        int sliceCount = wheelData.Slices.Count;                     
        int winningIndex = Random.Range(0, sliceCount);              
        winningSlice = wheelData.Slices[winningIndex];               

        float anglePerSlice = 360f / sliceCount;                     
        float targetAngle = winningIndex * anglePerSlice - fullTurns * 360f;   

        wheel.DORotate(new Vector3(0f, 0f, targetAngle), spinDuration, RotateMode.FastBeyond360)   
            .SetEase(spinEase)
            .OnComplete(OnSpinFinished);
    }

    private void OnSpinFinished()
    {
        if (winningSlice.IsBomb)                                    
        {
            Debug.Log("BOMB! You lost everything.");
        }
        else
        {
            Debug.Log($"You won {winningSlice.Reward.Type.DisplayName} x{winningSlice.Reward.Amount}!");
        }

        spinButton.interactable = true;
    }
}