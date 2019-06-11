using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLocation : MonoBehaviour
{
    private Animator parentAnimator;
    
    
    // Start is called before the first frame update
    void Start()
    {
        parentAnimator = GetComponentInParent<Animator>();
        
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(FindObjectOfType<Player>().GetInvincibility()==false){
            GetComponentInParent<EnemyMovement>().IsAttacked();
            GetComponent<Collider2D>().enabled = false;
            parentAnimator.Play("EnemyDeath");
            Destroy(transform.parent.gameObject,0.33f);
            Debug.Log("Player Touches Enemy! "+ other.gameObject.name.ToString());
        }              
    }
}
