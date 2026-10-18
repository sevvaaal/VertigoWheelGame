using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wheel", menuName = "Vertigo/Wheel")]
public class WheelSO : ScriptableObject
{
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Sprite indicatorSprite;
    [SerializeField] private Sprite bombIcon;
    [SerializeField] private List<SliceData> slices = new List<SliceData>();

    // The wheel's own art: bronze, silver or golden.
    public Sprite BaseSprite => baseSprite;
    public Sprite IndicatorSprite => indicatorSprite;
    public Sprite BombIcon => bombIcon;
    public IReadOnlyList<SliceData> Slices => slices;
}
