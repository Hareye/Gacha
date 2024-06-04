using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class SocketManager : MonoBehaviour
{
    public static SocketManager instance;

    private WebSocket webSocket;

    private void Awake()
    {
        instance = this;
        // Don't destroy when implementing persistency
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        webSocket = new WebSocket("ws://localhost:3000/socket/");

        webSocket.OnOpen += (sender, e) =>
        {
            Debug.Log("Connected to WebSocket server!");
        };

        webSocket.OnMessage += (sender, e) =>
        {
            Debug.Log("Message received: " + e.Data);
        };

        webSocket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket closed");
        };

        webSocket.Connect();
    }

    void OnDestroy()
    {
        webSocket.Close();
    }

    public void sendMessage()
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Send("Test");
        }
    }
}
