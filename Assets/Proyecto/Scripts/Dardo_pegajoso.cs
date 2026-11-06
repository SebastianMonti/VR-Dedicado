
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Dardo_pegajoso : MonoBehaviour
{
    private Rigidbody rb;
    private bool yaPegado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision colision)
    {
        if (yaPegado) return;

        // Buscamos si el objeto impactado tiene el script Zona_Puntaje
        Zona_Puntaje zona = colision.gameObject.GetComponent<Zona_Puntaje>();

        if (zona != null)
        {
            yaPegado = true;

            // 1. Calcular puntos del tiro
            int puntosGanados = zona.CalcularPuntaje();
            Debug.Log($"¡Dardo clavado! Puntos de la zona: {puntosGanados}");

            // 2. Sumar al gestor global si existe
            if (PuntajeManager.Instance != null)
            {
                PuntajeManager.Instance.RegistrarTiro(puntosGanados);
            }

            //3. Inmovilizar la física del dardo
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            // 4. Emparentar al objeto impactado para que se mueva junto con él
            transform.SetParent(colision.transform);
        }
    }

    /// <summary>
    /// Permite liberar el dardo si lo volvés a agarrar con la mano virtual.
    /// </summary>
    public void ResetearDardo()
    {
        yaPegado = false;
        rb.isKinematic = false;
        transform.SetParent(null);
    }
}