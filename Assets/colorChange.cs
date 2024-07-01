using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorChange : NetworkBehaviour
{
    public Material _material;
    private Material _materialInstance; // Instancia del material para cada coche
    int _colorNum;
    Color _colorNuevo;

    NetworkVariable<Color> _color = new NetworkVariable<Color>();

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
        // Crear una nueva instancia del material para este coche
        _materialInstance = new Material(_material);

        // Asignar la instancia del material al renderizador del coche
        GetComponent<Renderer>().material = _materialInstance;

        _color.OnValueChanged += OnColorChanged;
        //para que detecte el jugador que controlas segun tu build (y no puedas manejar el de otro)


        // Aplicar el color inicial
        if (IsServer)
        {
            _color.Value = _materialInstance.color; // Inicializar con el color actual del material
        }
        else
        {
            OnColorChanged(Color.clear, _color.Value); // Aplicar el color en los clientes
        }
    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        Debug.Log($"Color changed from {previousValue} to {newValue} on client.");
        _materialInstance.color = newValue;
    }

    public void OnColor(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        int numKeyValue;
        if (int.TryParse(context.control.name, out numKeyValue))
        {
            Debug.Log("int value of keypress is: " + numKeyValue);
            _colorNum = numKeyValue;
            onColorServerRpc(_colorNum);
        }
    }

    [ServerRpc]
    void onColorServerRpc(int input)
    {
        _colorNum = input;
        switch (_colorNum)
        {
            case 1:
                _color.Value = Color.blue;
                break;
            case 2:
                _color.Value = Color.red;
                break;
            case 3:
                _color.Value = Color.green;
                break;
            default:
                Debug.LogWarning("Color no válido");
                break;
        }

        Debug.Log($"Server changed color to {_color.Value}");
    }
}
