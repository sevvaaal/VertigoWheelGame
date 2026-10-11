using UnityEngine;

public class WheelSpinTest : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 360f;
    [SerializeField] private float decleration = 60f; 
    void Update()
    {
        if (degreesPerSecond > 0)

        {
            transform.Rotate(0f, 0f, -degreesPerSecond * Time.deltaTime); // time.deltaTime ile çarpmak per frame'den per second'a geçiriyor.
            degreesPerSecond -= decleration * Time.deltaTime;
        }

    }
}