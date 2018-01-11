using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour 
{
    public GameObject banner;

    private Animator bannerAnim;

    private void Start() 
    {
        if (!banner)
            Debug.LogError("Banner reference is not set!");
        else
        {
            bannerAnim = banner.GetComponent<Animator>();
        }
    }

    public void StartButtonAction()
    {
        bannerAnim.SetBool("Show", true);
    }

    public void BackToMenuButtonAction()
    {
        bannerAnim.SetBool("Show", false);
    }

	public void LoadMainScene()
    {
        SceneManager.LoadScene(1);
    }
}
