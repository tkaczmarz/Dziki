using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MessageDialog : DialogBase 
{
	public Button okButton;
    public Text message;

    private void Start() 
    {
        if (!okButton)
        {
            Debug.LogWarning("Dialog is missing Button reference. Attempting to find it...");
            okButton = GetComponentInChildren<Button>();
        }

        if (!message)
        {
            Debug.LogWarning("Dialog is missing Text reference. Attempting to find it...");
            message = GetComponentInChildren<Text>();
        }

        okButton.onClick.AddListener(PressedOK);
    }

    public void Show(string msg, UnityAction action = null)
    {
        message.text = msg;
        this.action = action;
        gameObject.SetActive(true);
    }

    public void PressedOK()
    {
        if (action != null)
            action.Invoke();

        Destroy(gameObject);
    }

    /// <summary>Method instantiates dialog prefab from Resources folder.</summary>
    public static MessageDialog Create(float screenWidthPercentage = 45, float screenHeightPercentage = 50)
    {
        Object res = Resources.Load("UI/MessagePanel");
        if (res)
        {
            GameObject panel = Instantiate(Resources.Load("UI/MessagePanel")) as GameObject;
            panel.transform.SetParent(MainMenuController.Instance.UICanvas.transform);
            panel.transform.localPosition = Vector3.zero;

            // set dialog size
            float width = Screen.width * (screenWidthPercentage / 100);
            float height = Screen.height * (screenHeightPercentage / 100);
            RectTransform rt = panel.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.sizeDelta = new Vector2(width, height);
            }

            return panel.GetComponent<MessageDialog>();
        }
        return null;
    }
}
