using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{

    public static RaceController Instance;

    public int totalLaps = 3;
    public int numPlayers;

    private readonly List<Player> _players = new(4);
    private CircuitController _circuitController;
    private GameObject[] _debuggingSpheres;

    public delegate void RaceFinishedHandler();
    public event RaceFinishedHandler OnRaceFinished;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_circuitController == null) _circuitController = GetComponent<CircuitController>();

        _debuggingSpheres = new GameObject[GameManager.Instance.numPlayers];
        for (int i = 0; i < GameManager.Instance.numPlayers; ++i)
        {
            _debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void Update()
    {
        if (_players.Count == 0)
            return;

        UpdateRaceProgress();
        CheckPlayersCompletion();
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

        //orden de carrera (llamar solo cuando haya un adelantamiento o algo haya cambiado)
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

    private void CheckPlayersCompletion()
    {
        bool allPlayersFinished = true;

        foreach (var player in _players)
        {
            if (player.CurrentLap <= totalLaps)
            {
                allPlayersFinished = false;
                break;
            }
        }

        if (allPlayersFinished)
        {
            Debug.Log("All players finished!");
            OnRaceFinished?.Invoke();
            // Aquí puedes llamar a un método para finalizar la carrera, mostrar los resultados, etc.
        }
    }
}