using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class PuntajeManager : MonoBehaviour
{
    public static PuntajeManager Instance { get; private set; }

    [Header("Estadísticas")]
    public int numeroRonda = 1;
    public int puntajeTotal = 0;
    public int puntajeRonda = 0;
    public int dardosTirados = 0;
    public int mejorTiro = 0;

    private const string CLAVE_RONDA = "PuntajeManager_NumeroRonda";

    [Header("Configuración")]
    [SerializeField] private int tirosPorRonda = 3;
    [SerializeField] private string nombreJugador = "Jugador 1";

    [Header("Referencias UI")]
    [SerializeField] private TMP_Text textoRonda;
    [SerializeField] private TMP_Text textoTiros;
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoMejor;

    private List<int> tirosRonda = new List<int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Recuperar la última ronda guardada.
        numeroRonda = PlayerPrefs.GetInt(CLAVE_RONDA, 1);

        ActualizarUI();

        Debug.Log("Partida reanudada en la ronda " + numeroRonda);
    }

    // Registra un tiro válido.
    public void RegistrarTiro(int puntos)
    {
        RegistrarResultadoTiro(puntos);
    }

    // Registra un tiro fallido.
    public void RegistrarFallo()
    {
        RegistrarResultadoTiro(0);
    }

    private void RegistrarResultadoTiro(int puntos)
    {
        // Evita registrar más de tres tiros.
        if (dardosTirados >= tirosPorRonda)
            return;

        tirosRonda.Add(puntos);

        puntajeRonda += puntos;
        puntajeTotal += puntos;
        dardosTirados++;

        if (puntos > mejorTiro)
            mejorTiro = puntos;

        ActualizarUI();

        // Si llegó al tercer tiro, termina la ronda.
        if (dardosTirados >= tirosPorRonda)
            CompletarRonda();
    }

    private void CompletarRonda()
    {
        // Guarda la ronda en el ranking.
        if (RankingBoard.Instance != null)
        {
            RankingBoard.Instance.AddEntry(
                nombreJugador,
                numeroRonda,
                tirosRonda.ToArray()
            );
        }
        else
        {
            Debug.LogWarning("No se encontró RankingBoard en la escena.");
        }

        Debug.Log("Ronda " + numeroRonda +
                  " completada. Puntos: " + puntajeRonda);

        // Prepara la siguiente ronda.
        numeroRonda++;
        puntajeRonda = 0;
        dardosTirados = 0;
        mejorTiro = 0;
        tirosRonda.Clear();

        // Guardar el progreso.
        PlayerPrefs.SetInt(CLAVE_RONDA, numeroRonda);
        PlayerPrefs.Save();

        ActualizarUI();
    }

    // Permite avanzar manualmente solo si se completaron los tiros.
    public void IniciarNuevaRonda()
    {
        if (dardosTirados < tirosPorRonda)
            Debug.Log("Todavía quedan tiros por realizar.");
    }

    // Reinicia la partida.
    public void ReiniciarJuego()
    {
        numeroRonda = 1;
        puntajeTotal = 0;
        puntajeRonda = 0;
        dardosTirados = 0;
        mejorTiro = 0;
        tirosRonda.Clear();

        ActualizarUI();

        Dardo_pegajoso.EliminarTodos();
    }

    private void ActualizarUI()
    {
        if (textoRonda != null)
        {
            textoRonda.text =
                "<size=65%><color=#B8C7D9>RONDA ACTUAL</color></size>\n" +
                "<size=150%><b>" + numeroRonda.ToString("00") + "</b></size>";
        }

        if (textoTiros != null)
        {
            textoTiros.text =
                "<size=65%><color=#B8C7D9>TIROS</color></size>\n" +
                "<size=130%><b>" + dardosTirados +
                " <size=70%>/ " + tirosPorRonda + "</size></b></size>";
        }

        if (textoPuntos != null)
        {
            textoPuntos.text =
                "<size=65%><color=#B8C7D9>PUNTOS</color></size>\n" +
                "<size=130%><b>" + puntajeRonda + "</b></size>";
        }

        if (textoMejor != null)
        {
            textoMejor.text =
                "<size=65%><color=#B8C7D9>MEJOR TIRO</color></size>\n" +
                "<size=130%><b>" + mejorTiro + "</b></size>";
        }
    }

    [Header("Modo de pruebas")]
    [SerializeField] private bool modoPrueba = true;

    private void Update()
    {
        if (!modoPrueba) return;

        // Agregar tiros con diferentes puntuaciones
        if (Input.GetKeyDown(KeyCode.F6))
            RegistrarTiro(20);

        if (Input.GetKeyDown(KeyCode.F7))
            RegistrarTiro(50);

        // Registrar un fallo
        if (Input.GetKeyDown(KeyCode.F8))
            RegistrarFallo();

        // Comenzar nueva ronda
        if (Input.GetKeyDown(KeyCode.F9))
            IniciarNuevaRonda();

        // Reiniciar partida
        if (Input.GetKeyDown(KeyCode.F10))
            ReiniciarJuego();
    }

    [ContextMenu("Pruebas/Reiniciar número de ronda")]
    public void ReiniciarNumeroRonda()
    {
        numeroRonda = 0;

        PlayerPrefs.SetInt(CLAVE_RONDA, numeroRonda);
        PlayerPrefs.Save();

        ActualizarUI();

        Debug.Log("Número de ronda reiniciado a 0.");
    }
}