using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string gameSceneName; //the name of the Game Scene to load. Important if there is multiple levels
    public Button play;

    void Start () {
		Button btn = play.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

     void TaskOnClick()
    {
        Debug.Log("Load Level");
        SceneManager.LoadScene(gameSceneName);
        
    }
}