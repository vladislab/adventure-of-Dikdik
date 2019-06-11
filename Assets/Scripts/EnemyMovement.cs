using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Species Identifer: (Only Pick One)")]
    [SerializeField] public bool jumpType = false;
    [SerializeField] public bool walkType = false;
    [SerializeField] public bool flyingType = false;
    [SerializeField] public float aggroRange = 5f;
    
    [Header("Enemy Config Params:")]
    [SerializeField] public float moveSpeed =1f;

    [Header("Sound:")]
    [SerializeField] [Range(0f,2f)] float soundVol = 1f;
    
    [Header("If Object is an Eagle:")]
    [SerializeField] public float startDashTime;
    [SerializeField] public float eagleDashSpeed;
    [SerializeField] public float eagleTrackingRange=30f;
    [SerializeField] public float eagleChasingRange=10f;
    [Header("Required Components:")]
    public Rigidbody2D rb2d;
    public BoxCollider2D sideCollider;
    public CapsuleCollider2D bodyCollider;
    public Animator animController;
    public AudioClip deathSound;
    
    private bool addedToScore=false;
    private bool isAlive = true;
    private bool isJumping = false;
    private bool isTracking = false;
    private float dashTime;
    private Vector2 eagleOriginalLocation;
    private bool isBackingOff=false;
    private float tempRange;
    //private float dashDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb2d=GetComponent<Rigidbody2D>();
        sideCollider=GetComponent<BoxCollider2D>();
        bodyCollider=GetComponent<CapsuleCollider2D>();
        animController=GetComponent<Animator>();
        eagleOriginalLocation = transform.position;

        CheckSpecies();
        tempRange = aggroRange;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpecies();
        if(!isAlive){
            return;
        }
        if(walkType){
            if(IsFacingRight()){
                rb2d.velocity = new Vector2(moveSpeed,0f);
            }
            else{
                rb2d.velocity = new Vector2(-moveSpeed,0f);
            }
        }
        
        FlipSprite();
        CheckHazards();
    }
    private void FixedUpdate() {
        if(jumpType||flyingType){
            //rb2d.bodyType = RigidbodyType2D.Dynamic;
            AggroDetection();
            if(flyingType){
                Flying();
            }
        }
    }
    private bool IsFacingRight(){
        return transform.localScale.x < 0;
    }

    private void OnTriggerExit2D(Collider2D other) {
        transform.localScale= new Vector2((Mathf.Sign(rb2d.velocity.x)),1f);
    }

    public void IsAttacked(){
        //rb2d.bodyType = RigidbodyType2D.Static;
        isAlive = false;
        DisableColliders();
        rb2d.velocity=Vector2.zero;
        if(!addedToScore){
            FindObjectOfType<GameSession>().AddToScore(150);
            addedToScore=true;
            StopAllCoroutines();
            AudioSource.PlayClipAtPoint(deathSound,transform.position,soundVol);
        }
        
    }

    private void AggroDetection(){
        Vector2 playerPosition = FindObjectOfType<Player>().GetPlayerRb2dPosition();
        float direction = Mathf.Sign(playerPosition.x-rb2d.position.x);
        Vector2 distanceFromPlayer = playerPosition-rb2d.position;
        
        if(distanceFromPlayer.magnitude<=aggroRange&& !isTracking){
            if(jumpType){
                Debug.Log("Distance from Frog to Player: "+distanceFromPlayer.magnitude);
                Jump(direction,true);
            }
        
        }
        if(flyingType){
                float distanceFromOrigin = (rb2d.position-eagleOriginalLocation).magnitude;
                //Debug.Log("Eagle from Nest:"+distanceFromOrigin.ToString());
                //Debug.Log("Distance from Eagle to Player: "+distanceFromPlayer.magnitude);
                StartCoroutine(EagleTrackProcess(playerPosition,distanceFromPlayer));
        }
        

    }
    private void Jump(float direction,bool activated){
        if(activated){
            if(sideCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
                StartCoroutine(JumpProcess(direction));    
                
            }
            else{
                animController.SetBool("isMoving",false);
            }
        }
        else{
            StopCoroutine(JumpProcess(direction));
        }
    }
    IEnumerator JumpProcess(float direction){
        if(isJumping==false){
            isTracking=true;
            isJumping=true;
            yield return new WaitForSecondsRealtime(1f);
            animController.Play("FrogJump");
            rb2d.AddForce(new Vector2(direction*7f,13f),ForceMode2D.Impulse);
            Debug.Log("Frogs Jump!");     
            //yield return new WaitForSecondsRealtime(3f);
            isJumping=false;
            isTracking=false;
        }
        
    }
    private void FlipSprite(){
        bool isMovingHorizontal = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
            if(isMovingHorizontal){
                transform.localScale =new Vector2(-Mathf.Sign(rb2d.velocity.x),1f);
            }
    }
    
    private void CheckHazards(){
        if(!flyingType){
            if(sideCollider.IsTouchingLayers(LayerMask.GetMask("Hazards"))
            ||bodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards"))){
                Debug.Log("Enemy touches Hazards");
                animController.Play("EnemyDeath");
                Destroy(gameObject,0.33f);
            }
        }
        
    }

    private void Flying(){
        Vector2 hoverPosition = new Vector2(rb2d.position.x,rb2d.position.y+0.01f*Mathf.Sin(1f*Time.fixedTime));
        rb2d.position=hoverPosition;
    }
    private void DisableColliders(){
        bodyCollider.enabled=false;
        sideCollider.enabled=jumpType;
    }

    IEnumerator EagleTrackProcess(Vector2 playerPosition,Vector2 distance){
        isTracking=true;
        float distanceFromOrigin = (rb2d.position-eagleOriginalLocation).magnitude;
        if(distanceFromOrigin>=eagleTrackingRange){
            rb2d.velocity=Vector2.zero;
            yield return new WaitForSeconds(3f);
            isBackingOff=true;
            tempRange=aggroRange;
            EagleReturns();
        }
        else if(distance.magnitude<=tempRange && !isBackingOff){
            tempRange=eagleChasingRange;
            EagleHoming(playerPosition,distance);
        }
        isTracking=false;
    }
    private void EagleHoming(Vector2 playerPosition,Vector2 distanceFromPlayer){
        
        if(distanceFromPlayer.magnitude<=2){
            rb2d.velocity=Vector2.zero;
        }
        else{
            Vector2 dashDirection = distanceFromPlayer.normalized;
            rb2d.velocity = new Vector2(moveSpeed*dashDirection.x,moveSpeed*dashDirection.y);
        }
    }
    private void EagleReturns(){
        //Vector2 directionFromOrigin = (eagleOriginalLocation-rb2d.position).normalized;
        var currentMovePosition = transform.position;
        //var velocity = (eagleOriginalLocation - currentMovePosition).normalized; 
        currentMovePosition.x = Mathf.MoveTowards(transform.position.x,eagleOriginalLocation.x,moveSpeed*Time.deltaTime);
        currentMovePosition.y = Mathf.MoveTowards(transform.position.y,eagleOriginalLocation.y,moveSpeed*Time.deltaTime);
        Debug.Log(currentMovePosition);
        //Debug.Log("Eagle Velocity:" + rb2d.velocity);
        if(isBackingOff){
            Debug.Log("Eagle Backing Off!");
            rb2d.MovePosition(currentMovePosition);
            //rb2d.velocity = velocity;
            
        }
        if((Vector2)currentMovePosition==eagleOriginalLocation){
                isBackingOff=false;
        }
    }
    private void CheckSpecies(){
        if(flyingType && jumpType){
            throw new System.Exception("Enemy cannot be two different species simutaneously!");
        }
        else if(flyingType && walkType){
            throw new System.Exception("Enemy cannot be two different species simutaneously!");
        }
        else if(walkType && jumpType){
            throw new System.Exception("Enemy cannot be two different species simutaneously!");
        }
    }
}
