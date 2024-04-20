using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class DragDrop : MonoBehaviour
{
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private GameObject dropZone;
    private GameObject startParent;
    private Vector2 startPosition;
    public InteractableObject interactableObject;


    



    
    // Update is called once per frame
    void Update()
    {
        if(isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x , Input.mousePosition.y);

        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.GetComponent<CardDisplay>().card.classCard == "Decoy Card")
        {
            CardDisplay collidedCardDisplay = collision.gameObject.GetComponent<CardDisplay>();

            if (collidedCardDisplay != null)
            {
                // Verifica si la carta con la que se esta colisionando cumple con las restricciones del Decoy (No chocar con cartas de oro, tener el mismo playerID, que la carta no este en la mano...)
                if(IsValidCollision(collidedCardDisplay))
                {
                    //Logica para manejar la colision valida
                    HandleValidCollision(collidedCardDisplay);

                    gameObject.GetComponent<DragDrop>().enabled = false;
                }
            }

            return;
        }
        isOverDropZone = true;

        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;

        dropZone = null;
    }

    public void StartDrag()
    {

        // Desactivar el GridLayoutGroup al comenzar el arrastre
        FindObjectOfType<DrawCard>().DisableGridLayoutGroup();

        startParent = transform.parent.gameObject;

        startPosition = transform.position;

        isDragging = true;
    }
    
    public void EndDrag()
    {
        isDragging = false;

        if(isOverDropZone && gameObject.GetComponent<CardDisplay>().card.classCard == "Decoy Card")
        {
            
            if(gameObject.GetComponent<CardDisplay>().card.whichEffectIs != 6 && gameObject.GetComponent<CardDisplay>().card.whichEffectIs != 7)
            {
                GameManager.Effects(gameObject.GetComponent<CardDisplay>().card.whichEffectIs);
            }
            
            GameManager.passTurn(gameObject, gameObject.GetComponent<CardDisplay>().card.playerID);

            EventTrigger dragDrop = gameObject.GetComponent<EventTrigger>();

            dragDrop.enabled = false;

        }

        else if (isOverDropZone && WichZoneIs() )
        {
            transform.SetParent(dropZone.transform, false);

            if(gameObject.GetComponent<CardDisplay>().card.whichEffectIs != 6 && gameObject.GetComponent<CardDisplay>().card.whichEffectIs != 7)
            {
                GameManager.Effects(gameObject.GetComponent<CardDisplay>().card.whichEffectIs);
            }
            
            GameManager.passTurn(gameObject, gameObject.GetComponent<CardDisplay>().card.playerID);

            EventTrigger dragDrop = gameObject.GetComponent<EventTrigger>();

            dragDrop.enabled = false;        
        }

        else 
        {
            transform.position = startPosition;

            transform.SetParent(startParent.transform, false); 
        }

        //Reactivar el GridLayoutGroup al terminar arrastre
        FindObjectOfType<DrawCard>().EnableGridLayoutGroup();
    }

    public bool WichZoneIs ()  // Metodo para que la carta solo colisione con su zona correspondiente.
    {
        Zones conditions = dropZone.GetComponent<Zones>();
        
        string k = conditions.ZoneNames;
        
        string l = gameObject.GetComponent<CardDisplay>().Zone;
        
        if (k==l) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool IsValidCollision(CardDisplay collidedCardDisplay)
    {
        //Verifica si la carta con la que se esta colisionando es de calse "SilverCard"
        Players players = GameObject.FindAnyObjectByType<Players>();

        if (collidedCardDisplay.card.classCard != "SilverCard")
        {
            return false;
        }

        // Verifica si la carta con la que se esta colisionando tiene el mismo playerID
        if(collidedCardDisplay.card.playerID != GetComponent<CardDisplay>().card.playerID)
        {
            return false;
        }

        // Verifica si la carta con la que se esta colisionando no pertenece a ninguna de las listas de cartas de los jugadores
        if (players.Player1.Cards.Contains(collidedCardDisplay.gameObject) || players.Player2.Cards.Contains(collidedCardDisplay.gameObject))
        {
            return false;
        }

        return true;
    }

    void HandleValidCollision(CardDisplay collidedCardDisplay)
    {
        Players players = GameObject.FindAnyObjectByType<Players>();

        //Logica para manejar la colision valida

        GameObject collidedGameObject = collidedCardDisplay.gameObject;

        Transform Hand = collidedGameObject.transform.parent;

        Hand.GetComponent<GridLayoutGroup>().enabled = false;

        Hand.GetComponent<GridLayoutGroup>().enabled = true;

        collidedCardDisplay.transform.SetParent(transform.parent.gameObject.transform, false);

        collidedCardDisplay.card.power = collidedCardDisplay.card.basedPower;

        collidedCardDisplay.powerText.text = collidedCardDisplay.card.power.ToString();

        collidedCardDisplay.gameObject.GetComponent<EventTrigger>().enabled = true;

        if (collidedCardDisplay.card.playerID)
        {
            players.Player1.Cards.Add(collidedCardDisplay.gameObject);
        }

        else
        {
            players.Player2.Cards.Add(collidedCardDisplay.gameObject);
        }

        gameObject.transform.SetParent(Hand.transform, false);

        gameObject.GetComponent<EventTrigger>().enabled = false;

        GameManager.passTurn(gameObject , gameObject.GetComponent<CardDisplay>().card.playerID);
    }
}
