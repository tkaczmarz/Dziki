using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NextTurnPanel : DialogBase 
{
	public Image colorBackground;
    public Text text;

    /// <summary>Method displays banner showing which team's turn it is.
    /// </summary>
    /// <param name="team">Team which turn it is. It's number and color is used.
    /// </param>
    /// <param name="activeTime">How long will the banner be shown in seconds.
    /// </param>
    /// <param name="onDisableAction">Action that will be executed when banner disappears.
    /// </param>
    public void Show(Team team, float activeTime, UnityAction onDisableAction = null)
    {
        gameObject.SetActive(true);
        colorBackground.color = team.color;
        text.text = "Turn of team " + team.nr;
        action = onDisableAction;
        StopAllCoroutines();
        StartCoroutine(ActivateForTime(activeTime));
    }

    /// <summary>Method displays banner with given text message.
    /// </summary>
    /// <param name="msg">Message to display.
    /// </param>
    /// <param name="team">Team which turn it is. It's number and color is used.
    /// </param>
    /// <param name="activeTime">How long will the banner be shown in seconds.
    /// </param>
    /// <param name="onDisableAction">Action that will be executed when banner disappears.
    /// </param>
    public void Show(string msg, Team team, float activeTime, UnityAction onDisableAction = null)
    {
        gameObject.SetActive(true);
        colorBackground.color = team.color;
        text.text = msg;
        action = onDisableAction;
        StopAllCoroutines();
        StartCoroutine(ActivateForTime(activeTime));
    }
}
