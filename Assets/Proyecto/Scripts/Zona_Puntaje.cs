using UnityEngine;

public class Zona_Puntaje : MonoBehaviour
{
    [Header("Configuración de Puntos")]
    [Tooltip("El valor base del sector (ej: 20, 19, 1, etc.)")]
    public int puntosBase = 1;

    [Tooltip("Multiplicador del sector: 1 = Normal, 2 = Doble, 3 = Triple")]
    public int multiplicador = 1;

    /// <summary>
    /// Devuelve el puntaje total obtenido en esta sección.
    /// </summary>
    public int CalcularPuntaje()
    {
        return puntosBase * multiplicador;
    }
}