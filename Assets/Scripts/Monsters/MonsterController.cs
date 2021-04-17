using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;

enum MonsterState
{
    ATTACKING,
    WALKING, 
    WAITING
}


public class MonsterController : MonoBehaviour
{

    public GameObject healthBar;

    MonsterState state;

    NavMeshAgent nvAgent;

    StationInstance target;

    double attackTime = 2.0;
    double lastTime = 0.0;

    int health;
    int initHealth = 5;

    Animator animator;

    //ProductManager productManager;

    // Start is called before the first frame update
    void Start()
    {
        state = MonsterState.WAITING;
        nvAgent = GetComponent<NavMeshAgent>();
        health = initHealth;
        healthBar.SetActive(false);
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
                if(stations.Count() <= 0)
                    break;
                int randIdx = Random.Range(0, stations.Count() - 1);
                target = stations.ElementAt(randIdx);
                nvAgent.SetDestination(target.getWaitPos(transform.position));
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
        healthBar.transform.forward = -Camera.main.transform.forward;
    }

    public void takeHealth(int h)
    {
        health -= h;
        if(health <= 0)
        {
            animator.SetBool("Dead", true);
            Destroy(this.gameObject);
        }
        else
        {
            animator.SetTrigger("Hit");
            healthBar.transform.GetChild(0).localScale = new Vector3((float)(health)/ initHealth, 1, 1);
            healthBar.SetActive(true);
            StartCoroutine(hideHealthBar(health));
        }
    }

    public void die()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator hideHealthBar(int lastHealth)
    {
        yield return new WaitForSeconds(2);
        if (health == lastHealth)
        {
            healthBar.SetActive(false);
        }
    }
}
