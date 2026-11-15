using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CajaDardos : MonoBehaviour
{
    [SerializeField] GameObject dardoPrefab;
    [SerializeField] XRSimpleInteractable caja;

    void OnEnable() => caja.selectEntered.AddListener(AlAgarrar);
    void OnDisable() => caja.selectEntered.RemoveListener(AlAgarrar);

    void AlAgarrar(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;
        var manager = caja.interactionManager;

        var dardo = Instantiate(dardoPrefab, interactor.transform.position, interactor.transform.rotation);
        var grab = dardo.GetComponent<XRGrabInteractable>();

        // Suelta la caja y le pone el dardo en la mano
        manager.SelectExit(interactor, caja);
        manager.SelectEnter(interactor, grab);
    }
}