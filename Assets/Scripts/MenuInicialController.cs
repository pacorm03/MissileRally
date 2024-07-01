using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuInicialController : MonoBehaviour
{
    public Dropdown playerDropdown;  // Campo público para el Dropdown
    public Button confirmButton;     // Campo público para el Button

    private static int numberOfPlayers;

    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        playerDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnConfirm()
    {
        SceneManager.LoadScene("PlayerSetup");     // Cambia a la escena de configuración de jugadores
    }

    void OnDropdownValueChanged(int value)
    {
        numberOfPlayers = value + 1; // Dropdown value is 0-based, so add 1
    }

    public static int GetNumberOfPlayers()
    {
        return numberOfPlayers;
    }
}
