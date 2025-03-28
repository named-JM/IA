using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void GoToMapScene()
    {
        SceneManager.LoadScene("Map");
    }
    // Function to load the LevelSelection scene
    public void GoToLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection"); 
    }

    // Function to load the Puzzle scene
    public void GoToPuzzleScene()
    {
        SceneManager.LoadScene("Puzzle"); 
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("Main"); 
    }

    public void GoToQuizScene()
    {
        SceneManager.LoadScene("Quiz");
    }
    public void GoToStartScene()
    {
        SceneManager.LoadScene("Start");
    }
   
}
