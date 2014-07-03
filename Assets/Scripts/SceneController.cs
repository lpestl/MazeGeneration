using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {

    private bool _isMenu = true;
    public int wMaze = 5;
    public int hMaze = 5;

    SceneController getInstance()
    {
        return this;
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        //StartCoroutine(loadScene("white"));
	}

    void OnGUI()
    {
        if (_isMenu)
        {
            // TO DO on menu
            if (GUI.Button(new Rect(50, Screen.height / 2 - 50, Screen.width - 100, 30), "Start game"))
            {
                _isMenu = false;
                StartCoroutine(loadScene("white"));
            }
            if (GUI.Button(new Rect(75, Screen.height / 2 + 20, Screen.width - 150, 30), "Quit"))
            {
                Application.Quit();
            }
        }
        else
        {
            // TO DO on scene
        }
    }

    IEnumerator loadScene(string nameScene)
    {
        Application.LoadLevel("splash");
        // остановка выполнения функции на 10 миллисекунд
        yield return new WaitForSeconds(0.01f);
        Application.LoadLevel(nameScene);
    }
}
