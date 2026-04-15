using UnityEngine;

public class Ennemy : MonoBehaviour
{
    public string type = "Target";
    public double health = 10;
    public double meleeDamage = 1;
    public double distanceDamage = 1;
    public Transform ennemyParticle;
    public EnnemyState ennemyState = EnnemyState.SLEEPING;
    public EnnemyState oldEnnemyState = EnnemyState.SLEEPING;
    public EnnemyPathfinding ennemyPathfinding;
    public Animator animator;
    public Vector3 suspectPoint;
    public int timesStepWasHeard = 0;
    public GameObject bloodParticleContact;
    void Start()
    {
        ennemyPathfinding = GetComponent<EnnemyPathfinding>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (health <= 0) Die();
        Behaviour();
        GestionAnimation();
    }
    public void shootInteraction(float damage = 0)
    {
        health -= damage;
        print("Touché");
        if (ennemyParticle != null)
        {
            ennemyParticle.gameObject.SetActive(true);
            ennemyParticle.GetComponent<ParticleSystem>().Play();
        }
    }
    public void Die()
    {
        print("am dead");
        Destroy(this.gameObject);
    }
    void GestionAnimation()
    {
        if (ennemyPathfinding)
        {
            animator.SetFloat("walkingSpeed", ((Mathf.Abs(ennemyPathfinding.navMeshAgent.velocity.x) + Mathf.Abs(ennemyPathfinding.navMeshAgent.velocity.z)) / 5));
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

        if (ennemyPathfinding && !animator.GetCurrentAnimatorStateInfo(0).IsName("Wake Up"))
        {
            switch (ennemyState)
            {
                case EnnemyState.ALERT:
                    ennemyPathfinding.Inspect(suspectPoint);
                    break;
                case EnnemyState.WANDERING:
                    ennemyPathfinding.Wander();
                    break;
                case EnnemyState.CHASING:
                    ennemyPathfinding.Chase();
                    break;
                case EnnemyState.SLEEPING:
                    ennemyPathfinding.Sleeping();
                    break;
            }
            oldEnnemyState = ennemyState;
        }
    }
    public void SoundReact(Vector3 suspectPosition)
    {
        ennemyState = EnnemyState.ALERT;
        this.suspectPoint = suspectPosition;
        timesStepWasHeard += 1;
    }
}
