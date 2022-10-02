using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject feathers;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Enemy.mouseClicked = true;
            anim.SetBool("isAttacking", true);

            //feather particles
            GameObject explodePart = Instantiate(feathers);
            explodePart.transform.position = transform.position;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Enemy.mouseClicked = false;
            anim.SetBool("isAttacking", false);
        }
    }
}
