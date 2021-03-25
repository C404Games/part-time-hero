using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltScript : MonoBehaviour
{
    public float speed = 1;
    public float rotate = 0;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    float incremental = 0;
    private void FixedUpdate()
    {
        //Guardamos valores originales.
        Vector3 pos = rb.position;
        Quaternion rot = rb.rotation;

        //Actualizamos posicion de los objetos movibles.
        Vector3 fatherRotation = transform.parent.rotation.eulerAngles;
        rb.position += new Vector3(Mathf.Cos(Mathf.PI / 180 * fatherRotation.y), 0, -Mathf.Sin(Mathf.PI / 180 * fatherRotation.y)) * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);


        //NO FUNCIONAL, SE DEJA COMENTADO PARA NO PERDER EL CÓDIGO POR SI LUEGO SE REANUDA.
        //Actualizamos la rotacion de los objetos movibles.
        //rBody.rotation = Quaternion.Euler(0, rotate, 0);
        //rb.MoveRotation(rot);
    }
}
