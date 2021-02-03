using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidBody;

    public GameObject MissileExplosion;


    // Start is called before the first frame update
    void Start()
    {
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
            Instantiate(MissileExplosion, new Vector3(hitPoint.x, hitPoint.y, 0), Quaternion.identity, GameManager.instance.MainGame);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.tag == "Shoot")
        {
            //Destroy(collision.gameObject);
            //UnityEngine.Debug.Log("Shot hit a shoot");
        }
    }

    public void StartShooting(Vector2 direction)
    {
        int speed = 5;
        Vector2 velocity = new Vector2(speed * direction.x, speed * direction.y);
        rigidBody.velocity = velocity;
        Destroy(gameObject, 4f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
