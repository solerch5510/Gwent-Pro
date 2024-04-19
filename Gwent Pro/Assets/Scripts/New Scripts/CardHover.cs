using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string cardName; // Asigna el nombre de la carta en el Inspector
    public CardZoom cardZoom; // Referencia al script CardZoom

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardZoom.ShowCardZoom(cardName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardZoom.HideCardZoom();
    }
}