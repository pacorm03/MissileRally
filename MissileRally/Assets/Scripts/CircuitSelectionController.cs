using UnityEngine;
using UnityEngine.SceneManagement;

public class CircuitSelectionController : MonoBehaviour
{
    public void SelectCircuit(int circuitIndex)
    {
        // Guardar el �ndice del circuito seleccionado
        PlayerPrefs.SetInt("SelectedCircuit", circuitIndex);
        // Cargar la escena del juego
        SceneManager.LoadScene("GameScene");
    }
}

