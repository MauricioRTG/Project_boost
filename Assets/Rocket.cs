using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 300f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip Death;
    [SerializeField] AudioClip Success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem DeathParticles;
    [SerializeField] ParticleSystem SuccessParticles;

    Rigidbody myrigidbody;
    AudioSource rocketThrustaudio;

    enum State { Alive, Dying, Transcending };
    State CurrentState = State.Alive;


    // Start is called before the first frame update
    void Start()
    {
        myrigidbody = GetComponent<Rigidbody>();
        rocketThrustaudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(CurrentState != State.Alive) { return; } //Ignore collision when dead

        switch(collision.gameObject.tag){
            case "Friendly":
                print("OK");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        CurrentState = State.Dying;
        rocketThrustaudio.Stop();
        rocketThrustaudio.PlayOneShot(Death);
        DeathParticles.Play();
        Invoke("RestartLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        CurrentState = State.Transcending;
        rocketThrustaudio.Stop();
        rocketThrustaudio.PlayOneShot(Success);
        SuccessParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay); 
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1); //todo allow for more than two levels
    }

    private void RespondToThrustInput()
    {

        float thrustThisFrame = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust(thrustThisFrame);
        }
        else
        {
            rocketThrustaudio.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust(float thrustThisFrame)
    {
        myrigidbody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!rocketThrustaudio.isPlaying)
        {//So it doesn't layer

            rocketThrustaudio.PlayOneShot(mainEngine);

        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
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
