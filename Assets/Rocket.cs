using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 300f;
    [SerializeField] float mainThrust = 100f;

    Rigidbody myrigidbody;
    AudioSource rocketThrustaudio;
    //bool  m_ToggleChange;
    // Start is called before the first frame update
    void Start()
    {
        myrigidbody = GetComponent<Rigidbody>();
        rocketThrustaudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }

    void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag){
            case "Friendly":
                print("OK");
                break;
            default:
                print("Dead");
                break;
        }
    }

    private void Thrust()
    {

        float thrustThisFrame = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space)){
            myrigidbody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!rocketThrustaudio.isPlaying){//So it doesn't layer
                rocketThrustaudio.Play();   
            }
        }
        else{
            rocketThrustaudio.Stop();
        }
    }

    private void Rotate()
    {
        myrigidbody.freezeRotation = true; //manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
           transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        myrigidbody.freezeRotation = false; //resume physics control
    }
}
