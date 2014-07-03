using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {

    private bool _isMenu = true;
    private bool _isPause = false;
    private bool _isOrto = false;

    private int _numLevel = 1;

    public int wMaze = 5;
    public int hMaze = 5;
    
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (Application.loadedLevel == 2)
        {
            if (GameObject.Find("EndLevel").GetComponent<TriggerEndLevel>().endLevel)
            {
                wMaze++;
                hMaze++;
                _numLevel++;
                GameObject.Find("EndLevel").GetComponent<TriggerEndLevel>().endLevel = false;
                StartCoroutine(loadScene("white"));
            }
        }
	}

    void OnGUI()
    {
        GUI.Label(new Rect(100, 5, Screen.width - 200, 50), "Level " + _numLevel + " (w:" + wMaze + "; h:" + hMaze + ")");
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
            if (GUI.Button(new Rect(Screen.width - 50, 5, 45, 25), "X"))
            {
                _isPause = !_isPause;
            }

            if (_isPause)
            {
                GUI.Window(10, new Rect(15, 50, Screen.width - 30, Screen.height - 65), windowSettings, "Settings");
            }
        }
    }

    IEnumerator loadScene(string nameScene)
    {
        Application.LoadLevel("splash");
        // остановка выполнения функции на 10 миллисекунд
        yield return new WaitForSeconds(0.01f);
        Application.LoadLevel(nameScene);
    }

    void windowSettings(int windowID)
    {
        if (GUI.Button(new Rect(25, 50, Screen.width - 80, 30), "Resume")) {
            _isPause = false;
        }

        if (GUI.Button(new Rect(25, 90, Screen.width - 80, 30), "2D / 3D"))
        {
            _isOrto = !_isOrto;
            Camera.mainCamera.orthographic = _isOrto;
        }

        if (GUI.Button(new Rect(25, 130, Screen.width - 80, 30), "Quit"))
        {
            Application.Quit();
        }

    }
}
