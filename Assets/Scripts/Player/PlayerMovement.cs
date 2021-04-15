using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    public float health = 0;
    public float punctuation = 0;
    public float volume = 0.5f;
    public float powerUpIncreaseValueHealth = 5.0f;
    public float powerUpIncreaseValuePunctuation = 5.0f;
    public float initialSpeed = 5.0f;
    public float currentSpeed;
    public float rotSpeed = 10.0f;
    public bool blocked = false;
    public bool increasedSpeed;
    public float currentTimeOfMaxSpeed;
    public float limitTimeMaxSpeed = 30;
    public bool frozen;
    public float currentTimeOfFrozen;
    public float limitTimeFrozen = 5;

    public Animator animator;

    private AudioSource audioSource;

    private Vector3 waitPosition;
    private Vector3 waitRotation;

    Vector3 velocity;
    

    Rigidbody rb;
    NavMeshAgent nvAgent;

    MonsterController monsterInReach;
    public bool attackBusy = false;


    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = initialSpeed;
        increasedSpeed = false;
        audioSource = GetComponent<AudioSource>();
        nvAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        velocity = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!blocked)
        {
            if (!frozen)
            {
                if (increasedSpeed)
                {
                    if (currentTimeOfMaxSpeed < this.limitTimeMaxSpeed)
                    {
                        currentSpeed *= 2;
                        this.currentTimeOfMaxSpeed += Time.deltaTime;
                    }
                    else
                    {
                        currentSpeed = initialSpeed;
                        increasedSpeed = false;
                        currentTimeOfMaxSpeed = 0;
                    }
                }
                rb.velocity = velocity;
                if (velocity.sqrMagnitude != 0 && (!nvAgent.hasPath || nvAgent.velocity.sqrMagnitude == 0f))
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * rotSpeed);
                if ((!nvAgent.hasPath || nvAgent.velocity.sqrMagnitude == 0f) && rb.velocity == new Vector3(0, 0, 0))
                {
                    animator.SetBool("isRunning", false);
                }
                else
                {
                    animator.SetBool("isRunning", true);
                }
            } else
            {
                if (this.currentTimeOfFrozen < limitTimeFrozen)
                {
                    this.currentTimeOfFrozen += Time.deltaTime;
                }
                else{
                    this.currentTimeOfFrozen = 0;
                    this.frozen = false;
                } 

            }
        }
        // SI estamos bloqueados, vamos a la posición de "espera"
        else
        {
            transform.position = Vector3.Lerp(transform.position, waitPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(waitRotation), Time.deltaTime * rotSpeed);
        }
    }    

    public bool isMonsterOnReach(MonsterController monster)
    {
        return monster == monsterInReach;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector3 input = context.ReadValue<Vector2>();
        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        velocity = (forward * input.y + right * input.x) * currentSpeed;        

        nvAgent.isStopped = true;
    }

    public void onAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (monsterInReach != null)
                attack();
            // Otras acciones dependiento del contexto
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
                    case "paralysis":
                        {
                            break;
                        }
                    case "money":
                        {
                            this.PowerUpPunctuation(powerUpIncreaseValuePunctuation);
                            break;
                        }
                    case "speed":
                        {
                            this.increasedSpeed = true;
                            break;
                        }
                    case "health":
                        {
                            this.PowerUpHealth(powerUpIncreaseValueHealth);
                            break;
                        }
                }
            }
            Destroy(collision.transform.parent.gameObject);
        }

        if (monsterInReach == null && collision.gameObject.tag.Equals("Monster"))
        {
            monsterInReach = collision.GetComponent<MonsterController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Monster") && other.GetComponent<MonsterController>() == monsterInReach)
        {
            monsterInReach = null;
        }
    }

    public void PowerUpHealth(float value)
    {
        health += value;
    }

    public void PowerUpPunctuation(float value)
    {
        punctuation += value;
    }    


    public void attack()
    {
        if (!attackBusy)
        {
            attackBusy = true;
            animator.SetTrigger("Attack");
            monsterInReach.takeHealth(1);
        }
    }

    public void blockMovement(float time, Vector3 waitPosition, Vector3 waitRotation)
    {
        blocked = true;
        this.waitPosition = waitPosition;
        this.waitRotation = waitRotation;
        StartCoroutine(unlockMovement(time));
        animator.SetBool("Chop", true);
    }

    private IEnumerator unlockMovement(float time)
    {
        yield return new WaitForSeconds(time);
        blocked = false;
        animator.SetBool("Chop", false);
    }
}
