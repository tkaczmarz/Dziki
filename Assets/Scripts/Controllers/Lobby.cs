using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TestujeSocketRAZ.service;
using TestujeSocketRAZ.model.send;

public class Lobby : MonoBehaviour 
{
    public int RoomPort { get { return roomPort; } }
    public bool Online { get { return online; } }

    private int roomPort;
    private bool online = false;

    public IEnumerator WaitForServerCommands(int port)
    {
        roomPort = port;
        //to jest odpowiedzialne za poruszanie graczem
        //wysylasz swoj ruch i konczysz polaczenie
        //udpClient.SendRequestPlayerMove(playerMove, 58278);
        //laczysz sie z serwerem i sluchasz wiadomosci
        
        Debug.Log("<color=lime>START LISTENING</color>");
        UdpManager udpClient = new UdpManager();
        udpClient.Connect(port);
        while (true) 
        {
            string data = udpClient.waitForMessages(port);

            // data analysis
            if (data.Length > 0)
            {
                Debug.Log("<color=orange>Data acquired: '" + data + "'</color>");

                if (data.Contains("playerList"))    // player list
                {
                    Debug.Log("<color=orange>Acquired player list!</color>");
                    MainMenuController.Instance.RefreshPlayerNames(ResponseHandler.getArrayFromString(data));
                }
                else if (data.Contains("roomStart"))    // start game
                {
                    Debug.Log("Start game");
                    online = true;
                    MainMenuController.Instance.StartGame();
                }
                else if (data.Contains("ENDOFTURN"))
                {
                    Debug.Log("END OF TURN");
                    GameController.Instance.TurnEndedButtonPressed();
                }
            }

            yield return null;

            //jak jakas przyjdzie mozesz dac break i znow SendRequestPlayerMove z twoim ruchem
        }
    }

    public void SendMessage(string msg, int port)
    {
        UdpManager manager = new UdpManager();
        PlayerMove req = new PlayerMove();
        req.test = msg;
        manager.SendRequestPlayerMove(req, port);
        Debug.Log("MESSAGE SENT");
    }
}
