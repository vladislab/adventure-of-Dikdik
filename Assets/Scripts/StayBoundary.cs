using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayBoundary : MonoBehaviour
{
    private void Update() {
        
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.name=="Player"){
            //other.transform.parent.GetComponent<Rigidbody2D>().velocity = transform.parent.GetComponent<Rigidbody2D>().velocity;
            //Debug.Log(transform.parent.GetComponent<Rigidbody2D>().velocity.ToString());
            other.transform.SetParent(transform.parent.transform);
            Debug.Log(other.GetComponent<Rigidbody2D>().velocity.ToString());
            Debug.Log("Player enters platform");
            
        }
        //Debug.Log("Platform Topped");    
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.name == "Player"){
            Debug.Log("Player exits platform");
            other.gameObject.transform.parent= null;
        }
        //Debug.Log("Platform Un-Topped");            
    }
    
}
