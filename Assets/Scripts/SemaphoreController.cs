using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class SemaphoreController : NetworkBehaviour
{
    private GameObject redLight;
    private GameObject yellowLight;
    private GameObject greenLight;

    private void Awake()
    {
        // Asegúrate de que los objetos estén asignados en Awake o Start
        redLight = GameObject.Find("LED1");
        yellowLight = GameObject.Find("LED2");
        greenLight = GameObject.Find("LED3");
    }

    private void Start()
    {
        

        if (IsServer || IsHost)
        {
            StartCoroutine(SemaphoreRoutine());
        }


    }

    private IEnumerator SemaphoreRoutine()
    {
        // Desactivar todas las luces al principio
        SetLights(false, false, false);
        // Start with red light
        UpdateLightsClientRpc(true, false, false);

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Change to yellow light
        UpdateLightsClientRpc(false, true, false);

        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Change to green light
        UpdateLightsClientRpc(false, false, true);

        // Signal that the race can start
        GameManager.Instance.StartRace();
    }

    [ClientRpc]
    private void UpdateLightsClientRpc(bool red, bool yellow, bool green)
    {
        SetLights(red, yellow, green);
    }

    private void SetLights(bool red, bool yellow, bool green)
    {
        redLight.SetActive(red);
        yellowLight.SetActive(yellow);
        greenLight.SetActive(green);
    }
}
