using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDocile : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float aggroRange = 10f;
    [SerializeField] float jumpSpeed = 3f;
    public Rigidbody2D rb2d;
    public BoxCollider2D sideCollider;
    public CapsuleCollider2D bodyCollider;
    //public CircleCollider2D aggroCollider;
    public Animator anim;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        rb2d=GetComponent<Rigidbody2D>();
        sideCollider=GetComponent<BoxCollider2D>();
        bodyCollider=GetComponent<CapsuleCollider2D>();
        //aggroCollider=GetComponent<CircleCollider2D>();
        anim=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AggroDetection();
        FlipSprite();
        
    }
    
    private void AggroDetection(){
        float distanceFromPlayer = (player.rb2d.position - rb2d.position).magnitude;
        
        float directionFromPlayer = Mathf.Sign((player.rb2d.position.x - rb2d.position.x));
        if(aggroRange>=distanceFromPlayer){
            rb2d.velocity = new Vector2(directionFromPlayer*moveSpeed,0f);
            anim.SetBool("isMoving",true);
        }
        else{
            rb2d.velocity = new Vector2(0f,0f);
            anim.SetBool("isMoving",false);
        }
    }
    private void FlipSprite(){
        
        bool isMovingHorizontal = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        
        if(isMovingHorizontal){
            transform.localScale =new Vector2(Mathf.Sign(rb2d.velocity.x),1f);
        }
    }
}
