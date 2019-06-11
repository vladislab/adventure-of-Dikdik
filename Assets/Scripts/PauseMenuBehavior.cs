﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBehavior : MonoBehaviour
{

    private void Awake() {
        if(FindObjectsOfType<PauseMenuBehavior>().Length>1){
            Destroy(gameObject);
        }
        else{
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Resume(){
        
    }
}
