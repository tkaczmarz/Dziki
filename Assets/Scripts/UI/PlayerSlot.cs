using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour 
{
    public int nr = 0;
	public Text playerNameText;
    public Button kickButton;
    public Button occupyButton;
    public Image teamColor;

    private bool isEmpty = true;

    public void Initialize()
    {
        playerNameText.text = "<pusty>";
        // occupyButton.gameObject.SetActive(true);
        kickButton.gameObject.SetActive(false);
        isEmpty = true;
    }

    public void Occupy(Team team, PlayerSlot from)
    {
        // if (team.isAdmin)
        // {
        //     playerNameText.text = team.leader + " (Admin)";
        // }
        // else
            playerNameText.text = team.leader;

        // clear previous slot
        if (from != null)
            from.Initialize();

        team.color = teamColor.color;
        team.nr = nr;
        isEmpty = false;
        occupyButton.gameObject.SetActive(false);

        // Team localPlayer = MainMenuController.Instance.playerTeam;
        // if (localPlayer.isAdmin && localPlayer != team)
        //     kickButton.gameObject.SetActive(true);
    }
}
