using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardZoom : MonoBehaviour
{
    public GameObject Canvas;

    private GameObject zoomCard; // Variable para almacenar el objeto instanciado
    

    public void Awake()
    {
        Canvas = GameObject.Find("MainCanvas");
        
    }

    public void OnHoverEnterCard(GameObject cardPrefab)
    {
        if (zoomCard == null) // Verificar si el objeto ya está instanciado

        {
             // Instanciar el objeto de juego solo si no está instanciado
            zoomCard = Instantiate(cardPrefab, new Vector2(-378, 241), Quaternion.identity);
            zoomCard.transform.SetParent(Canvas.transform, false);

            AdjustScale(zoomCard, cardPrefab);
        }

        AdjustScale(zoomCard, cardPrefab);
    }

    private void AdjustScale(GameObject zoomCard, GameObject cardPrefab) // Ajustar la escala del objeto instanciado y de sus hijos
    {
        RectTransform rect = zoomCard.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(285, 480);

        foreach (Transform child in zoomCard.transform)
        {
            float scaleFactorX = rect.sizeDelta.x / cardPrefab.GetComponent<RectTransform>().sizeDelta.x;
            float scaleFactorY = rect.sizeDelta.y / cardPrefab.GetComponent<RectTransform>().sizeDelta.y;

            child.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);
        }
    }

        
    public void OnHoverExit()
    {
        if (zoomCard != null)
        {
            Destroy(zoomCard);
            zoomCard = null;
        }
    }
}
