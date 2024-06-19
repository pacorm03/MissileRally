using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RaceController : MonoBehaviour
{
    public int numPlayers;
    public GameObject[] circuits;  // Array de prefabs de circuitos
    public Transform[] startPoints;  // Array de puntos de inicio para cada circuito
    public GameObject playerPrefab;  // Prefab del coche del jugador
    public CinemachineVirtualCamera virtualCamera;  // Referencia a la Cinemachine Virtual Camera

    private readonly List<Player> _players = new List<Player>(4);
    private CircuitController _circuitController;
    private GameObject[] _debuggingSpheres;
    private GameObject originalPlayer;  // Referencia al coche del jugador original

    private void Start()
    {
        _circuitController = GetComponent<CircuitController>();

        _debuggingSpheres = new GameObject[GameManager.Instance.numPlayers];
        for (int i = 0; i < GameManager.Instance.numPlayers; ++i)
        {
            _debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }

        // Desactivar todos los circuitos existentes en la escena
        foreach (var circuit in circuits)
        {
            circuit.SetActive(false);
        }

        // Obtener el índice del circuito seleccionado
        int selectedCircuitIndex = PlayerPrefs.GetInt("SelectedCircuit", 0);

        // Instanciar solo el circuito seleccionado
        if (selectedCircuitIndex >= 0 && selectedCircuitIndex < circuits.Length)
        {
            circuits[selectedCircuitIndex].SetActive(true);
            Instantiate(circuits[selectedCircuitIndex], Vector3.zero, Quaternion.identity);

            // Posicionar al jugador en el punto de inicio correcto
            if (selectedCircuitIndex < startPoints.Length)
            {
                Transform startPoint = startPoints[selectedCircuitIndex];
                GameObject playerInstance = Instantiate(playerPrefab, startPoint.position, startPoint.rotation);
                Player player = playerInstance.GetComponent<Player>();
                player.ID = 0;  // Asigna un ID al jugador, si es necesario
                player.Name = "Player 1";  // Asigna un nombre al jugador, si es necesario
                player.car = playerInstance;  // Asigna el coche al jugador

                _players.Add(player);

                // Desactivar o eliminar el coche original del jugador
                if (originalPlayer != null)
                {
                    Destroy(originalPlayer);
                }

                // Configurar la cámara virtual de Cinemachine para seguir al coche instanciado
                virtualCamera.Follow = playerInstance.transform;
                virtualCamera.LookAt = playerInstance.transform;
            }
            else
            {
                Debug.LogError("No hay un punto de inicio definido para el índice de circuito seleccionado.");
            }
        }
        else
        {
            Debug.LogError("Índice de circuito seleccionado fuera de rango.");
        }
    }

    private void Update()
    {
        if (_players.Count == 0)
            return;

        UpdateRaceProgress();
    }

    public void AddPlayer(Player player)
    {
        _players.Add(player);
    }

    private class PlayerInfoComparer : Comparer<Player>
    {
        readonly float[] _arcLengths;

        public PlayerInfoComparer(float[] arcLengths)
        {
            _arcLengths = arcLengths;
        }

        public override int Compare(Player x, Player y)
        {
            if (_arcLengths[x.ID] < _arcLengths[y.ID])
                return 1;
            else return -1;
        }
    }

    public void UpdateRaceProgress()
    {
        // Update car arc-lengths
        float[] arcLengths = new float[_players.Count];

        for (int i = 0; i < _players.Count; ++i)
        {
            arcLengths[i] = ComputeCarArcLength(i);
        }

        _players.Sort(new PlayerInfoComparer(arcLengths));

        string myRaceOrder = "";
        foreach (var player in _players)
        {
            myRaceOrder += player.Name + " ";
        }

        Debug.Log("Race order: " + myRaceOrder);
    }

    float ComputeCarArcLength(int id)
    {
        // Compute the projection of the car position to the closest circuit 
        // path segment and accumulate the arc-length along of the car along
        // the circuit.
        Vector3 carPos = this._players[id].car.transform.position;

        float minArcL =
            this._circuitController.ComputeClosestPointArcLength(carPos, out _, out var carProj, out _);

        this._debuggingSpheres[id].transform.position = carProj;

        if (this._players[id].CurrentLap == 0)
        {
            minArcL -= _circuitController.CircuitLength;
        }
        else
        {
            minArcL += _circuitController.CircuitLength *
                       (_players[id].CurrentLap - 1);
        }

        return minArcL;
    }
}
