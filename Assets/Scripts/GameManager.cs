using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int numPlayers = 50;
    public RaceController currentRace;

    public static GameManager Instance { get; private set; }

    private List<Vector3> spawnPositions = new List<Vector3>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeSpawnPositions();
    }

    //Lista para guardar las posiciones
    private void InitializeSpawnPositions()
    {
        GameObject startPositionsParent = GameObject.Find("StartPos");
        if (startPositionsParent != null)
        {
            foreach (Transform child in startPositionsParent.transform)
            {
                spawnPositions.Add(child.position);
            }
        }
        else
        {
            Debug.LogError("Start positions parent object 'startpos' not found!");
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    //con el servidor conectado se empiezan a conectar los clientes
    private void OnServerStarted()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    //asignamos posicion por cliente asignado
    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            AssignPlayerSpawnPosition(clientId);
        }
    }
    
    //asignamos la posición, con ayuda del id de cliente encontramos la posicion correspondiente
    private void AssignPlayerSpawnPosition(ulong clientId)
    {
        Vector3 spawnPosition = spawnPositions[(int)(clientId % (ulong)spawnPositions.Count)];

        NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient);
        if (networkClient != null && networkClient.PlayerObject != null)
        {
            Player playerScript = networkClient.PlayerObject.GetComponent<Player>();
            playerScript.spawnPosition.Value = spawnPosition;
            playerScript.transform.position = spawnPosition;
        }
    }
}
