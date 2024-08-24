using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Numerics;
using Unity.VisualScripting;

public class CardZoom : MonoBehaviour
{
    public Card2 card;
    
    public Text nameText;
    public Text descriptionText;

    public Image artImage;
    public Text powerText;

    public Text classCardText;

    public string Zone ;   

    //Use this for initialization

    void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;
        classCardText.text = card.classCard;
        artImage.sprite = card.spriteImage;
        Zone = card.Zone;
      
        powerText.text = card.power.ToString();

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
    }

    public static void ShowComponents(CardDisplay ThisCard)
    {
        CanvasGroup canvasGroup1 = GameObject.FindAnyObjectByType<CardZoom>().GetComponent<CanvasGroup>();

        canvasGroup1.alpha = 1;

        CardZoom cardZoom = canvasGroup1.gameObject.GetComponent<CardZoom>();

        cardZoom.nameText.text = ThisCard.card.name;

        cardZoom.powerText.text = ThisCard.card.power.ToString();

        cardZoom.descriptionText.text = ThisCard.card.description;

        cardZoom.classCardText.text = ThisCard.card.classCard;

        cardZoom.artImage.sprite = ThisCard.card.spriteImage;
    }

    public static void HideComponents()
    {
        CanvasGroup canvasGroup1 = GameObject.FindAnyObjectByType<CardZoom>().GetComponent<CanvasGroup>();

        canvasGroup1.alpha = 0;
    }
}
