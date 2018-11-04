using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour {

    public static ClientManager instance;
    [SerializeField] string IPAddress;
    [SerializeField] int Port;
    [SerializeField] GameObject ConnectionPrefab;


    [SerializeField] public int myConnectionId;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();
        InitPlayers();
        ClientHandleData.InitPackets();
        ClientTCP.InitClient(IPAddress, Port); 
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InstantiateNetworkPlayers(int connectionId)
    {
        Types.Players[connectionId].PlayerPref = Instantiate(ConnectionPrefab);
        Types.Players[connectionId].PlayerPref.name = "Player: " + connectionId;
        if (connectionId != myConnectionId)
        {
            Types.Players[connectionId].PlayerPref.GetComponent<CharacterController>().enabled = false;
            Types.Players[connectionId].PlayerPref.GetComponentInChildren<Camera>().enabled = false;

        }

    }


    void InitPlayers()
    {
        for (int i = 1; i < Constants.MAX_PLAYERS; i++)
        {
            Types.Players[i] = new PlayerRec();
        }
    }

    private void OnApplicationQuit()
    {

        ClientTCP.DisconnectFromServer();
      
    }
}
