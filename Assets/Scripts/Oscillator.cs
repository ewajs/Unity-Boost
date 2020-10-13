using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only allow to be added once per game object
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField]
    Vector3 movementVector;

    [Range(0, 5)]
    [SerializeField]
    float period = 2f;

    Vector3 startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Protect against divide by zero (leave oscillator static)
        if (period <= Mathf.Epsilon) { return; }
        Vector3 offset = movementVector * Mathf.Sin(Time.time * 2 * Mathf.PI / period);
        transform.position = startingPosition + offset;
    }
}
