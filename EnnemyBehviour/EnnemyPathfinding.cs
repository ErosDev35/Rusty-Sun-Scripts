using UnityEngine;
using UnityEngine.AI;

public class EnnemyPathfinding : MonoBehaviour
{
    public Ennemy ennemy;
    public Transform player;
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public Vector3 objectivePosition;
    public float timeAtSameObjective;
    public float timeToBeBored = 10;
    public float walkingSpeed = 4;
    public float runningSpeed = 10;
    public bool isBlind = false;
    Vector3 wanderPosition = Vector3.zero;
    public void Wander()
    {
        if(navMeshAgent.remainingDistance < 20)
        {
            timeAtSameObjective = 0;
            Vector3 randDirection = Random.insideUnitSphere * 100;
            randDirection += transform.position;

            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, 100, 1 << NavMesh.GetAreaFromName("Walkable"));

            wanderPosition = navHit.position;
            print(Vector3.Angle(transform.position, wanderPosition));

            objectivePosition = wanderPosition;
        }

        GoToDestination(walkingSpeed, objectivePosition);

        if(timeAtSameObjective > timeToBeBored)
        {
            ennemy.ennemyState = EnnemyState.SLEEPING;
        }
    }
    public void Inspect(Vector3 inspectPoint)
    {
        GoToDestination(walkingSpeed, inspectPoint);
        if(timeAtSameObjective > 10)
        {
            ennemy.ennemyState = EnnemyState.WANDERING;
        }
    }
    public void Chase()
    {
        GoToDestination(runningSpeed, objectivePosition);

        if(timeAtSameObjective > timeToBeBored)
        {
            ennemy.ennemyState = EnnemyState.ALERT;
            ennemy.suspectPoint = objectivePosition;
        }
    }
    void GoToDestination(float speed, Vector3 checkpoint)
    {
        timeAtSameObjective += (objectivePosition == checkpoint)? Time.deltaTime : -timeAtSameObjective;
        
        objectivePosition = checkpoint;
        navMeshAgent.destination = objectivePosition;
        navMeshAgent.speed = speed;
    }
    void Start()
    {
        ennemy = GetComponent<Ennemy>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
    }
    public void Sleeping()
    {
        navMeshAgent.speed = 0;
        animator.Play("Sleeping");
    }
}
