using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    Transform player;

    State currentState;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("FPSController").GetComponent<Transform>();

        currentState = new Idle(gameObject, agent, animator, player);
    }

    private void Update()
    {
        currentState = currentState.Process();
        Debug.Log(currentState);
    }
}
