using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	CharacterController charControl = null;

	[SerializeField]
    float moveSpeed = 5.0f;
	float vSpeed = 0;

	float gravity = 9.81f;	

	public float health = 0;
	public float maxHealth;

	AudioSource playerAS;
	public AudioSource ouchAS;
	public AudioClip[] ouchClips;
	public AudioClip candyPickUpClip;
	public AudioClip legoClip;

	// Start is called before the first frame update
	void Start()
    {
		health = maxHealth;
		playerAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 moveVector = GetMoveVectorFromInput();

		vSpeed -= gravity * Time.deltaTime;
		moveVector.y = vSpeed * Time.deltaTime;

		charControl.Move(moveVector);
	}

	Vector3 GetMoveVectorFromInput()
	{
		Vector3 forwardVector = Vector3.zero;
		Vector3 rightVector = Vector3.zero;

		if (Input.GetKey(KeyCode.A))
		{
			rightVector = transform.TransformDirection(Vector3.left);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			rightVector = transform.TransformDirection(Vector3.right);
		}

		if (Input.GetKey(KeyCode.W))
		{
			forwardVector = transform.TransformDirection(Vector3.forward);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			forwardVector = transform.TransformDirection(Vector3.back);
		}

		return (forwardVector + rightVector).normalized * moveSpeed * Time.deltaTime;
	}

	public void Damage(float amount)
    {
		health -= amount;

		if(health <= 0)
        {
			GameManager.instance.GameOver();
        }
    }

    /*
    public void OnCollisionEnter(Collision collision)
    {
		/*
		if (collision.gameObject.CompareTag("Candy") && health < 100)
		{
			health += 5;

			if (health > 100)
			{
				health = 100;
			}
		}

		if (collision.gameObject.CompareTag("Lego"))
		{
			Debug.Log("ow");
			health -= 2;
		}
	}
	*/

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
		if (hit.gameObject.CompareTag("Candy") && health < 100)
		{
			playerAS.PlayOneShot(candyPickUpClip);
			health += 5;
			
			if (health > 100)
			{
				health = 100;
			}

			Destroy(hit.gameObject);
			Debug.Log("destroyed candy");
		}

		
		if (hit.gameObject.CompareTag("Lego"))
		{
			PlayRandomClip();
			playerAS.PlayOneShot(legoClip);
			health -= 2;
			StartCoroutine(LegoDelay());
		}
	}

	void PlayRandomClip()
    {
		ouchAS.clip = ouchClips[Random.Range(0, ouchClips.Length)];
		ouchAS.Play();
	}

	IEnumerator LegoDelay()
    {
		yield return new WaitForSeconds(3);
    }
}
