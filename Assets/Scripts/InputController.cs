using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputController : NetworkBehaviour
{
    private CarController car;
    Transform _playerTransform;
    Vector2 _movement;
    float _brake;



    //variables qde red que se actualizaran en todos los clientes cuando cambien
    NetworkVariable<float> _speed = new NetworkVariable<float>();
    NetworkVariable<float> _rotSpeed = new NetworkVariable<float>();



    private void Start()
    {
        car = GetComponent<Player>().car.GetComponent<CarController>();
        _playerTransform = transform;
        //para que detecte el jugador que controlas segun tu build (y no puedas manejar el de otro)
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }


        //si cambia llamamos al evento
        _speed.OnValueChanged += OnSpeedChanged;
        _rotSpeed.OnValueChanged += OnRotSpeedChanged;

    }

    //eventos que aplican los nuevos valores
    private void OnSpeedChanged(float previousValue, float newValue)
    {
        if (IsServer) return;
        _speed.Value = car._currentSpeed;
    }
    private void OnRotSpeedChanged(float previousValue, float newValue)
    {
        if (IsServer) return;
        _rotSpeed.Value = car.maxSteeringAngle;
    }

    private void FixedUpdate()
    {
        //Procesar movimientos en el servidor
        if (!IsServer) return;
        //Actualizamos los movimientos del coche (en este caso no puede ser playertransform, ya que el coche no se mueve de manera uniforme)
        //como car tambien tiene un network transform, al  notar este cambio en el servidor, se actualiza en todos los clientes
        car.InputAcceleration = _movement.y;
        car.InputSteering = _movement.x;

    }



    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) ;
        _movement = context.ReadValue<Vector2>();
        //llamamos a la función que se ejecutará en el servidor
        onMoveServerRpc(_movement);
        
    }



    public void OnBrake(InputAction.CallbackContext context)
    {
        if (!IsOwner) ;
        _brake = context.ReadValue<float>();
        OnBrakeServerRpc(_brake);
    }

    //Se ejecuta en el servidor
    [ServerRpc]
    void onMoveServerRpc(Vector2 input)
    {
        _movement = input;
        car.InputAcceleration = _movement.y;
        car.InputSteering = _movement.x;
        _speed.Value = car._currentSpeed;
        _rotSpeed.Value = car.maxSteeringAngle;
    }


    [ServerRpc]
    public void OnBrakeServerRpc(float input)
    {
        car.InputBrake = input;
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
    }
}