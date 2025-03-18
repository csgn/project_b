using System;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    private static SteamManager Instance { get; set; }
    
    [SerializeField]
    private FacepunchTransport transport;

    private Lobby? Lobby { get; set; }

    #region Steam Callbacks
    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.LogError($"Lobby couldn't be created!, {result}");
            return;
        }

        lobby.SetFriendsOnly();
        lobby.SetJoinable(true);

        Debug.Log($"Lobby was created: {lobby.Id}");
    }
    
    private void OnLobbyEntered(Lobby lobby)
    {
        if (NetworkManager.Singleton.IsHost) return;
        StartClient(lobby.Id);
    }
    
    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
    }
    
    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
    }
    #endregion
    
    #region Network Callbacks
    private void OnServerStarted()
    {
        Debug.Log("Server started!", this);
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Client connected, clientId={clientId}", this);
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"Client disconnected, clientId={clientId}", this);

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }
    #endregion 
    
    #region Public Connection Methods
    public async void StartHost(int maxMembers = 8)
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        
        Lobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
    }
    
    public void StartClient(SteamId id)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        transport.targetSteamId = id;

        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        Lobby?.Leave();

        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.Shutdown();
    }
    
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
    }

    private void OnApplicationQuit() => Disconnect();
    #endregion
}