using UnityEngine;

public class TableroMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float amplitud = 1.5f;
    [SerializeField] private float velocidad = 1f;

    [Header("Estado")]
    [SerializeField] private bool movimientoActivo = true;

    private Vector3 posicionInicial;
    private float tiempoMovimiento;

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void Update()
    {
        if (!movimientoActivo)
            return;

        tiempoMovimiento += Time.deltaTime;

        float desplazamiento =
            Mathf.Sin(tiempoMovimiento * velocidad) * amplitud;

        transform.position = posicionInicial +
                             transform.right * desplazamiento;
    }

    // Activar movimiento
    public void ActivarMovimiento()
    {
        movimientoActivo = true;
    }

    // Desactivar movimiento y volver a la posición inicial
    public void DesactivarMovimiento()
    {
        movimientoActivo = false;
        ReiniciarMovimiento();
    }

    // Reiniciar posición y ciclo de movimiento
    public void ReiniciarMovimiento()
    {
        tiempoMovimiento = 0f;
        transform.position = posicionInicial;
    }

    // Alternar movimiento; si se desactiva, vuelve al inicio
    [ContextMenu("Movimiento/Alternar movimiento")]
    public void AlternarMovimiento()
    {
        movimientoActivo = !movimientoActivo;

        if (!movimientoActivo)
            ReiniciarMovimiento();
    }
}