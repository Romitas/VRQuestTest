using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Active players count status message
public class JoinedLeftMessage : MessageBase {
    public int playerCount;

    public JoinedLeftMessage() { }
    public JoinedLeftMessage(int playerCount) {
        this.playerCount = playerCount;
    }
}

// Flame (light) state status message
public class SwitchMessage : MessageBase {
    public bool action;
    public SwitchMessage() { }
    public SwitchMessage(bool action) { this.action = action; }
}

public class Server : MonoBehaviour {

    private int playerCount = 0;
    private bool state = true;

    // TODO: fix these
    public const short SwitchMsgId = 420;
    public const short JoinedMsgId = 280;
    public const short LeftMsgId = 160;

    // Use this for initialization
    void Start () {
        Debug.Log("Hello World");

        NetworkServer.Listen(4444);
        NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        NetworkServer.RegisterHandler(SwitchMsgId, OnSwitch);

        Application.runInBackground = true;
    }
	
	// Update is called once per frame
	void Update () { }

    private void OnConnected(NetworkMessage netMsg) {
        playerCount++;
        Debug.Log("Client connected");

        // Update all player with the current state of flame and new player count
        NetworkServer.SendToAll(JoinedMsgId, new JoinedLeftMessage(playerCount));
        NetworkServer.SendToAll(SwitchMsgId, new SwitchMessage(this.state));
        Debug.Log(this.state);
    }

    private void OnDisconnected(NetworkMessage netMsg) {
        playerCount--;
        Debug.Log("Client disconnected");

        NetworkServer.SendToAll(LeftMsgId, new JoinedLeftMessage(playerCount));
    }

    private void OnSwitch(NetworkMessage netMsg) {
        SwitchMessage msg = netMsg.ReadMessage<SwitchMessage>();
        this.state = msg.action;

        Debug.Log("Light switched");
        NetworkServer.SendToAll(SwitchMsgId, msg);
    }
}
