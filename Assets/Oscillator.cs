using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] //Only one of this script in a gameObject
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    [Range(0, 1)] [SerializeField] float movementFactor; // 0 for not moved, 1 for fully moved

    Vector3 StartingPosition; //must be stored for absolute movements

    // Start is called before the first frame update
    void Start()
    {
        StartingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Set movementfactor
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period; //grows continualy from 0
        
        

        const float tau = Mathf.PI * 2; //about 6.28 a full circle in radians
        float rawSinWave = Mathf.Sin(cycles * tau); // it goes from +1 to -1

        movementFactor = (rawSinWave / 2f) + 0.5f; // In order to go from 0 to 1; First dividing by 2 meaning it goes from -0.5 to 0.5 then adding to 0.5 

        //Set offset
        Vector3 offset = movementVector * movementFactor;
        transform.position = offset + StartingPosition;
        
    }
}
