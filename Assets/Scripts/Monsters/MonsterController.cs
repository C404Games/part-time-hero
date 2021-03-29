using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;

public enum MonsterState
{
    ATTACKING,
    WALKING, 
    WAITING
}


public class MonsterController : MonoBehaviour
{

    MonsterState state;

    NavMeshAgent nvAgent;

    StationInstance target;

    double attackTime = 2.0;
    double lastTime = 0.0;

    int health;

    Animator animator;

    //ProductManager productManager;

    // Start is called before the first frame update
    void Start()
    {
        state = MonsterState.WAITING;
        nvAgent = GetComponent<NavMeshAgent>();
        health = 5;
        animator = GetComponent<Animator>();
        //productManager = FindObjectOfType<ProductManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case MonsterState.WAITING:
                IEnumerable<StationInstance> stations = FindObjectsOfType<StationInstance>().Where(s => s.getHealth() > 0);
                int randIdx = Random.Range(0, stations.Count() - 1);
                target = stations.ElementAt(randIdx);
                nvAgent.SetDestination(target.gameObject.transform.position);
                nvAgent.isStopped = false;
                state = MonsterState.WALKING;
                animator.SetBool("Walking", true);
                break;
            case MonsterState.WALKING:
                if(target == null)
                {
                    animator.SetBool("Walking", false);
                    state = MonsterState.WAITING;
                    break;
                }
                if(!nvAgent.hasPath)
                {
                    animator.SetBool("Walking", false);
                    lastTime = Time.time;
                    nvAgent.isStopped = true;
                    state = MonsterState.ATTACKING;
                }
                break;
            case MonsterState.ATTACKING:
                if(target == null || target.getHealth() <= 0)
                {
                    state = MonsterState.WAITING;
                    target = null;
                }

                if(Time.time - lastTime >= attackTime)
                {
                    animator.SetTrigger("Attack");
                    lastTime = Time.time;
                    target.takeHealth(1);
                }
                break;
        }
    }

    public void takeHealth(int h)
    {
        health -= h;
        if(health <= 0)
        {
            animator.SetBool("Dead", true);
            //Destroy(this.gameObject);
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }
}
