using UnityEngine;

public class Rocola : MonoBehaviour
{
    public AudioSource fuente;
    public AudioClip[] canciones;

    private int indice = -1;

    // Pasa a la siguiente canción y vuelve a la primera al terminar la lista
    [ContextMenu("Probar siguiente canción")]
    public void SiguienteCancion()
    {
        Debug.Log("Botón apretado");

        if (canciones.Length == 0)
        {
            Debug.Log("Lista de canciones vacía");
            return;
        }

        if (fuente == null)
        {
            Debug.Log("Falta asignar el Audio Source en el campo Fuente");
            return;
        }

        indice = (indice + 1) % canciones.Length;
        fuente.clip = canciones[indice];
        fuente.Play();
    }

    [ContextMenu("Probar detener")]
    public void Detener()
    {
        fuente.Stop();
    }
}



/*using UnityEngine;

public class Rocola : MonoBehaviour
{
    public AudioSource fuente;
    public AudioClip[] canciones;

    private int indice = -1;

    // Pasa a la siguiente canción y vuelve a la primera al terminar la lista
     
    public void SiguienteCancion()
    {
        if (canciones.Length == 0) return;

        indice = (indice + 1) % canciones.Length;
        fuente.clip = canciones[indice];
        fuente.Play();
    }

    public void Detener()
    {
        fuente.Stop();
    }
}*/