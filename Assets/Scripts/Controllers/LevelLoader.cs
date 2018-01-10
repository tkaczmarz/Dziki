using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour 
{
    public static LevelLoader Instance { get { return instance; } }

	private static LevelLoader instance = null;

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadTestScene()
    {
        SceneManager.LoadScene("TestScene");
    }
}
