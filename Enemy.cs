using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float attackCooldown;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float maxHealth;
    private float health;

    Animator anim;

    float attackCooldownTime;

    PlayerController player;
    NavMeshAgent agent;

    Vector3 prevPosition = Vector3.zero;

    Vector3 moveDirection;
    Rigidbody rb;

    float timer = 0f;

    [SerializeField]
    float pushTime = 1f;

    Collider myCollider;

    enum State
    {
        Navigation,
        Physics
    }

    State currentState = State.Navigation;

    AudioSource enemyAudioSource;
    public AudioClip legoClip;
    public AudioClip ouchClip;

    public AudioSource pillowHitAS;
    public AudioClip[] pillowHitClips;

    //public AudioSource enemyDeathAS;
    //public AudioClip[] enemyDeathClips;

    public static bool mouseClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        prevPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        health = maxHealth;
        enemyAudioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(health);

        //movement
        Vector3 lookDir = transform.position - prevPosition;
        lookDir.y = 0;
        Quaternion lookRotation;
        if (lookDir != Vector3.zero)
        {
            lookRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = lookRotation;
        }

        prevPosition = transform.position;

        if (currentState == State.Navigation)
        {
            agent.SetDestination(player.transform.position);
            anim.SetBool("isWalking", true);
            agent.GetComponent<NavMeshAgent>().speed = 3f;
        }

        //attack
        attackCooldownTime = Mathf.Max(0, attackCooldownTime - Time.deltaTime);
        if ((transform.position - player.transform.position).magnitude < attackRange && attackCooldownTime == 0)
        {
            player.Damage(attackDamage);
            anim.SetTrigger("Attack");
            attackCooldownTime = attackCooldown;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SwitchToNavigation();
            }
        }

        if(health <= 0)
        {
            //Debug.Log("destroyed enemy");
            //PlayRandomDeathClip();
            //StartCoroutine(Wait());
            GlobalVariables.enemyDied = true;
            Destroy(this.gameObject);
            UIManager.instance.AddScore();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collide with Pillow
        if (other.gameObject.CompareTag("Pillow"))
        {
            //if (mouseClicked == true)
            //{
                SwitchToPhysics();
                timer = pushTime;
                pillowHitAS.volume = 0.3f;
                PlayRandomClip();
                moveDirection = (rb.transform.position - other.transform.position).normalized + new Vector3(0, 0.25f, 0);
                rb.AddForce(moveDirection.normalized * 2f, ForceMode.Impulse);
                health -= 2;
            //} 
        }

        // Collide with Lego
        if (other.gameObject.CompareTag("Lego"))
        {
            enemyAudioSource.PlayOneShot(legoClip);
            enemyAudioSource.PlayOneShot(ouchClip);
            SwitchToPhysics();
            timer = pushTime;
            moveDirection = (rb.transform.position - other.transform.position).normalized + new Vector3(0, 0.25f, 0);
            rb.AddForce(moveDirection.normalized * 5f, ForceMode.Impulse);
            health -= 1;
        }

        // Collide with toy (if thrown)
        //if ((other.gameObject.CompareTag("Toy") && other.gameObject.GetComponent<PickUpObject>().hasBeenThrown))

        //{

        //    moveDirection = (rb.transform.position - other.transform.position).normalized + new Vector3(0, 0.25f, 0);
        //    rb.AddForce(moveDirection.normalized * 20f, ForceMode.Impulse);
        //    //if (other.gameObject.CompareTag("Toy"))
        //    //{
        //    //    enemyAudioSource.volume = 0.3f;
        //    //    enemyAudioSource.PlayOneShot(toyPunchClip);
        //    //    other.transform.position -= moveDirection.normalized * 0.5f;
        //    //    other.GetComponent<Rigidbody>().AddForce(-moveDirection.normalized * 10f, ForceMode.Impulse);
        //    //}

        //    health -= 1;

        //}
    }

    void SwitchToNavigation()
    {
        currentState = State.Navigation;
        agent.enabled = true;
        rb.isKinematic = true;
        agent.Warp(transform.position);
        myCollider.isTrigger = true;
    }

    void SwitchToPhysics()
    {
        currentState = State.Physics;
        agent.enabled = false;
        rb.isKinematic = false;
        myCollider.isTrigger = false;
    }

    void PlayRandomClip()
    {
        pillowHitAS.clip = pillowHitClips[Random.Range(0, pillowHitClips.Length)];
        pillowHitAS.Play();
    }

    //void PlayRandomDeathClip()
    //{
    //    enemyDeathAS.clip = enemyDeathClips[Random.Range(0, enemyDeathClips.Length)];
    //    enemyDeathAS.Play();
    //}

    //IEnumerator Wait()
    //{
    //    yield return new WaitForSeconds(5);
    //}

}