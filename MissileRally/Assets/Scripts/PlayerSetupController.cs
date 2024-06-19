using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSetupController : MonoBehaviour
{
    public Button confirmButton;  // Botón para confirmar
    public GameObject[] playerPanels; // Arreglo que contiene todos los paneles de jugador

    private int numberOfPlayers;
    private PlayerData[] playersData; // Arreglo para almacenar los datos de los jugadores

    void Start()
    {
        numberOfPlayers = MenuInicialController.GetNumberOfPlayers();
        playersData = new PlayerData[numberOfPlayers]; // Inicializar el arreglo con el tamaño correcto

        // Activar solo los paneles necesarios en orden
        for (int i = 0; i < numberOfPlayers; i++)
        {
            playerPanels[i].SetActive(true);

            // Depuración
            Debug.Log("Buscando objetos dentro de PlayerPanel[" + i + "]");
            Transform playerNameInputTransform = playerPanels[i].transform.Find("PlayerNameInput");
            if (playerNameInputTransform != null)
            {
                Debug.Log("PlayerNameInput encontrado en PlayerPanel[" + i + "]");
            }
            else
            {
                Debug.LogError("PlayerNameInput NO encontrado en PlayerPanel[" + i + "]");
            }

            Transform colorDropdownTransform = playerPanels[i].transform.Find("ColorDropdown");
            if (colorDropdownTransform != null)
            {
                Debug.Log("ColorDropdown encontrado en PlayerPanel[" + i + "]");
            }
            else
            {
                Debug.LogError("ColorDropdown NO encontrado en PlayerPanel[" + i + "]");
            }

            Transform playerTitleTransform = playerPanels[i].transform.Find("Text");
            if (playerTitleTransform != null)
            {
                Debug.Log("Text encontrado en PlayerPanel[" + i + "]");
            }
            else
            {
                Debug.LogError("Text NO encontrado en PlayerPanel[" + i + "]");
            }

            // Fin de depuración

            PlayerData playerData = new PlayerData();
            playerData.nameInput = playerNameInputTransform.GetComponent<InputField>();
            playerData.colorDropdown = colorDropdownTransform.GetComponent<Dropdown>();
            playerData.titleText = playerPanels[i].GetComponentInChildren<Text>(); // Cambio aquí
            playersData[i] = playerData;

            // Asegurar que los componentes del panel están activos
            playerData.nameInput.gameObject.SetActive(true);
            playerData.colorDropdown.gameObject.SetActive(true);
            playerData.titleText.gameObject.SetActive(true);
        }

        // Desactivar todos los paneles restantes
        for (int i = numberOfPlayers; i < playerPanels.Length; i++)
        {
            playerPanels[i].SetActive(false);
        }

        confirmButton.onClick.AddListener(GoToCircuitSelection);
    }

   /* void StartGame()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            string playerName = playersData[i].nameInput.text;
            int playerColorIndex = playersData[i].colorDropdown.value;
            Debug.Log("Player " + (i + 1) + ": Name = " + playerName + ", Color = " + playerColorIndex);
            // Aquí puedes guardar los datos de los jugadores para su uso posterior
        }

        // Cambia a la escena del juego
        SceneManager.LoadScene("GameScene");
    }*/

    void GoToCircuitSelection()
    {
        SceneManager.LoadScene("CircuitSelection");
    }
}

[System.Serializable]
public class PlayerData
{
    public InputField nameInput;  // Campo de texto para el nombre del jugador
    public Dropdown colorDropdown;  // Dropdown para seleccionar el color del jugador
    public Text titleText;  // Texto para el título del jugador
}


