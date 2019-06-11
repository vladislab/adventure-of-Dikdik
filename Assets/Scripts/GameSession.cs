using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int maxLives = 3;
    [SerializeField] Text LivesText;
    [SerializeField] Text ScoreText;
    [SerializeField] static int playerScore;
    
    public GameObject pauseMenu;
    private int currentScore;
    private bool isPaused;

    //Example of Singleton Pattern
    private void Awake() {
        int numGameSession = FindObjectsOfType<GameSession>().Length;
        if(numGameSession>1){
            Destroy(gameObject);
        }
        else{
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        LivesText.text = playerLives.ToString();
        ScoreText.text = playerScore.ToString();
        pauseMenu.SetActive(false);
        isPaused=false;
    }
    private void Update() {
       // Debug.Log("Current Score: "+currentScore.ToString()+"| PlayerScore = "+playerScore.ToString());   
        if(Input.GetButtonDown("Cancel")){
            if(!isPaused){
                PauseGame();
            }
            else{
                ResumeGame();
            } 
            
        }
    }

    public void ProcessPlayerDeath(){
        if(playerLives>=1){
            TakeLife();
        }
        else{
            
            ResetGameSession();
        }
    }

    private void TakeLife(){
        playerLives--;
        LivesText.text = playerLives.ToString();
    }
    public void AddToScore(int score){
        currentScore +=score;
        ScoreText.text = (playerScore+currentScore).ToString();
        
    }
    public void AddToLive(){
        if(playerLives<maxLives){
            playerLives++;
        }
        else{
            AddToScore(300);
        }
        LivesText.text = playerLives.ToString();
    }
    private void ResetGameSession(){
        currentScore=0;
        playerLives=3;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //Destroy(gameObject);
        ScoreText.text = playerScore.ToString();
        LivesText.text = playerLives.ToString();
    }
    public void OnLoadNextLevel(){
        playerScore+=currentScore;
        currentScore=0;
        ScoreText.text = playerScore.ToString();
    }
    public void OnLoadStartLevel(){
        playerLives=3;
        currentScore=0;
        playerScore=0;
        ScoreText.text = playerScore.ToString();
        LivesText.text = playerLives.ToString();
    }
    public int GetPlayerLives(){
        return playerLives;
    }
    public void StartGame(){
        SceneManager.LoadScene(1);
    }
    public void QuitGame(){
        Application.Quit();
    }

    public void PauseGame(){
        Time.timeScale=0f;
        pauseMenu.SetActive(true);
        isPaused=true;
        Cursor.visible=true;
    }
    public void ResumeGame(){
        Time.timeScale=1f;
        pauseMenu.SetActive(false);
        isPaused=false;
        Cursor.visible=false;
    }
}
