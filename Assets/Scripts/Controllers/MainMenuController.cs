using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TestujeSocketRAZ.service;

public class MainMenuController : MonoBehaviour 
{
    public static MainMenuController Instance { get { return instance; } }

    private static MainMenuController instance = null;

    public GameObject banner;
    public InputField nickField;
    public GameObject UICanvas;
    public InputField roomNameField;
    public GameObject createLobbyDialog;
    public GameObject roomDialog;
    public Text lobbyText;

    [HideInInspector]
    public Team playerTeam;

    private Animator bannerAnim;
    private PlayerSlot[] slots;

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

        if (!nickField)
            Debug.LogError("Missing reference to nickname text field! (MainMenuController)");

        if (!roomNameField)
            Debug.LogError("Missing reference to room name text field! (MainMenuController)");

        playerTeam = GameObject.FindWithTag("Player").GetComponent<Team>();
        if (!playerTeam)
            Debug.LogError("Can't find player's team!");

        slots = roomDialog.GetComponentsInChildren<PlayerSlot>();
        foreach (PlayerSlot slot in slots)
            slot.Initialize();

        roomDialog.gameObject.SetActive(false);
        createLobbyDialog.gameObject.SetActive(true);
    }

    public void PlayButtonAction()
    {
        // check if given nickname is valid
        if (NicknameValid(nickField.text))
        {
            bannerAnim.SetBool("Show", true);
            playerTeam.leader = nickField.text;
        }
        else
            MessageDialog.Create().Show("Podany niepoprawny nick!\nMusi mieć minimum 2 i maksimum 20 znaków.");
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
        if (name.Length < 2 || name.Length > 20)
            return false;

        if (name[0] == ' ' || name[1] == ' ')
            return false;

        return true;
    }

    /// <summary>Method creates a lobby for players to join.</summary>
    public void CreateRoom()
    {
        string roomName = roomNameField.text;
        if (!NicknameValid(roomName))
        {
            Debug.Log("WRONG ROOM NAME!");
            MessageDialog.Create(35, 25).Show("Nazwa pokoju musi składać się z 2-20 znaków!");
            return;
        }

        Debug.Log("CREATING NEW ROOM " + roomName + " for " + playerTeam.leader);
        string result = RoomManager.addRoom(playerTeam.leader, roomName, 4);

        if (result == "success")
        {
            Debug.Log("Room creation successful");
            createLobbyDialog.SetActive(false);
            roomDialog.SetActive(true);
            lobbyText.text = roomName;

            playerTeam.isAdmin = true;
            if (slots.Length > 0)
                slots[0].Occupy(playerTeam, null);
        }
        else
            Debug.LogError("Server error during room creation");
    }

    public void ChangeSlot(int i)
    {
        PlayerSlot previousSlot = null;
        PlayerSlot targetSlot = null;
        foreach (PlayerSlot slot in slots)
            if (slot.nr == i)
                targetSlot = slot;
            else if (slot.nr == playerTeam.nr)
                previousSlot = slot;
        
        targetSlot.Occupy(playerTeam, previousSlot);
    }
}
