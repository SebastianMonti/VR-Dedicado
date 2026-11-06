using UnityEngine;
using TMPro; // Habilita la compatibilidad con TextMeshPro

public class PuntajeManager : MonoBehaviour
{
    public static PuntajeManager Instance { get; private set; }

    [Header("Estadísticas del Juego")]
    public int puntajeTotal = 0;
    public int dardosTirados = 0;

    [Header("Referencias del Cartel (TextMeshPro)")]
    [Tooltip("Arrastrá acá el objeto Text (TMP) de Puntos")]
    public TextMeshProUGUI textoPuntaje;

    [Tooltip("Arrastrá acá el objeto Text (TMP) de Dardos")]
    public TextMeshProUGUI textoDardos;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ActualizarUI();
    }

    /// <summary>
    /// Suma los puntos del tiro y aumenta el contador de dardos.
    /// </summary>
    public void RegistrarTiro(int puntos)
    {
        puntajeTotal += puntos;
        dardosTirados++;
        ActualizarUI();
    }

    /// <summary>
    /// Cuenta el dardo tirado si pega fuera del tablero.
    /// </summary>
    public void RegistrarFallo()
    {
        dardosTirados++;
        ActualizarUI();
    }

    /// <summary>
    /// Reinicia los valores del cartel.
    /// </summary>
    public void ReiniciarJuego()
    {
        puntajeTotal = 0;
        dardosTirados = 0;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoPuntaje != null)
        {
            textoPuntaje.text = "Puntos: " + puntajeTotal;
        }

        if (textoDardos != null)
        {
            textoDardos.text = "Dardos: " + dardosTirados;
        }
    }
}