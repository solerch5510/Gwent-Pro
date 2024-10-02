using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Numerics;

public class CardDisplay : MonoBehaviour
{
    public Card2 card;

    public InteractableObject interactableObject;

    public static List<GameObject> selectedCards = new List<GameObject>();

    private static GameObject lastSelectedCard = null;
    
    public Text nameText;
    public Text descriptionText;

    public Image artImage;
    public Text powerText;

    public Text classCardText;

    public Interpreter interpreter;

    public string Zone ;
    public bool ActivateOnClick = true;

    public int cardFaction;


    //Use this for initialization

    public void Start()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;
        classCardText.text = card.classCard;
        artImage.sprite = card.spriteImage;
        Zone = card.Zone;

        cardFaction=card.Faction;
      
        powerText.text = card.power.ToString();

        interpreter = card.interpreter;
    }

    public void OnHoverEnter()
    {
        CardZoom.ShowComponents(gameObject.GetComponent<CardDisplay>());
    }

    public void OnHoverExit()
    {
        CardZoom.HideComponents();
    }

    public void OnClick()
    {
        if (ActivateOnClick == true)
        {
            if (gameObject == lastSelectedCard)
            {
                // No hacer nada si la carta seleccionada es la misma.

                return;
            }     

         //Agregar la carta a la lista de cartas seleccionadas
         selectedCards.Add(gameObject);

         // Actualizar la ultima carta seleccionada
         lastSelectedCard = gameObject;

         CardZoom.HideComponents();

         //Verificar si se han seleccionado dos cartas 
         if ( selectedCards.Count == 2)
         {
            foreach (var cardDisplay in FindObjectsOfType<CardDisplay>())
            {
                cardDisplay.ActivateOnClick = false;
            } 
            // Llamar la funcion para intercambiar las cartas con el deck
            CardReplacement.ReplaceSelectedCardsWithDeckCards(selectedCards, gameObject.GetComponent<CardDisplay>().card.playerID);

            //Limpiar la lista de cartas selecionadas.
            selectedCards.Clear(); 
         }

         
        
        }
    }

    public void ActivateDragDropOnAllCards()
    {
        //Buscar todos los objetos en la escena que tienen el componente CardDisplay
        CardDisplay[] allCards = FindObjectsOfType<CardDisplay>();

        foreach (CardDisplay card in allCards)
        {
            //Activar el componente DragDrop en cada carta
            DragDrop dragDropComponent = card.GetComponent<DragDrop>();
            
            if (dragDropComponent != null)
            {
                dragDropComponent.enabled = true;
            }
        }
    }

}
