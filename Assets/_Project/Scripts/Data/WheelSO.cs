using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wheel", menuName = "Vertigo/Wheel")]
public class WheelSO : ScriptableObject
{
    [SerializeField] private Sprite bombIcon;
    [SerializeField] private List<SliceData> slices = new List<SliceData>();

    public Sprite BombIcon => bombIcon;
    public IReadOnlyList<SliceData> Slices => slices;
}