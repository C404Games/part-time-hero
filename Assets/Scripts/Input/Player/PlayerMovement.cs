﻿using System.Collections;
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
    public Animator animator;

    private AudioSource audioSource;

    //Controls controls;

    Vector3 velocity;
    

    Rigidbody rb;
    NavMeshAgent nvAgent;

    Vector3 destination;

    MonsterController monsterInReach;


    private void Awake()
    {
        //controls = new Controls();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        nvAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        velocity = new Vector2(0, 0);
    }

    private void OnEnable()
    {
        //controls.Gameplay.Movement.performed += c => OnMove(c);
        //controls.Gameplay.Movement.started += c => OnMove(c);
        //controls.Gameplay.Movement.canceled += c => OnMove(c);
        //controls.Gameplay.MouseClick.performed += c => onClick(c);
        //controls.Gameplay.Action.performed += c => onAction(c);
        //controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        //controls.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        //Vector2 dx = velocity * Time.deltaTime;
        //transform.position += new Vector3(dx.y, 0.0f, -dx.x);
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

    public void onAction(InputAction.CallbackContext context)
    {
        if (monsterInReach != null)
            attack();
        // Otras acciones dependiento del contexto
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


}
