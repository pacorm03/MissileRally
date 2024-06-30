using UnityEngine;
using Unity.Netcode;
using Cinemachine;


public class Player : NetworkBehaviour
{
    // Player Info
    public string Name { get; set; }
    public int ID { get; set; }

    // Race Info
    public GameObject car;
    public int CurrentPosition { get; set; }
    public int CurrentLap { get; set; }


    public CinemachineVirtualCamera virtualCamera;
    //Network variable porque queremos guardar las posiciones por si un jugador se conecta mas tarde, y asi tener guardada la posición que le tocaría
    public NetworkVariable<Vector3> spawnPosition = new NetworkVariable<Vector3>();


    GameObject camera;

    private void Start()
    {
        //buscamos la camara
        camera = GameObject.Find("VirtualCamera");
        virtualCamera = camera.GetComponent<CinemachineVirtualCamera>();
        // Mover el player a la posición de spawn recibida del servidor
        transform.position = spawnPosition.Value;
        
        //llamamos al evento que asignará la posición de aparición
        spawnPosition.OnValueChanged += OnSpawnPositionChanged;

        //aparecer en la carrera
        GameManager.Instance.currentRace.AddPlayer(this);
         if (IsOwner)
        {
              // asignamos el transform del coche actual
              virtualCamera.Follow = car.transform;
              virtualCamera.LookAt = car.transform;
              //Prioridad de la camara
              virtualCamera.Priority = 10;
            }
            else
            {
                Debug.LogError("Main camera not found in the scene!");
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

}