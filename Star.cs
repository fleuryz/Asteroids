using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;

    int type;
    int[] starsCount = new int[] { 100, 10, 1 };

    public void StartStar(int type)
    {
        this.type = type;
        spriteRenderer.sprite = FileManager.instance.GetStarSprite(type);
        StartCoroutine(SelfDestroy());
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.instance.AddStars(starsCount[type]);
            Destroy(this.gameObject);
        }
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
