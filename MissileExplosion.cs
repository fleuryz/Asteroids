using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileExplosion : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("Explosion");
        AudioManager.instance.PlaySound(AudioID.LASER_HIT);
        Destroy(gameObject, 0.5f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ship")
        {
            //collision.gameObject.SendMessage("ApplyDamage", 10);
        }
        else if (collision.gameObject.tag == "Meteor")
        {
            Vector2 hitPoint = collision.contacts[0].point;
            collision.gameObject.GetComponent<Meteor>().GetHit(hitPoint,10);
        }
        else if (collision.gameObject.tag == "Shoot")
        {
            //Destroy(collision.gameObject);
            //UnityEngine.Debug.Log("Shot hit a shoot");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
