using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuitButton : MonoBehaviour
{
    public Button quit;

    private void Start () {
		Button btn = quit.GetComponent<Button>();
		btn.onClick.AddListener(QuitOnClick);
	}

    private void QuitOnClick()
    {
        Debug.Log("Quit Level");
        Application.Quit(); //quits the game
        
    }
}