using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float health = 100;
    private float punctuation = 0;
    public float volume = 0.5f;
    public float powerUpIncreaseValueHealth = 5.0f;
    public float powerUpIncreaseValuePunctuation = 5.0f;
    public float speed = 5.0f;
    public float rotSpeed = 10.0f;
    public Animator animator;
    public int matchId;
    public float frozenTime;
    private bool isFrozen;
    private bool melting;
    private float timer;

    private AudioSource audioSource;

    Controls controls;

    Vector3 velocity;
    

    Rigidbody rb;
    NavMeshAgent agent;

    Vector3 destination;



    private void Awake()
    {
        controls = new Controls();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        velocity = new Vector2(0, 0);
    }

    private void OnEnable()
    {
        controls.Gameplay.Move.performed += c => OnMove(c);
        controls.Gameplay.Move.started += c => OnMove(c);
        controls.Gameplay.Move.canceled += c => OnMove(c);
        controls.Gameplay.MouseClick.performed += c => onClick(c);
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (isFrozen || melting) {
            isFrozen = false;
            if (timer > frozenTime)
            {
                melting = false;
            }
        } else {
            //Vector2 dx = velocity * Time.deltaTime;
            //transform.position += new Vector3(dx.y, 0.0f, -dx.x);
            rb.velocity = velocity;
            if (velocity.sqrMagnitude != 0 && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * rotSpeed);
            if ((!agent.hasPath || agent.velocity.sqrMagnitude == 0f) && rb.velocity == new Vector3(0, 0, 0))
            {
                animator.SetBool("isRunning", false);
            }
            else
            {
                animator.SetBool("isRunning", true);
            }
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector3 input = context.ReadValue<Vector2>();
        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        velocity = (forward * input.y + right * input.x) * speed;        

        agent.isStopped = true;
    }

    public void onClick(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(controls.Gameplay.MousePosition.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag.Equals("Floor"))
            {
                destination = hit.point;
                agent.SetDestination(destination);
                agent.isStopped = false;
            }
        }
    }


    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.gameObject.tag == "PowerUp")
        {
            audioSource.Play();
            if (collision.gameObject.GetComponent<PowerUpBehaviour>() != null)
            {
                switch (collision.gameObject.GetComponent<PowerUpBehaviour>().type)
                {
                    case "health":
                        {
                            this.PowerUpHealth(powerUpIncreaseValueHealth);
                            break;
                        }
                    case "money":
                        {
                            this.PowerUpPunctuation(powerUpIncreaseValuePunctuation);
                            break;
                        }
                    case "frozen":
                        {
                            this.isFrozen = true;
                            break;
                        }
                }
            }
            Destroy(collision.transform.parent.gameObject);
        }
    }

    public void PowerUpHealth(float value)
    {
        health = (health < 100)? health + value : health;
    }

    public void PowerUpPunctuation(float value)
    {
        punctuation += value;
    }

    public int getMatchId()
    {
        return this.matchId;
    }

    public void setMatchId(int matchId)
    {
        this.matchId = matchId;
    }


    public float getPunctuation()
    {
        return this.punctuation;
    }

    public void setPunctuation(float punctuation)
    {
        this.punctuation = punctuation;
    }

    public float getHealth()
    {
        return this.health;
    }

    public void setHealth(float health)
    {
        this.health = health;
    }

    public bool getIsFrozen()
    {
        return this.isFrozen;
    }

    public void setIsFrozen(bool isFrozen)
    {
        this.isFrozen = isFrozen;
    }
}
