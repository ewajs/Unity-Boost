﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField]
    AudioClip mainEngineSound;
    [SerializeField]
    AudioClip transcendingSound;
    [SerializeField]
    AudioClip dyingSound;

    [SerializeField]
    ParticleSystem mainEngineParticle;
    [SerializeField]
    ParticleSystem transcendingParticle;
    [SerializeField]
    ParticleSystem dyingParticle;

    [SerializeField]
    float rcsThrust = 50f;

    [SerializeField]
    float mainThrust = 1000f;

    [SerializeField]
    float levelLoadDelay = 2f;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisionsEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Stop sound
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsEnabled) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("OK");
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(transcendingSound);
                transcendingParticle.Play();
                Invoke("LoadNextScene", levelLoadDelay); // parameterize
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(dyingSound);
                dyingParticle.Play();
                Invoke("LoadStartScene", levelLoadDelay);
                break;
        }
    }

    private void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space)) // Can thurst while rotating
        {
            float thrustSpeed = mainThrust * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * thrustSpeed);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngineSound);
            }
            if (!mainEngineParticle.isPlaying)
            {
                mainEngineParticle.Play();
            }
        }
        else
        {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }

    private void Rotate()
    {
        // Take manual control of rotation
        rigidBody.freezeRotation = true;
        // Rotating input logic
        float rotationSpeed = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
        // Release rotation to physics engine
        rigidBody.freezeRotation = false;
    }

}
