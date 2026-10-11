using UnityEngine;

public class LifecycleTest : MonoBehaviour
{
    private int hasLoggedUpdate = 0;

    void Awake()
    {
        Debug.Log(name + ": Awake");
    }

    void Start()
    {
        Debug.Log(name + ": Start");
    }

    void Update()
    {
        if (hasLoggedUpdate < 3)
        {
            Debug.Log(name + ": first Update");
            hasLoggedUpdate ++;
        }
    }
}