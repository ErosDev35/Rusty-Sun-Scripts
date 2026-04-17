using UnityEngine;
using UnityEngine.AI;

public class TentacleCarAI : EnnemyPathfinding
{
    public Ennemy ennemy;
    public Transform player;
    public bool isBlind = false;
    void Start()
    {
        ennemy = GetComponent<Ennemy>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }
    void Update()
    {
        Behaviour();
        GestionAnimation();
    }
    void GestionAnimation()
    {
        if (navMeshAgent)
        {
            animator.SetFloat("walkingSpeed", ((Mathf.Abs(navMeshAgent.velocity.x) + Mathf.Abs(navMeshAgent.velocity.z)) / 5));
        }
    }
    void Behaviour()
    {
        if (oldEnnemyState != ennemyState && oldEnnemyState == EnnemyState.SLEEPING && animator)
        {
            oldEnnemyState = ennemyState;
            animator.Play("Wake Up");
            return;
        }

        if (timesStepWasHeard > 2)
        {
            ennemyState = EnnemyState.CHASING;
            timesStepWasHeard = 0;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Wake Up"))
        {
            switch (ennemyState)
            {
                case EnnemyState.ALERT:
                    Inspect(suspectPoint);
                    break;
                case EnnemyState.WANDERING:
                    Wandering();
                    break;
                case EnnemyState.CHASING:
                    Chase();
                    break;
                case EnnemyState.SLEEPING:
                    Sleep();
                    break;
            }
            oldEnnemyState = ennemyState;
        }
    }
    public void Inspect(Vector3 inspectPoint)
    {
        GoToDestination(walkingSpeed, inspectPoint);
        if (timeAtSameObjective > 10)
        {
            ennemy.ennemyState = EnnemyState.WANDERING;
        }
    }
    public void Chase()
    {
        GoToDestination(runningSpeed, objectivePosition);

        if (timeAtSameObjective > timeToBeBored)
        {
            ennemy.ennemyState = EnnemyState.ALERT;
            ennemy.suspectPoint = objectivePosition;
        }
    }
    public void Sleep()
    {
        Sleeping();
        animator.Play("Sleeping");
    }
    void Wandering()
    {
        Wander();
        if (timeAtSameObjective > timeToBeBored)
        {
            ennemy.ennemyState = EnnemyState.SLEEPING;
        }
    }
}
