using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Controls controls;

    Vector2 velocity;
    float speed = 10.0f;

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
        //Vector2 dx = velocity * Time.deltaTime;
        //transform.position += new Vector3(dx.y, 0.0f, -dx.x);
        rb.velocity = new Vector3(velocity.y, 0.0f, -velocity.x); ;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        velocity = moveInput * speed;
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

}
