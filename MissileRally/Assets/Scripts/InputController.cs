using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private CarController car;

    private void Start()
    {
        // Asegúrate de que este script está en el prefab del coche instanciado
        car = GetComponent<Player>().car.GetComponent<CarController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        if (car != null)
        {
            car.InputAcceleration = input.y;
            car.InputSteering = input.x;
        }
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<float>();
        if (car != null)
        {
            car.InputBrake = input;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // Funcionalidad de ataque, si aplica
    }
}
