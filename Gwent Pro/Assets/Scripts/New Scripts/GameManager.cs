using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    
    public  GameObject readyToPLay1;
    public GameObject readyToPLay2; 

    public GameObject Board;
    public DrawCard drawCard;

    public DrawCard2 drawCard2;

    public static void passTurn (GameObject thisCard ,bool playerID)
    {
        //Obtener la referencia a Players y a la lista de cartas de cada uno.
        Players players = GameObject.FindObjectOfType<Players>();

        List<GameObject> player1Cards = players.Player1.Cards;

        List<GameObject> player2Cards = players.Player2.Cards;

        if(playerID == true) // Si la carta es del player uno entonces:
        {
            player1Cards.Remove(thisCard); //Quita la carta que colisiono de la lista de player1

            foreach(GameObject card in player1Cards) //Desactiva las cartas de la mano
            {
                card.SetActive(false);
            }

            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            gameManager.readyToPLay2.SetActive(true); // Activa el readyToPlay2 del player2
        }

        else // si la carta es del Player2 entonces:
        {
            player2Cards.Remove(thisCard); //Quita la carta de la lista de Player2

            foreach(GameObject card in player2Cards) //Desactiva las cartas de la mano (Del player2)
            {
                card.SetActive(false);
            }

            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            gameManager.readyToPLay1.SetActive(true);//Activa el readyToPlay1 del Player1.

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
