using UnityEngine;
using UnityEngine.UI;

public class CardZoom : MonoBehaviour
{
    // Referencia al panel de UI que se usará para mostrar el zoom de la carta
    public GameObject zoomPanel;

    // Componente de UI para el nombre dentro del panel de zoom
    private Text nameText;
    private CanvasGroup canvasGroup;

    void Start()
    {
        // Inicialización del componente de UI para el nombre
        nameText = zoomPanel.transform.Find("ZoomNameText").GetComponent<Text>();
        canvasGroup = zoomPanel.GetComponent<CanvasGroup>();

        // Ocultar inicialmente el panel de zoom
        canvasGroup.alpha = 0;
    }

    // Método llamado cuando el cursor entra sobre una carta
    public void OnHoverEnterCard(string cardName)
    {
        // Actualizar el panel de zoom con el nombre de la carta
        nameText.text = cardName;

        // Mostrar el panel de zoom
        canvasGroup.alpha = 1;
    }

    // Método llamado cuando el cursor sale de la carta
    public void OnHoverExit()
    {
        // Ocultar el panel de zoom
        canvasGroup.alpha = 0;
    }
}
