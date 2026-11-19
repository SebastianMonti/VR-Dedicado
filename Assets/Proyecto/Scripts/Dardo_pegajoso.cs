using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Dardo_pegajoso : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grab;
    private bool yaPegado = false;

    public static readonly List<Dardo_pegajoso> Todos = new List<Dardo_pegajoso>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        Todos.Add(this);
        if (grab != null)
        {
            grab.selectEntered.AddListener(AlAgarrar);
            grab.selectExited.AddListener(AlSoltar);
        }
    }

    void OnDisable()
    {
        Todos.Remove(this);
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(AlAgarrar);
            grab.selectExited.RemoveListener(AlSoltar);
        }
    }

    void AlAgarrar(SelectEnterEventArgs args)
    {
        if (!yaPegado) return;

        transform.SetParent(null);
        rb.isKinematic = false;
        yaPegado = false;
    }

    void AlSoltar(SelectExitEventArgs args)
    {
        if (!yaPegado)
        {
            rb.isKinematic = false;
        }
    }

    void OnCollisionEnter(Collision colision)
    {
        Debug.Log(colision.gameObject.name);
        if (yaPegado) return;

        Zona_Puntaje zona = colision.gameObject.GetComponent<Zona_Puntaje>();
        if (zona == null) return;

        yaPegado = true;

        int puntosGanados = zona.CalcularPuntaje();
        Debug.Log($"¡Dardo clavado! Puntos de la zona: {puntosGanados}");

        if (PuntajeManager.Instance != null)
        {
            PuntajeManager.Instance.RegistrarTiro(puntosGanados);
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        transform.SetParent(colision.transform);
    }

    public static void EliminarTodos()
    {
        foreach (var dardo in new List<Dardo_pegajoso>(Todos))
        {
            if (dardo != null) Destroy(dardo.gameObject);
        }
    }
}