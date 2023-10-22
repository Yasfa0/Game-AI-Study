using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE { Idle, Patrol, Attack}
    public enum EVENT { ENTER, UPDATE, EXIT }

    public STATE name;
    protected EVENT stage;
    protected State nextState;

    protected GameObject npc;
    protected Transform oppTarget;
    //protected Animator anim;
    protected NavMeshAgent agent;

    protected List<Transform> patrolPoints = new List<Transform>();
    protected int patrolIndex = 0;

    float visDist = 15f;
    float visAngle = 30f;

    protected float attackRange = 2f;

    public State(GameObject _npc, NavMeshAgent _agent, List<Transform> _patrolPoints, int _patrolIndex)
    {
        npc = _npc;
        agent = _agent;
        patrolPoints = _patrolPoints;
        patrolIndex = _patrolIndex;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }


    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }

        return this;
    }

    public void SetPatrolPoints(List<Transform> input)
    {
        patrolPoints = input;
    }

    public STATE GetCurrentSTATE()
    {
        return name;
    }

    public bool CanSeeCivilain()
    {
        if (GameObject.FindObjectOfType<CivilainBehaviour>() != null)
        {

            oppTarget = GameObject.FindObjectOfType<CivilainBehaviour>().transform;
            Vector3 direction = oppTarget.position - npc.transform.position;

            float angle = Vector3.Angle(direction, npc.transform.forward);

            if (direction.magnitude < visDist && angle <= visAngle)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanAttack()
    {
        if (GameObject.FindObjectOfType<CivilainBehaviour>() != null)
        {
            oppTarget = GameObject.FindObjectOfType<CivilainBehaviour>().transform;
            Vector3 direction = oppTarget.position - npc.transform.position;

            if (direction.magnitude <= attackRange)
            {
                return true;
            }
        }
        return false;
    }

}

public class Idle : State
{
    float startWait;

    public Idle(GameObject _npc, NavMeshAgent _agent, List<Transform> _patrolPoints, int _patrolIndex) : base(_npc, _agent,_patrolPoints, _patrolIndex)
    {
        name = STATE.Idle;
    }

    public override void Enter()
    {
        base.Enter();
        startWait = Time.time;
    }

    public override void Update()
    {
        if (CanAttack())
        {
            nextState = new Attack(npc, agent,patrolPoints,patrolIndex);
            stage = EVENT.EXIT;
        }

        if(Time.time - startWait > 3)
        {
            Debug.Log("Patrol Index idle: " + patrolIndex);
            nextState = new Patrol(npc,agent,patrolPoints,patrolIndex);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Patrol : State
{

    public Patrol(GameObject _npc, NavMeshAgent _agent, List<Transform> _patrolPoints, int _patrolIndex) : base(_npc, _agent,_patrolPoints, _patrolIndex)
    {
        name = STATE.Patrol;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        agent.SetDestination(patrolPoints[patrolIndex].position);

        if (CanAttack())
        {
            nextState = new Attack(npc, agent,patrolPoints,patrolIndex);
            stage = EVENT.EXIT;
        }

        if (agent.hasPath)
        {
            Vector3 direction = patrolPoints[patrolIndex].position - npc.transform.position;
            if(direction.magnitude < 1)
            {
                Debug.Log("Reached Destination");
                patrolIndex++;
                if (patrolIndex >= patrolPoints.Count)
                {
                    patrolIndex = 0;
                }

                nextState = new Idle(npc, agent,patrolPoints,patrolIndex);
                stage = EVENT.EXIT;
            }
        }

    }

    public override void Exit()
    {
        Debug.Log("Patrol Index Exit: " + patrolIndex);
        base.Exit();
    }
}

public class Attack : State
{

    public Attack(GameObject _npc, NavMeshAgent _agent, List<Transform> _patrolPoints, int _patrolIndex) : base(_npc, _agent,_patrolPoints, _patrolIndex)
    {
        name = STATE.Attack;
    }

    public override void Enter()
    {
        base.Enter();
        oppTarget = GameObject.FindObjectOfType<CivilainBehaviour>().transform;
        oppTarget.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if (!CanAttack())
        {
            nextState = new Idle(npc, agent,patrolPoints,patrolIndex);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}