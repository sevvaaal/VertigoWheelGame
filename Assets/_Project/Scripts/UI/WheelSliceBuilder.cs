using UnityEngine;

public class WheelSliceBuilder : MonoBehaviour
{
    [SerializeField] private WheelSO wheelData;
    [SerializeField] private SliceView slicePrefab;
    [SerializeField] private float sliceRadius = 145f;

    private void Awake()
    {
        BuildSlices();
    }

    private void BuildSlices()
    {
        int sliceCount = wheelData.Slices.Count;
        float anglePerSlice = 360f / sliceCount;

        for (int i = 0; i < sliceCount; i++)
        {
            float angle = i * anglePerSlice;
            float radians = angle * Mathf.Deg2Rad;

            SliceView sliceView = Instantiate(slicePrefab, transform);
            sliceView.transform.localPosition = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0f) * sliceRadius;
            sliceView.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
            sliceView.Setup(wheelData.Slices[i], wheelData.BombIcon);
        }
    }
}