using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Player_Movement : MonoBehaviour
{
    private bool died;
    private float reductionRate;
    public Animator loadin_Animator;
    public RectTransform loadpannel;
    public RedBlinking redBlinkingRef;
    private bool isBlinking = false;

    // Movement Variables

    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    public float minutesToDecrease = 1f;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public Image healthBarSprite;
    public Image flashBarSprite;

    public string sceneName;

    public GameObject actionPrompt;

    private void Start()
    {
        reductionRate = 0.02f;
        died = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        actionPrompt.SetActive(false);
        readyToJump = true;
        // Set reduction rate based on the desired minutes
        SetFlashlightDecreaseRate(minutesToDecrease);
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, Ground);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        updateHealthBar();
        updateFlashBar();
    }

    private void SetFlashlightDecreaseRate(float minutes)
    {
        float totalSeconds = minutes * 60f;
        reductionRate = 1f / totalSeconds;
    }

    private void updateHealthBar()
    {
        if (flashBarSprite.fillAmount == 0f)
        {
            healthBarSprite.fillAmount -= 0.02f * Time.deltaTime;
            if (!isBlinking)
            {
                redBlinkingRef.StartBlinking();
                isBlinking = true;
            }
        }
        if (healthBarSprite.fillAmount == 0f && !died)
        {
            if (isBlinking)
            {
                redBlinkingRef.StopBlinking();
            }
            died = true;
            loadin_Animator.SetTrigger("SlideIn");
            Invoke("startLoadingIntro", 1f);
        }
        if (this.gameObject.scene.name == "Forest" && this.transform.position.y <= -10f)
        {
            died = true;
            loadin_Animator.SetTrigger("SlideIn");
            Invoke("startLoadingIntro", 1f);
        }
    }
    private void startLoadingIntro()
    {
        loadpannel.position = new Vector3(0, loadpannel.position.y, loadpannel.position.z);
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameOver")
        {
            GameObject temp = GameObject.Find("EventSystemGameOver");
            if (temp != null)
            {
                GameOver gameover = temp.GetComponent<GameOver>();
                gameover.sceneName = sceneName;
                gameover.camera.SetActive(true);
                gameover.canvas.SetActive(true);
                gameover.loadin_Animator.SetTrigger("SlideOut");
                SceneManager.UnloadScene(sceneName);
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after handling
    }

    private void updateFlashBar()
    {
        flashBarSprite.fillAmount -= reductionRate * Time.deltaTime;
    }

    public void increaseFlash()
    {
        redBlinkingRef.StopBlinking();
        flashBarSprite.fillAmount = 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            updateHealthBar();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Artifact") || other.CompareTag("Key") || other.CompareTag("Flash"))
        {
            actionPrompt.SetActive(true);
        }
        
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Artifact") || other.CompareTag("Key") || other.CompareTag("Flash"))
        {
            actionPrompt.SetActive(false);
        }
    }
}