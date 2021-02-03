using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotExplosion : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("Explosion");
        AudioManager.instance.PlaySound(AudioID.LASER_HIT);
        Destroy(gameObject, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
