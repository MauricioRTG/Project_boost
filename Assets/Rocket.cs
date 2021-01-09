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

    enum State { Alive, Dying, Transcending};
    State CurrentState = State.Alive;
    bool collisionsEnabled = true;


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
        if(Debug.isDebugBuild)
        {
           RespondToDebugKeys();
        } 
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled; // is going to change to the other state 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(CurrentState != State.Alive || !collisionsEnabled) { return; } //Ignore collision when dead 

        switch (collision.gameObject.tag){
            case "Friendly":
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (SceneManager.sceneCountInBuildSettings == nextSceneIndex)
        {
            nextSceneIndex = 0;
        }
        
        SceneManager.LoadScene(nextSceneIndex); 
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
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        rocketThrustaudio.Stop();
        mainEngineParticles.Stop();
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
        myrigidbody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
           transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

    }
}
