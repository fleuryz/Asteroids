using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public GameObject Star;
    
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidBody;
    public BoxCollider2D boxCollider;

    int life;
    int size;

    public void StartMeteor(int size)
    {
        this.size = size;
        spriteRenderer.sprite = FileManager.instance.GetMeteorSprite(size);
        switch (size)
        {
            case 2:
                life = 10;
                boxCollider.size = new Vector2(0.85f, 0.85f);
                break;
            case 1:
                life = 5;
                boxCollider.size = new Vector2(0.4f, 0.4f);
                break;
            case 0:
                life = 1;
                boxCollider.size = new Vector2(0.25f, 0.25f);
                break;
            default:
                life = 1;
                boxCollider.size = new Vector2(0.15f, 0.15f);
                break;
        }
        transform.rotation = new Quaternion(0f, 0f, Random.Range(0f,1f) , 1);

        rigidBody.angularVelocity = Random.Range(0f, 10f);
    }

    public void StartNewMeteor()
    {
        StartMeteor(2);

        //Decide in which wall it will be spawed
        float fast_main_axis = 2; //Speed of the meteor moving from one wall to the other
        float slow_main_axis = 1;
        float aux_axis = 1;
        int wall = Random.Range(0, 4);
        switch (wall)
        {
            case 0:     //Left
                transform.position = new Vector3(-10f, Random.Range(-6f, 6f), 0f);
                rigidBody.velocity = new Vector3(Random.Range(slow_main_axis, fast_main_axis), Random.Range(-aux_axis, aux_axis), 0f);
                break;
            case 1:     //Right
                transform.position = new Vector3(10f, Random.Range(-6f, 6f), 0f);
                rigidBody.velocity = new Vector3(Random.Range(-slow_main_axis, -fast_main_axis), Random.Range(-aux_axis, aux_axis), 0f);
                break;
            case 2:     //Top
                transform.position = new Vector3(Random.Range(-10f, 10f), 6f, 0f);
                rigidBody.velocity = new Vector3(Random.Range(-aux_axis, aux_axis), Random.Range(-slow_main_axis, -fast_main_axis), 0f);
                break;
            case 3:     //Bottom
                transform.position = new Vector3(Random.Range(-10f, 10f), -6f, 0f);
                rigidBody.velocity = new Vector3(Random.Range(-aux_axis, aux_axis), Random.Range(slow_main_axis, fast_main_axis), 0f);
                break;
            default:
                break;
        }


        
        
    }

    public void StartFragmentMeteor(int size, bool upper, Vector2 velocity)
    {
        StartMeteor(size);

        rigidBody.angularVelocity = upper ? -20f : 20f;
        rigidBody.velocity = upper ?  velocity * 1.5f + new Vector2(0f, 1f) : velocity * 2 + new Vector2(0f, -1f);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Ship.instance.GotHit();
            //Destroy(this.gameObject);
            //collision.gameObject.SendMessage("ApplyDamage", 10);
        }else if(collision.gameObject.tag == "Meteor")
        {
        }
    }

    public void GetHit(Vector2 hitPoint, int strength = 1)
    {
        life -= strength;
        if(life <= 0)
        {
            if (size > 0)
            {
                GameObject newMeteor1 = Instantiate(GameManager.instance.Meteor_Prefab, transform.position, transform.rotation, GameManager.instance.MainGame);
                GameObject newMeteor2 = Instantiate(GameManager.instance.Meteor_Prefab, transform.position, transform.rotation, GameManager.instance.MainGame);

                newMeteor1.GetComponent<Meteor>().StartFragmentMeteor(size - 1, true, rigidBody.velocity);
                newMeteor2.GetComponent<Meteor>().StartFragmentMeteor(size - 1, false, rigidBody.velocity);

                GameManager.instance.AddMeteorCount(2);
            }
            GameManager.instance.RemoveMeteorCount(size);
            CheckStars();
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 10)
            transform.position = new Vector3(-10f, transform.position.y, transform.position.z);
        else if (transform.position.x < -10)
            transform.position = new Vector3(10f, transform.position.y, transform.position.z);
        if (transform.position.y > 6)
            transform.position = new Vector3(transform.position.x, -6f, transform.position.z);
        else if (transform.position.y < -6)
            transform.position = new Vector3(transform.position.x, 6f, transform.position.z);

    }

    void CheckStars()
    {
        int star;
        double goldStar = 0.02 / (3 - size);
        double silverStar = 0.1 / (3 - size);
        double bronzeStar = 0.5 / (3 - size);
        double chance = Random.value;
        if(chance <= goldStar)
        {
            star = 0; 
        }else if(chance <= silverStar){
            star = 1;
        }else if(chance <= bronzeStar)
        {
            star = 2;
        }
        else
        {
            return;
        }

        Instantiate(Star, transform.position, transform.rotation, GameManager.instance.MainGame).GetComponent<Star>().StartStar(star);

    }
}
