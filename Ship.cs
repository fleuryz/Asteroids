using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public static Ship instance;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidBody;
    public Animator animator;

    public GameObject simpleShot, missileShot, shield_H;

    private int lifes, strength, defense, speed, shield, energy, missiles;
    private bool recovering = false;
    public bool dead = false;
    private bool shieldActive = false;

    // Start is called before the first frame update
    public void StartShip(int energy, int missiles)
    {
        instance = this;
        dead = false;
        recovering = false;

        lifes = 3;
        strength = 1;
        defense = 1;
        speed = 1;
        shield = 99;

        this.energy = energy;
        this.missiles = missiles;

        shield_H = Instantiate(shield_H, transform.position, transform.rotation, transform);
        shield_H.SetActive(false);

        GameManager.instance.UpdateLifes(lifes);
        GameManager.instance.UpdateMissiles(missiles);
        GameManager.instance.UpdateEnergy(energy);
        GameManager.instance.UpdateShield(shield);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead || GameManager.instance.isPaused)
            return;
        Movement();
        if (Input.GetButtonDown("Fire1"))
        {
            StartCharging();
        }else if (Input.GetButtonUp("Fire1"))
        {
            ShootBasic();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if(missiles > 0)
                ShootMissile();
        }
        if (Input.GetButtonDown("Shield"))
        {
            ActivateShield();
        }else if (Input.GetButtonUp("Shield"))
        {
            DeactivateShield();
        }
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.instance.Pause(true);
        }
    }

    void Movement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        transform.position += (movement * Time.deltaTime * speed)*3;
        if (transform.position.x > 9.3)
            transform.position = new Vector3(-9.3f, transform.position.y, transform.position.z);
        else if (transform.position.x < -9.3)
            transform.position = new Vector3(9.3f, transform.position.y, transform.position.z);
        if (transform.position.y > 5.3)
            transform.position = new Vector3(transform.position.x, -5.3f, transform.position.z);
        else if (transform.position.y < -5.3)
            transform.position = new Vector3(transform.position.x, 5.3f, transform.position.z);

        //Make the ship look at the mouse
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90);

    }

    public void GotHit()
    {
        if (recovering || shieldActive || dead)
            return;
        lifes--;
        GameManager.instance.UpdateLifes(lifes);
        if (lifes <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(Recover());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ship")
        {
            //collision.gameObject.SendMessage("ApplyDamage", 10);
        }
    }

    void StartCharging()
    {
    }

    void ShootBasic()
    {
        AudioManager.instance.PlaySound(AudioID.LASER);

        Vector2 towards = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //Where the shiping will shot at
        towards.Normalize();

        Instantiate(simpleShot, transform.position, transform.rotation, GameManager.instance.MainGame).GetComponent<Shot>().StartShooting(towards);
    }

    void ShootMissile()
    {
        AudioManager.instance.PlaySound(AudioID.MISSILE);

        Vector2 towards = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //Where the shiping will shot at
        towards.Normalize();

        Instantiate(missileShot, transform.position, transform.rotation, GameManager.instance.MainGame).GetComponent<Missile>().StartShooting(towards);
        missiles--;
        GameManager.instance.UpdateMissiles(missiles);
        
    }

    void ActivateShield()
    {
        AudioManager.instance.PlaySound(AudioID.SHIELD_ON);
        shieldActive = true;
        shield_H.SetActive(true);
        StartCoroutine(ShieldActive());
    }

    void DeactivateShield()
    {
        AudioManager.instance.PlaySound(AudioID.SHIELD_OFF);
        shieldActive = false;
        shield_H.SetActive(false);
    }

    IEnumerator ShieldActive()
    {
        while (shieldActive)
        {
            shield--;
            if(shield <= 0)
            {
                if (energy > 0)
                {
                    energy--;
                    GameManager.instance.UpdateEnergy(energy);
                    shield = 99;
                }
                else
                {
                    shield = 0;
                    DeactivateShield();
                }
            }
            GameManager.instance.UpdateShield(shield);
            yield return new WaitForSeconds(0.05f);
        }
        
        
    }

    IEnumerator Die()
    {
        dead = true;
        animator.SetTrigger("Die");
        AudioManager.instance.PlaySound(AudioID.SHIP_EXPLOSION); 
        yield return new WaitForSeconds(2f);
        GameManager.instance.EndGame(energy, missiles);
    }

    IEnumerator Recover()
    {
        AudioManager.instance.PlaySound(AudioID.DAMAGE);
        recovering = true;
        if (!animator.GetBool("Recovering"))
            animator.SetBool("Recovering", true);
        yield return new WaitForSeconds(2f);
        recovering = false;
        animator.SetBool("Recovering", false);

    }
}
