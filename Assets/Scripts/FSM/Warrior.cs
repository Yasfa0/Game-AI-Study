using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Warrior : MonoBehaviour
{
    protected State idleState;
    protected Animator anim;
    protected NavMeshAgent agent;
    protected State currentState;
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>(); 

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        idleState = new Idle(gameObject, agent, patrolPoints,0);
        idleState.SetPatrolPoints(patrolPoints);
        currentState = new Idle(gameObject, agent,patrolPoints,0);
        currentState.SetPatrolPoints(patrolPoints);
    }

    protected new void Update()
    {
        currentState = currentState.Process();
        Debug.Log(gameObject.name + " " + currentState);
    }
}
