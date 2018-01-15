using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour 
{
    public static MainMenuController Instance { get { return instance; } }

    private static MainMenuController instance = null;

    public GameObject banner;
    public InputField nickText;
    public GameObject UICanvas;

    private Animator bannerAnim;
    private Team playerTeam;

    private void Awake() 
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    private void Start() 
    {
        if (!banner)
            Debug.LogError("Banner reference is not set!");
        else
        {
            bannerAnim = banner.GetComponent<Animator>();
        }

        if (!nickText)
            Debug.LogError("Missing reference to nickname text field! (MainMenuController)");

        playerTeam = FindObjectOfType<Team>();
        if (!playerTeam)
            Debug.LogError("Can't find player's team!");
    }

    public void StartButtonAction()
    {
        // check if given nickname is valid
        if (NicknameValid(nickText.text))
        {
            bannerAnim.SetBool("Show", true);
        }
        else
            MessageDialog.CreateDialog().Show("Podany niepoprawny nick!\nMusi mieć minimum 2 znaki.");
    }

    public void BackToMenuButtonAction()
    {
        bannerAnim.SetBool("Show", false);
    }

	public void LoadMainScene()
    {
        LevelLoader.Instance.LoadTestScene();
    }

    private bool NicknameValid(string name)
    {
        if (name.Length < 2)
            return false;

        if (name[0] == ' ' || name[1] == ' ')
            return false;

        return true;
    }
}
