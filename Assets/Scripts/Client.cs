using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {

    public GameObject player;
    public GameObject target;

    NetworkClient myClient;
    int playerCount = 1;

    const short LeftMsgId   = 160;
    const short JoinedMsgId = 280;
    const short SwitchMsgId = 420;

    // Use this for initialization
    void Start() {
        //target = GameObject.FindGameObjectWithTag("Flame");

        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(SwitchMsgId, OnSwitch);
        myClient.RegisterHandler(JoinedMsgId, OnJoined);
        myClient.RegisterHandler(LeftMsgId, OnLeft);
        myClient.Connect("127.0.0.1", 4444);

        Application.runInBackground = true;
    }

    // Update is called once per frame
    void Update() { }

    public void OnSwitch(NetworkMessage netMsg) {
        target.SetActive(netMsg.ReadMessage<SwitchMessage>().action);
    }

    public void OnConnected(NetworkMessage netMsg) {
        Debug.Log("Connected to server successfully.");
    }

    public void OnLeft(NetworkMessage netMsg) {
        JoinedLeftMessage joinedMsg = netMsg.ReadMessage<JoinedLeftMessage>();
        int actPlayerCount = joinedMsg.playerCount;
        this.playerCount = actPlayerCount;
        if (actPlayerCount >= 4) { return; }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Destroy(players[players.Length - 1]);
        
    }

    public void OnJoined(NetworkMessage netMsg) {
        JoinedLeftMessage joinedMsg = netMsg.ReadMessage<JoinedLeftMessage>();
        int actPlayerCount = joinedMsg.playerCount;
        
        int dist = actPlayerCount - this.playerCount - 1;
        GameObject newPlayer;

        // Fillin' in slots for those connected later (3rd and 4th players)
        if (dist > 0) {
            newPlayer = Instantiate(player);
            newPlayer.transform.position = new Vector3(0f, 4.5f, 20f);
            newPlayer.transform.Rotate(0, 180, 0);
        } if (dist > 1) {
            newPlayer = Instantiate(player);
            newPlayer.transform.position = new Vector3(-20f, 4.5f, 0f);
            newPlayer.transform.Rotate(0, 90, 0);
        }

        // Adding newly joined player
        this.playerCount = actPlayerCount;
        newPlayer = Instantiate(player);
        switch (playerCount) {
            case 2:
                newPlayer.transform.position = new Vector3(0f, 4.5f, 20f);
                newPlayer.transform.Rotate(0, 180, 0);
                break;
            case 3:
                newPlayer.transform.position = new Vector3(-20f, 4.5f, 0f);
                newPlayer.transform.Rotate(0, 90, 0);
                break;
            case 4:
                newPlayer.transform.position = new Vector3(20f, 4.5f, 0f);
                newPlayer.transform.Rotate(0, -90, 0);
                break;
            default:
                Destroy(newPlayer);
                break;
        }
    }

    public void SendMessageSwitch(bool action) {
        myClient.Send(SwitchMsgId, new SwitchMessage(action));
    }
}
