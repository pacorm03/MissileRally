using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputController : NetworkBehaviour
{
    private CarController car;

    private void Start()
    {
        car = GetComponent<Player>().car.GetComponent<CarController>();

        //para que detecte el jugador que controlas segun tu build (y no puedas manejar el de otro)
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        car.InputAcceleration = input.y;
        car.InputSteering = input.x;
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<float>();
        car.InputBrake = input;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
    }
}