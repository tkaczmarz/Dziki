using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTurnPanel : MonoBehaviour 
{
	public Image colorBackground;
    public Text text;

    /// <summary>
    /// Method displays banner showing which team's turn it is.
    /// <param name="team">Team which turn it is. It's number and color is used.</param>
    /// <param name="activeTime">How long will the banner be shown in seconds.</param>
    /// </summary>
    public void Show(Team team, float activeTime)
    {
        gameObject.SetActive(true);
        colorBackground.color = team.color;
        text.text = "Turn of team " + team.nr;
        StopAllCoroutines();
        StartCoroutine(ActivateForTime(activeTime));
    }

    /// <summary>
    /// Method displays banner with given text message.
    /// <param name="msg">Message to display.</param>
    /// <param name="team">Team which turn it is. It's number and color is used.</param>
    /// <param name="activeTime">How long will the banner be shown in seconds.</param>
    /// </summary>
    public void Show(string msg, Team team, float activeTime)
    {
        gameObject.SetActive(true);
        colorBackground.color = team.color;
        text.text = msg;
        StopAllCoroutines();
        StartCoroutine(ActivateForTime(activeTime));
    }

    private IEnumerator ActivateForTime(float activeTime)
    {
        float timer = 0.0f;
        while (timer < activeTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        gameObject.SetActive(false);
    }
}
