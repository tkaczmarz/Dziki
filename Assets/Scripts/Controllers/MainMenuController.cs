﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TestujeSocketRAZ.service;
using TestujeSocketRAZ.model.send;

public enum NameStatus
{
    Valid, WrongLength, WrongCharacters
}

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
    private Button gameStartButton;
    private string roomName = "";
    private string adminName = "";
    private Lobby lobby;

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

        gameStartButton = roomDialog.GetComponentInChildren<Button>();

        lobby = playerTeam.gameObject.GetComponent<Lobby>();
    }

    public void LoadTestScene()
    {
        playerTeam.nr = 1;
        playerTeam.color = Color.red;
        LevelLoader.Instance.LoadTestScene();
    }

    public void PlayButtonAction()
    {
        // check if given nickname is valid
        NameStatus status = NameValid(nickField.text);
        if (status == NameStatus.Valid)
        {
            bannerAnim.SetBool("Show", true);
            playerTeam.leader = nickField.text;
        }
        else
        {
            switch (status)
            {
                case NameStatus.WrongLength:
                {
                    MessageDialog.Create().Show("Podany niepoprawny nick!\nMusi mieć minimum 2 i maksimum 20 znaków.");
                }
                break;
                case NameStatus.WrongCharacters:
                {
                    MessageDialog.Create().Show("Nick nie może zawierać spacji oraz przecinków!");
                }
                break;
                default:
                {
                    MessageDialog.Create().Show("Niepoprawny nick!");
                }
                break;
            }
        }
    }

    public void BackToMenuButtonAction()
    {
        if (roomDialog.activeSelf)
        {
            roomDialog.gameObject.SetActive(false);
            createLobbyDialog.gameObject.SetActive(true);
            lobbyText.text = "Stwórz lub wyszukaj grę";
        }
        else
            bannerAnim.SetBool("Show", false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

	public void LoadMainScene()
    {
        LevelLoader.Instance.LoadTestScene();
    }

    private NameStatus NameValid(string name)
    {
        if (name.Length < 2 || name.Length > 20)
            return NameStatus.WrongLength;

        if (name.Contains(" ") || name.Contains(","))
            return NameStatus.WrongCharacters;

        return NameStatus.Valid;
    }

    /// <summary>Method creates a lobby for players to join.</summary>
    public void CreateRoom()
    {
        roomName = roomNameField.text;
        if (NameValid(roomName) != NameStatus.Valid)
        {
            Debug.Log("WRONG ROOM NAME!");
            MessageDialog.Create(35, 25).Show("Nazwa pokoju musi składać się z 2-20 znaków!");
            return;
        }

        Debug.Log("CREATING NEW ROOM " + roomName + " for " + playerTeam.leader);
        try
        {
            RoomManager rm = new RoomManager();
            int port = rm.addRoom(playerTeam.leader, roomName, 4);
            bool result = port == -1 ? false : true;
            
            if (result)
            {
                Debug.Log("Room creation successful");
                StartCoroutine(lobby.WaitForServerCommands(port));

                playerTeam.isAdmin = true;
                createLobbyDialog.SetActive(false);
                roomDialog.SetActive(true);
                lobbyText.text = roomName;
                if (gameStartButton && playerTeam.isAdmin)
                    gameStartButton.gameObject.SetActive(true);
                else
                    gameStartButton.gameObject.SetActive(false);

                playerTeam.isAdmin = true;
                if (slots.Length > 0)
                    slots[0].Occupy(playerTeam, null);
            }
            else
            {
                MessageDialog.Create().Show("Serwer nie odpowiada. Spróbuj ponownie później.");
                roomName = "";
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Server exception: " + e.StackTrace);
            MessageDialog.Create().Show("Serwer nie odpowiada. Spróbuj ponownie później.");
            roomName = "";
        }
    }

    public void EnterRoom()
    {
        roomName = roomNameField.text;
        if (NameValid(roomName) != NameStatus.Valid)
        {
            MessageDialog.Create(35, 25).Show("Nazwa pokoju musi składać się z 2-20 znaków!");
            return;
        }

        try
        {
            PlayerManager pm = new PlayerManager();
            List<string> playerList;
            int port = pm.addPlayer(roomName, "", playerTeam.leader, out playerList);
            bool result = port == -1 ? false : true;
            
            if (result)
            {
                playerTeam.isAdmin = false;
                Debug.Log("Entered room successfully!");
                StartCoroutine(lobby.WaitForServerCommands(port));

                // place players on their slots
                for (int i = 0; i < playerList.Count; i++)
                {
                    slots[i].playerNameText.text = playerList[i];
                }
                playerTeam.nr = playerList.Count;

                createLobbyDialog.SetActive(false);
                roomDialog.SetActive(true);
                if (gameStartButton && playerTeam.isAdmin)
                    gameStartButton.gameObject.SetActive(true);
                else
                    gameStartButton.gameObject.SetActive(false);
                    
                lobbyText.text = roomName;

                // if (slots.Length > 0)
                //     slots[0].Occupy(playerTeam, null);
            }
            else
            {
                MessageDialog.Create().Show("Nie udało się dołączyć do pokoju '" + roomName + "'");
                roomName = "";
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Server exception: " + e.StackTrace);
            MessageDialog.Create().Show("Nie udało się dołączyć do pokoju '" + roomName + "'");
            roomName = "";
        }
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

    public void RefreshPlayerNames(List<string> names)
    {
        for (int i = 0; i < names.Count; i++)
        {
            Debug.Log("NAME " + i + ": "+ names[i]);
            slots[i].playerNameText.text = names[i];
        }
    }

    public void SendStartRequest()
    {
        RoomManager rm = new RoomManager();
        bool result = rm.roomStart(roomName, "", "");
    }

    List<Team> teams = new List<Team>();
    public void StartGame(bool online = false)
    {
        Debug.Log("Starting room " + roomName);

        foreach (PlayerSlot slot in slots)
        {
            string player = slot.playerNameText.text;
            if (player != "<pusty>")
            {
                if (player.Equals(playerTeam.leader))
                {
                    playerTeam.color = slot.teamColor.color;
                    playerTeam.nr = slot.nr;
                    teams.Add(playerTeam);
                }
                else
                {
                    GameObject teamObject = new GameObject("Team " + player);
                    Team team = teamObject.AddComponent<Team>();
                    team.leader = player;
                    team.nr = slot.nr;
                    team.color = slot.teamColor.color;
                    teams.Add(team);
                }
            }
        }

        LevelLoader.Instance.LoadTestScene();
    }
}
