using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DardoPegajoso : MonoBehaviour
{
    [Tooltip("El tag del tablero u objeto donde el dardo debe pegarse")]
    public string tagTablero = "Tablero";

    private Rigidbody rb;
    private bool yaPegado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision colision)
    {
        // Evitamos que el código se ejecute varias veces si ya se pegó
        if (yaPegado) return;
        Debug.Log("Colisión detectada con: " + colision.gameObject.name);
        // Comprobamos si el objeto impactado tiene el tag definido
        if (colision.gameObject.CompareTag(tagTablero))
        {
            yaPegado = true;

            // Frenamos cualquier inercia residual


            // Activamos isKinematic para anular la gravedad y dejarlo fijo
            rb.isKinematic = true;
        }
    }
}