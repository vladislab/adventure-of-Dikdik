using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsPickup : MonoBehaviour
{
    [SerializeField] int point = 100;
    [SerializeField] [Range(0f,2f)] float soundVol =1f;
    public AudioClip pickupSound;
    private float originalY;
    private bool addedToScore = false;
    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        Hovering();
    }

    private void OnTriggerEnter2D(Collider2D other) { 
        GetComponent<Animator>().Play("PickupAnimation");
        Destroy(gameObject,0.33f);
        if(!addedToScore){
            FindObjectOfType<GameSession>().AddToScore(point);
            addedToScore = true;
            AudioSource.PlayClipAtPoint(pickupSound,transform.position,soundVol);
        }
        GetComponent<Collider2D>().enabled=false;
    }

    private void Hovering(){
        var hoveringPosition = new Vector3(transform.localPosition.x,0.1f*Mathf.Sin(5f*Time.time)+originalY,0f);
        transform.localPosition = hoveringPosition;
    }
}
