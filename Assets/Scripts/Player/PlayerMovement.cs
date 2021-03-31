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
    public float speed = 5.0f;
    public float rotSpeed = 10.0f;
    public bool blocked = false;

    public Animator animator;

    private AudioSource audioSource;

    private Vector3 waitPosition;

    Vector3 velocity;
    

    Rigidbody rb;
    NavMeshAgent nvAgent;

    Vector3 destination;

    MonsterController monsterInReach;


    // Start is called before the first frame update
    void Start()
    {
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

        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, waitPosition, Time.deltaTime * 10);
        }
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

        velocity = (forward * input.y + right * input.x) * speed;        

        nvAgent.isStopped = true;
    }

    public void onMouseClick(InputAction.CallbackContext context)
    {
        if (!blocked) {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag.Equals("Floor"))
                {
                    destination = hit.point;
                    nvAgent.SetDestination(destination);
                    nvAgent.isStopped = false;
                }
            }
        }

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
        animator.SetTrigger("Attack");
        monsterInReach.takeHealth(1);
    }

    public void blockMovement(float time, Vector3 waitPosition)
    {
        blocked = true;
        this.waitPosition = waitPosition;
        StartCoroutine(unlockMovement(time));
    }

    private IEnumerator unlockMovement(float time)
    {
        yield return new WaitForSeconds(time);
        blocked = false;
    }


}
