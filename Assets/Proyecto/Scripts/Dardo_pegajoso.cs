using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class Dardo_pegajoso : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grab;
    private bool yaPegado = false;

    public static readonly List<Dardo_pegajoso> Todos = new List<Dardo_pegajoso>();

    void OnEnable() => Todos.Add(this);
    void OnDisable() => Todos.Remove(this);

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnCollisionEnter(Collision colision)
    {
        if (yaPegado) return;

        // Buscamos si el objeto impactado tiene el script Zona_Puntaje
        Zona_Puntaje zona = colision.gameObject.GetComponent<Zona_Puntaje>();
        if (zona == null) return;

        yaPegado = true;

        // 1. Calcular puntos del tiro
        int puntosGanados = zona.CalcularPuntaje();
        Debug.Log($"¡Dardo clavado! Puntos de la zona: {puntosGanados}");

        // 2. Sumar al gestor global si existe
        if (PuntajeManager.Instance != null)
        {
            PuntajeManager.Instance.RegistrarTiro(puntosGanados);
        }

        // 3. Hacer que ya no se pueda agarrar (esto también lo suelta si estaba en la mano)
        if (grab != null) grab.enabled = false;

        // 4. Inmovilizar la física del dardo
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // 5. Emparentar al objeto impactado para que se mueva junto con él
        transform.SetParent(colision.transform);
    }

    public static void EliminarTodos()
    {
        // Copia de la lista, porque al destruir cada dardo se saca de Todos
        foreach (var dardo in new List<Dardo_pegajoso>(Todos))
        {
            if (dardo != null) Destroy(dardo.gameObject);
        }
    }
}