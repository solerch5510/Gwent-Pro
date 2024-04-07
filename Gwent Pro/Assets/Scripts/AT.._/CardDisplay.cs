using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card2 card;

    public InteractableObject interactableObject;

    public Text nameText;
    public Text descriptionText;

    public Image artImage;
    public Text powerText;

    public Text classCardText;

    public string Zone ;


    public static bool staticCardBack;


    //Use this for initialization

    void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;
        classCardText.text = card.classCard;
        artImage.sprite = card.spriteImage;
        Zone = card.Zone;
      
        powerText.text = card.power.ToString();
    }

}
