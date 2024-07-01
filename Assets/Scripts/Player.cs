using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using TMPro;
using System.Collections.Generic;
using System.Threading;
using System.Collections;


public class Player : NetworkBehaviour
{
    // Player Info
    public string Name { get; set; }
    public int ID { get; set; }

    // Race Info
    public GameObject car;
    public int CurrentPosition { get; set; }
    private int _currentLap;
    public int CurrentLap
    {
        get => _currentLap;
        set
        {
            _currentLap = value;
            UpdateLapCounter();
            if (_currentLap > 0)
            {
                // Save lap time
                LastLapTime = Time.time - lapStartTime;
                lapStartTime = Time.time;
                // Optionally: save lap times in a list
                lapTimes.Add(LastLapTime);
            }
        }
    }

    public CinemachineVirtualCamera virtualCamera;
    //Network variable porque queremos guardar las posiciones por si un jugador se conecta mas tarde, y asi tener guardada la posición que le tocaría
    public NetworkVariable<Vector3> spawnPosition = new NetworkVariable<Vector3>();

    GameObject camera;

    private TextMeshProUGUI lapCounterText; // Referencia al contador de vueltas del HUD
    private TextMeshProUGUI lapTimeText;
    private TextMeshProUGUI totalTimeText;

    // Timing
    private float raceStartTime;
    private float lapStartTime;
    public float LastLapTime { get; private set; }
    public float TotalTime { get; private set; }
    private List<float> lapTimes = new List<float>();

    GameObject semaphore;
    private float initialFOV = 60f;
    private float wideFOV = 90f;
    private float transitionDuration = 1f;

    private void Start()
    {
        // Initialize lap counter
        CurrentLap = 0;

        // Initialize timing
        raceStartTime = Time.time;
        lapStartTime = Time.time;

        // Buscamos y asignamos la camara
        camera = GameObject.Find("VirtualCamera");
        virtualCamera = camera.GetComponent<CinemachineVirtualCamera>();

        // Mover el player a la posición de spawn recibida del servidor
        transform.position = spawnPosition.Value;
        
        // Llamamos al evento que asignará la posición de aparición
        spawnPosition.OnValueChanged += OnSpawnPositionChanged;

        // Aparecer en la carrera
        GameManager.Instance.currentRace.AddPlayer(this);

        // Asignar la cámara al coche del jugador inicialmente
        if (IsOwner)
        {
            virtualCamera.Follow = car.transform;
            virtualCamera.LookAt = car.transform;
            virtualCamera.Priority = 10;
            virtualCamera.m_Lens.FieldOfView = wideFOV;

            // Escuchar el evento del inicio de la carrera
            GameManager.Instance.OnRaceStart += HandleRaceStart;
        }
        else
        {
            Debug.LogError("Main camera not found in the scene!");
        }

        // Find the UI text objects
        lapCounterText = GameObject.Find("LapCounterText").GetComponent<TextMeshProUGUI>();
        lapTimeText = GameObject.Find("LapTimeText").GetComponent<TextMeshProUGUI>();
        totalTimeText = GameObject.Find("TotalTimeText").GetComponent<TextMeshProUGUI>();

        UpdateLapCounter();
    }

    private void HandleRaceStart()
    {
        StartCoroutine(TransitionToNormalFOV());
    }

    private IEnumerator TransitionToNormalFOV()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(wideFOV, initialFOV, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        virtualCamera.m_Lens.FieldOfView = initialFOV;
    }

    private void Update()
    {
        if (IsOwner && CurrentLap > 0)
        {
            // Update lap time
            float currentLapTime = Time.time - lapStartTime;
            lapTimeText.text = "Lap Time: " + FormatTime(currentLapTime);

            // Update total time
            TotalTime = Time.time - raceStartTime;
            totalTimeText.text = "Total Time: " + FormatTime(TotalTime);
        }
    }

    private void UpdateLapCounter()
    {
        if (lapCounterText != null && IsOwner)
        {
            lapCounterText.text = "Lap: " + CurrentLap;
        }
    }

    private void OnSpawnPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        //En el cliente se le asigna la nueva posicion
        if (!IsServer)
        {
            // Solo los clientes deben actualizar la posición cuando cambia
            transform.position = newPosition;
        }
    }

    public override string ToString()
    {
        return Name;
    }

    public override void OnNetworkSpawn()
    {
        //cuando spawnea, la posición se modifica en el cliente a su posición de spawn correspondiente
        if (!IsServer) return;
        transform.position = spawnPosition.Value;
       

    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
    }
}