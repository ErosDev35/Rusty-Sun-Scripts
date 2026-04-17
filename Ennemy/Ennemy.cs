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
    public Animator animator;
    public Vector3 suspectPoint;
    public int timesStepWasHeard = 0;
    public GameObject bloodParticleContact;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (health <= 0) Die();
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
    public void SoundReact(Vector3 suspectPosition)
    {
        ennemyState = EnnemyState.ALERT;
        this.suspectPoint = suspectPosition;
        timesStepWasHeard += 1;
    }
}
