using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra al trigger tiene el componente Player
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            // Incrementar la cuenta de vueltas del jugador
            player.CurrentLap++;
            Debug.Log(player.Name + " completed lap: " + player.CurrentLap);
        }
    }
}
