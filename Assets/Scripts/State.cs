using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE 
    {
          IDLE,
          PATROL,
          PURSUE,
          ATTACK,
          SLEEP
    };
    
    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected NavMeshAgent agent;   
    protected Animator anim;
    protected Transform player;
    protected State nextState;

    public float visDist = 15.0f;
    public float visAngle = 60.0f;
    public float shootDist = 10.0f;

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - agent.transform.position;

        float Angle = Vector3.Angle(direction, npc.transform.forward);
        
        if (direction.magnitude < visDist && Angle < visAngle)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - agent.transform.position;

        if (direction.magnitude < shootDist)
        {
            return true;
        }
        return false;
    }

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        player = _player;
    }

    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Update()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Exit()
    {
        stage = EVENT.EXIT;
    }

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
}

public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) 
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE; 
    }

    public override void Enter()
    {
        anim.SetTrigger("isIdle");
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (Random.Range(0, 100) < 10)
        {
            nextState = new Patrol(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}

public class Patrol : State
{
    int currentIndex = -1;
    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PATROL;
        agent.speed = 2;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        currentIndex = 0;
        anim.SetTrigger("isWalking");
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (agent.remainingDistance < 1)
        {
            if (currentIndex >= GameEnvironment.Singleton.CheckPoints.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }

            agent.SetDestination(GameEnvironment.Singleton.CheckPoints[currentIndex].transform.position);
        }

        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
       
    }

    public override void Exit()
    {
        anim.ResetTrigger("isWalking");
        base.Exit();
    }
}

public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;
        agent.speed = 4;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (agent.hasPath)
        {
            if (CanSeePlayer())
            {
                agent.transform.LookAt(player.transform);
                if (CanAttackPlayer())
                {
                    nextState = new Attack(npc, agent, anim, player);
                    stage = EVENT.EXIT;
                }
            }
            else
            {
                nextState = new Patrol(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.ResetTrigger("isRunning");
    }
}

public class Attack : State
{

    AudioSource source;
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    : base(_npc, _agent, _anim, _player)
    {
        agent.isStopped = true;
    }

    public override void Enter()
    {
        anim.SetTrigger("isShooting");
        source = agent.gameObject.GetComponent<AudioSource>();
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        
        if(!source.isPlaying)
            source.Play();

        agent.transform.LookAt(player.transform);

        if (!CanAttackPlayer())
        {
            if (CanSeePlayer())
            {
                nextState = new Pursue(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
            else
            {
                nextState = new Patrol(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }

    }

    public override void Exit()
    {
        anim.ResetTrigger("isShooting");
        base.Exit();
    }


}

