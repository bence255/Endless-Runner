using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    GameManager gameManager;
    Rigidbody rigidBody;
    Animator animator;
    int isJumpingHash;
    int xDirectionHash;
    int isRunningHash;
    private float playerSpeed = 4;
    private float translatespeed = 4;
    private int currentLane = 0;
    private float laneDistance;
    private float jumpTime = 0f;
    private float jumpResetTime = 2f;
    private bool onGround;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        laneDistance = gameManager.GetLaneDistance();
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isJumpingHash = Animator.StringToHash("isJumping");
        xDirectionHash = Animator.StringToHash("xDirection");
        isRunningHash = Animator.StringToHash("isRunning");
        gameObject.GetComponentInChildren<ParticleSystem>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.01f) onGround = true;
        else onGround = false;

        if (Input.GetKeyDown(KeyCode.A) && currentLane > -2)
        {
            Debug.Log("SwitchLeft: " + currentLane);
            currentLane -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
        {
            Debug.Log("SwitchRight: " + currentLane);
            currentLane += 1;
        }

        MovePlayer(isMoving);

    }

    private void MovePlayer(bool isMoving)
    {
        if (isMoving)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed);

            if (Input.GetKeyDown(KeyCode.Space) && onGround && Time.time > jumpTime + jumpResetTime)
            {
                Debug.Log("Jump");
                rigidBody.AddForce(transform.up * 300);
                animator.SetBool(isJumpingHash, true);
                jumpTime = Time.time;
            }

            if (onGround && Time.time > jumpTime + 0.2f)
            {
                animator.SetBool(isJumpingHash, false);
            }

            if (true) LateralMovePlayer();
        }
    }

    private void LateralMovePlayer()
    {
        if (transform.position.x > laneDistance * currentLane)
        {
            transform.Translate(Vector3.left * Time.deltaTime * translatespeed);
        }
        else if (transform.position.x < laneDistance * currentLane)
        {
            transform.Translate(Vector3.right * Time.deltaTime * translatespeed);
        }
    }

    public void SetMovement(bool isSet)
    {
        if (isSet)
        {
            animator.SetBool(isRunningHash, true);
        }
        else
        {
            animator.SetBool(isRunningHash, false);
        }
        isMoving = isSet;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin")
        {
            if (transform.position.z - other.transform.position.z < 10)
            {
                Debug.Log("Coin");
                gameManager.AddCoin();
            }
            Destroy(other.gameObject);
        }
        if (other.tag == "Obsticle")
        {
            if (transform.position.z - other.transform.position.z < 10)
            {
                Debug.Log("Obsticle");
                gameObject.GetComponentInChildren<ParticleSystem>().Play();
                gameManager.RemoveLife();
            }
        }
        if (other.tag == "Illusion")
        {
            Debug.Log("Illusion");
            Destroy(other.gameObject);
        }
    }
    public void SetJumpResetTime(float time)
    {
        jumpResetTime = time;
    }

    public void SetPlayerSpeed(float speed)
    {
        playerSpeed = speed;
    }
}
