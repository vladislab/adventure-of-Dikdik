using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2 (0f,25f);
    [SerializeField] float invincibleTime = 1.5f;
    [SerializeField] [Range(0f,2f)] float soundVol;
    private int pLive = 3;
    private float hMove;
    private float gravityDefault;
    private bool invincibility = false;
    //Cached Component References
    public Rigidbody2D rb2d;
    public Animator animatorController;
    public BoxCollider2D feetCollider;
    public CapsuleCollider2D bodyCollider;
    public UnityEvent OnLandingAttack;
    public AudioClip jumpSound;
    public AudioClip hitSound;
    

    //public LayerMask layerMask;
    //public Collider2D collider;
    
    //States
    private bool isAlive = true;
    private bool isGrounded;
    private Renderer rend;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d=GetComponent<Rigidbody2D>();
        animatorController=GetComponent<Animator>();
        feetCollider=GetComponent<BoxCollider2D>();
        bodyCollider=GetComponent<CapsuleCollider2D>();
        gravityDefault = rb2d.gravityScale;
        rend = GetComponent<Renderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        pLive = FindObjectOfType<GameSession>().GetPlayerLives();
        if(!isAlive){
            return;
        }
        
        Run();
        FlipSprite();
        Jump();
        ClimbLadder();
        Die();
    }
    private void Run(){
        hMove = Input.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(hMove*runSpeed,rb2d.velocity.y);
        rb2d.velocity= playerVelocity;
        animatorController.SetFloat("Speed",Mathf.Abs(playerVelocity.x));
    }
    private void Jump(){
        //animatorController.SetBool("isJump",!IsGrounded());
        if(Input.GetButtonDown("Jump") && IsGrounded()){
            Vector2 jumpToAdd = new Vector2(0f,jumpForce);
            rb2d.velocity += jumpToAdd;
            AudioSource.PlayClipAtPoint(jumpSound,transform.position,soundVol);
        }
        
        
    }
    private void FlipSprite(){
        
        bool isMovingHorizontal = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        
        if(isMovingHorizontal){
            transform.localScale =new Vector2(Mathf.Sign(rb2d.velocity.x),1f);
        }
    }

    private bool IsGrounded(){
        return feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground","Climbing","Platform"));
    }

    private void ClimbLadder(){
        
        bool isTouchingLadder = bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        bool hasVerticalSpeed = Mathf.Abs(rb2d.velocity.y)>Mathf.Epsilon;
        float vMove = Input.GetAxisRaw("Vertical");
        if(isTouchingLadder){
            Vector2 climbVelocity = new Vector2(rb2d.velocity.x,climbSpeed*vMove);
            rb2d.velocity = climbVelocity;
            rb2d.gravityScale=0f;
            //bool hasVerticalSpeed = Mathf.Abs(rb2d.velocity.y)>Mathf.Epsilon;
            animatorController.SetBool("isClimbing",hasVerticalSpeed);
            animatorController.SetBool("isJump",false);
        }
        else{
            animatorController.SetBool("isClimbing",false);
            rb2d.gravityScale=gravityDefault;
        }
        
        
    }

    private void Die(){
        if(IsHit() && invincibility==false){
            Debug.Log("Player got hit!");
            animatorController.SetTrigger("Dying");
            //animatorController.SetBool("isJump",false);
            rb2d.velocity = deathKick;
            AudioSource.PlayClipAtPoint(hitSound,transform.position,soundVol);
            StartCoroutine(Hit());
            StartCoroutine(Blink(0.7f,0.05f));
            
        }
    }
    private bool IsHit(){
        bool isHit;
        if(bodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards"))
            || feetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")) 
            || bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy"))){
            
            isHit=true;
        }
        else{
            isHit=false;
        }
        return isHit;
    }

    IEnumerator Hit(){
        MakeInvincible(true);
        if(pLive<1){
                DisableColliders();
                isAlive=false;
        } 
        yield return new WaitForSecondsRealtime(invincibleTime);
        //yield return StartCoroutine(Blink(0.7f,0.05f));
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
        MakeInvincible(false); 
    }
    IEnumerator Blink(float duration, float blinkTime){
        while(duration >0f && invincibility == true){
            duration -= Time.deltaTime;
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkTime);
        }
        rend.enabled = true;
    }
    public void MakeInvincible(bool invincibility){
        //bodyCollider.enabled = !invincibility;
        this.invincibility=invincibility;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("HitLocation"),invincibility);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Hazards"),invincibility);
        
        //Debug.Log(invincibility.ToString());
    }

    public void DisableColliders(){
        bodyCollider.enabled=false;
        feetCollider.enabled=false;
    }
    public bool GetInvincibility(){
        return invincibility;
    }
    
    public Vector2 GetPlayerRb2dPosition(){
        return rb2d.position;
    }
}
