using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardReplacement : MonoBehaviour
{

    public static void ReplaceSelectedCardsWithDeckCards(List<GameObject> selectedCards, bool playerID)
    {
        //Obtener la referencia a Players y a la lista de cartas del jugador
        Players players = GameObject.FindObjectOfType<Players>();

        List<GameObject> playerCards1 = players.Player1.Cards; // Trabajando con la lista de cartas del player1

        List<GameObject> playerCards2 = players.Player2.Cards; // Trabajando con la lista de cartas del player2

        

        if(playerID == true)
        {

            //Eliminar las cartas seleccionadas de la lista del jugador y de la escena
            foreach (GameObject card in selectedCards)
            {
                playerCards1.Remove(card);

                Destroy(card);
            }

            DrawCard drawCard = GameObject.Find("GameManager").GetComponent<DrawCard>(); // Obtener la referencia al script DrawCard en el GameObject GameManager

            drawCard.drawnTwoCards();

            CardDisplay cardDisplay = GameObject.Find("Void").GetComponent<CardDisplay>();
            
            cardDisplay.ActivateDragDropOnAllCards();

            foreach (GameObject card in playerCards1) //Borrar todas las cartas de la mano del jugador de la escena
            {
                card.SetActive(false); //Desactivar la carta
            }

            //Obtener la referencia al objeto GameManager
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            //Activar el readyToPlay2
            gameManager.readyToPLay2.SetActive(true);
        }

        else
        {

            //Eliminar las cartas seleccionadas de la lista del jugador y de la escena
            foreach (GameObject card in selectedCards)
            {
                playerCards2.Remove(card);

                Destroy(card);
            }

            DrawCard2 drawCard2 = GameObject.Find("GameManager").GetComponent<DrawCard2>(); // Obtener la referencia al script DrawCard en el GameObject GameManager

            drawCard2.drawnTwoCards();

            CardDisplay cardDisplay = GameObject.Find("Void").GetComponent<CardDisplay>();

            cardDisplay.ActivateDragDropOnAllCards();

            foreach (GameObject card in playerCards2) //Borrar todas las cartas de la mano del jugador de la escena
            {
                card.SetActive(false); //Desactivar la carta
            }

            //Obtener la referencia al objeto GameManager
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            //Activar el readyToPlay2
            gameManager.readyToPLay1.SetActive(true);
        }                
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
