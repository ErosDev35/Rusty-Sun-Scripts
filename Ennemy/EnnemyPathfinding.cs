using UnityEngine;
using UnityEngine.AI;

public class EnnemyPathfinding : Ennemy
{
    public float timeAtSameObjective;
    public Vector3 objectivePosition;
    public NavMeshAgent navMeshAgent;
    Vector3 wanderPosition = Vector3.zero;
    public float timeToBeBored = 10;
    public float walkingSpeed = 4;
    public float runningSpeed = 10;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    public void GoToDestination(float speed, Vector3 checkpoint)
    {
        timeAtSameObjective += (objectivePosition == checkpoint) ? Time.deltaTime : -timeAtSameObjective;

        objectivePosition = checkpoint;
        navMeshAgent.destination = objectivePosition;
        navMeshAgent.speed = speed;
    }
    public void Wander()
    {
        if (navMeshAgent.remainingDistance < 20)
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
    }
    public void Sleeping()
    {
        navMeshAgent.speed = 0;
    }
}
