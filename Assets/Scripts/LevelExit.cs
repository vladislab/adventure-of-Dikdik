using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class LevelExit : MonoBehaviour
{
    [SerializeField] float LevelLoadDelay = 2f;
    [SerializeField] float LevelExitSlowMoFactor = 0.7f;
    [SerializeField] [Range(0f,1f)] float soundVol;
    public AudioClip exitSound;
    public UnityEvent OnLoadingNextScene;
    private bool isPressed = false;
    private void Update() {
        isPressed =Input.GetButtonDown("Vertical");
    }
    private void OnTriggerStay2D(Collider2D other) {   
        Debug.Log("Player touches Door");   
        if(isPressed){
            StartCoroutine(LoadNextLevel());
            AudioSource.PlayClipAtPoint(exitSound,transform.position,soundVol);
            
        }
        
    }
    
    IEnumerator LoadNextLevel(){
        FindObjectOfType<GameSession>().OnLoadNextLevel();
        Destroy(FindObjectOfType<ScenePersist>());
        Time.timeScale = LevelExitSlowMoFactor;
        yield return new WaitForSecondsRealtime(LevelLoadDelay);
        Time.timeScale = 1f;
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex+1);
        //OnLoadingNextScene.Invoke();
    }

}
