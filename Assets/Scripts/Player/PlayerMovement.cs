using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    public int team = 1;

    public float volume = 0.5f;
    public float speed = 5.0f;
    public float rotSpeed = 10.0f;
    public bool blocked = false;

    public bool increasedSpeed;
    public float speedFactor = 1.0f;
    public float currentTimeOfMaxSpeed;
    public float timeMaxSpeed;

    public bool frozen;
    public float currentTimeOfFrozen;
    public float timeFrozen;

    public Animator animator;

    public GameObject iceCube;

    private Vector3 waitPosition;
    private Vector3 waitRotation;

    Vector3 velocity;
    float acceleration = 15.0f;

    Rigidbody rb;
    NavMeshAgent nvAgent;
    ClickMovement clickMovement;

    MonsterController monsterInReach;
    public bool attackBusy = false;

    RecipieBook recipieBookInReach;

    // Start is called before the first frame update
    void Start()
    {
        increasedSpeed = false;
        nvAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        clickMovement = GetComponent<ClickMovement>();
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
                    if (currentTimeOfMaxSpeed < timeMaxSpeed)
                    {
                        this.currentTimeOfMaxSpeed += Time.deltaTime;
                    }
                    else
                    {
                        increasedSpeed = false;
                        speedFactor = 1.0f;
                        currentTimeOfMaxSpeed = 0;
                    }
                }
                rb.velocity = velocity * speed * speedFactor;
                nvAgent.speed = speed * speedFactor;
                nvAgent.acceleration = acceleration * speedFactor;
                if (velocity.sqrMagnitude != 0 && (!nvAgent.hasPath || nvAgent.velocity.sqrMagnitude == 0f))
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * rotSpeed * speedFactor);
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
                if (currentTimeOfFrozen < timeFrozen)
                {
                    currentTimeOfFrozen += Time.deltaTime;
                }
                else{
                    currentTimeOfFrozen = 0;
                    frozen = false;
                    iceCube.SetActive(false);
                    clickMovement.resume();
                } 

            }
        }
        // SI estamos bloqueados, vamos a la posición de "espera"
        else
        {
            transform.position = Vector3.Lerp(transform.position, waitPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(waitRotation), Time.deltaTime * rotSpeed);
            animator.SetBool("isRunning", false);
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

        velocity = (forward * input.y + right * input.x);        

        nvAgent.isStopped = true;
    }

    public void onAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (monsterInReach != null)
                attack();
            else if (recipieBookInReach != null)
                openCloseBook();                
            // Otras acciones dependiento del contexto
        }
    }


    void OnTriggerEnter(Collider collision)
    {
       
        if (monsterInReach == null && collision.gameObject.tag.Equals("Monster"))
        {
            monsterInReach = collision.GetComponent<MonsterController>();
        }
        if (monsterInReach == null && collision.gameObject.tag.Equals("RecipieBook"))
        {
            recipieBookInReach = collision.GetComponent<RecipieBook>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Monster") && other.GetComponent<MonsterController>() == monsterInReach)
        {
            monsterInReach = null;
        }
        if (other.gameObject.tag.Equals("RecipieBook") && other.GetComponent<RecipieBook>() == recipieBookInReach)
        {
            recipieBookInReach = null;
        }
    }

    public void attack()
    {
        if (!attackBusy)
        {
            animator.SetTrigger("Attack");
            monsterInReach.takeHealth(1);
        }
    }

    public void openCloseBook()
    {
        if (recipieBookInReach.isOpen())
        {
            recipieBookInReach.closeBook();
            blocked = false;
        }
        else
        {
            recipieBookInReach.openBook();
            waitPosition = transform.position;
            waitRotation = transform.forward;
            blocked = true;
        }
    }

    public bool isBookOpen()
    {
        return recipieBookInReach != null && recipieBookInReach.isOpen() && blocked;
    }

    public void blockMovement(float time, Vector3 waitPosition, Vector3 waitRotation)
    {
        blocked = true;
        this.waitPosition = waitPosition;
        this.waitRotation = waitRotation;
        StartCoroutine(unlockMovement(time));
        animator.SetBool("Chop", true);
    }
    /*
    public void repairStation(float time, StationInstance station)
    {
        blockMovement(time, station.getWaitPos(transform.position), station.getWaitRot(transform.position));
        StartCoroutine(repairStationTime(station, time));
    }
    */

    public void freeze(float time)
    {
        frozen = true;
        timeFrozen = time;
        currentTimeOfFrozen = 0;
        iceCube.SetActive(true);
        clickMovement.stop();
    }

    public void increaseSpeed(float factor, float time)
    {
        increasedSpeed = true;
        currentTimeOfMaxSpeed = 0;
        timeMaxSpeed = time;
        speedFactor = factor;
    }

    private IEnumerator unlockMovement(float time)
    {
        yield return new WaitForSeconds(time);
        blocked = false;
        animator.SetBool("Chop", false);
    }
    /*
    private IEnumerator repairStationTime(StationInstance station, float time)
    {
        yield return new WaitForSeconds(time);
        station.repair();
    }
    */
}
