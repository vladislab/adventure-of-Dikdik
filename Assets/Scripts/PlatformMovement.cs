using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] public float hMove = 3f;
    [SerializeField] public float vMove = 0f;
    public BoxCollider2D boxCollider;
    public Rigidbody2D rb2d;
    public Collider2D playerColldier;
    private bool isFacingRight;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(boxCollider,playerColldier);
    }

    // Update is called once per frame
    void Update()
    {
        if(isFacingRight){
            Movement(1f);
        }
        else{
            Movement(-1f);
        }
    }

    private void Movement(float hDirection){
        rb2d.velocity = new Vector2(hMove*Mathf.Sign(hDirection),vMove);
        //Debug.Log(rb2d.velocity.ToString());
        //boxCollider.transform.localScale = new Vector2(hDirection,1f);
    }
    private void OnTriggerEnter2D(Collider2D other) {
            isFacingRight = !isFacingRight;
    }


}
