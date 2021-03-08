using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickMovement : MonoBehaviour
{

    Controls controls;

    Vector2 destination;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.Gameplay.MouseClick.performed += c => onClick(c);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

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
                rb.MovePosition(destination);
            }
        }
    }
}
