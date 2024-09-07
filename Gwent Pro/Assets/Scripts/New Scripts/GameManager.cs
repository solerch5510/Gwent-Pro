using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool whichPlayerIs = true; // para controlar el flujo del juego (es decir que player esta jugando en el momento (true-player1 ; false-player2))
    public static bool endTurn1;
    public static bool endTurn2;   
    public static int pointsPlayer1;
    public static int pointsPlayer2; 
    public static int roundsWonByPlayer1;
    public static int roundsWonByPlayer2;
    public  GameObject readyToPLay1; 
    public GameObject readyToPLay2;
    public GameObject CongratulationsPlayer1;  
    public GameObject CongratulationsPlayer2;
    public GameObject trophy; // O chapita de ganar la ronda , como prefieras ... 
    public GameObject roundWinner;
    public GameObject mainCanvas;
    public GameObject Board;
    public DrawCard drawCard;
    public DrawCard2 drawCard2;
    public GameObject player1S;
    public GameObject player1R;
    public GameObject player1M;
    public GameObject player2S;
    public GameObject player2R;

    public GameObject player2M;
    public GameObject Graveyard1;
    public GameObject Graveyard2;

    public static void passTurn (GameObject thisCard ,bool playerID)
    {
        //Obtener la referencia a Players y a la lista de cartas de cada uno.
        Players players = GameObject.FindObjectOfType<Players>();

        List<GameObject> player1Cards = players.Player1.Cards;

        List<GameObject> player2Cards = players.Player2.Cards;

        if(playerID == true) // Si la carta es del player uno entonces:
        {
            player1Cards.Remove(thisCard); //Quita la carta que colisiono de la lista de player1

            if(player1Cards.Count == 0)
            {
                round();

                return;
            }

            if(endTurn2 == true) // Si player2 paso turno keep playing
        {
            return;
        }

            foreach(GameObject card in player1Cards) //Desactiva las cartas de la mano
            {
                card.SetActive(false);
            }

            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            gameManager.readyToPLay2.SetActive(true); // Activa el readyToPlay2 del player2

            whichPlayerIs = false;
        }

        else // si la carta es del Player2 entonces:
        {
            player2Cards.Remove(thisCard); //Quita la carta de la lista de Player2

            if(player2Cards.Count == 0)
            {
                round();

                return;
            }

            if(endTurn1 == true) // Si player1 paso turno keep playing
        {
            return;
        }          

            foreach(GameObject card in player2Cards) //Desactiva las cartas de la mano (Del player2)
            {
                card.SetActive(false);
            }

            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            gameManager.readyToPLay1.SetActive(true);//Activa el readyToPlay1 del Player1.

            whichPlayerIs = true;

        }
        
    }

    public static void round ()
    {
        //Obtener la referencia a Players y a la lista de cartas de cada uno.
        Players players = GameObject.FindObjectOfType<Players>();

        List<GameObject> player1Cards = players.Player1.Cards;

        List<GameObject> player2Cards = players.Player2.Cards;

        if(whichPlayerIs == true)
        {
            endTurn1 = true; // Player1 pasa turno

            foreach (GameObject card in player1Cards)
            {
                card.SetActive(false);
            }

            if(endTurn2 == false)
            {    

                GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

                gameManager.readyToPLay2.SetActive(true); // Activa el readyToPlaay2
            }
            
            whichPlayerIs = false;
        }

        else if(whichPlayerIs == false)
        {
            endTurn2 = true; // Player2 pasa turno

            foreach (GameObject card in player2Cards)
            {
                card.SetActive(false);
            }
            
            if(endTurn1 == false)
            {
                GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

                gameManager.readyToPLay1.SetActive(true); // Activa el readyToPlaay1
            }

            whichPlayerIs = true;
        }

        if((endTurn1 && endTurn2) == true) //Si la ronda acabo, desactiva todas las cartas de la escena.
        {
            //Buscar todas las cartas de la escena (componentes CardDisplay)
            CardDisplay[] allCards = GameObject.FindObjectsOfType<CardDisplay>();

            //Itera sobre cada CardDisplay en la escena
            foreach (CardDisplay cardDisplay in allCards)
            {
                // Restablece el poder de la carta a su basedPower
                cardDisplay.card.power = cardDisplay.card.basedPower;
            }
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            whoWinsTheRound();

            gameManager.roundWinner.SetActive(true); // Activa el roundWinner.

            gameManager.drawCard.drawnTwoCards();

            gameManager.drawCard2.drawnTwoCards();

            CardDisplay cardDisplay1 = GameObject.Find("Void").GetComponent<CardDisplay>();
            
            cardDisplay1.ActivateDragDropOnAllCards();

            foreach (GameObject card in player1Cards)
            {
                card.SetActive(false);
            }

            foreach(GameObject card in player2Cards)
            {
                card.SetActive(false);
            }        
        }

    }

    public static void whoWinsTheRound()
    {
        if(pointsPlayer1 > pointsPlayer2 || pointsPlayer1 == pointsPlayer2) // Si empatan o gana player1 ; Player1 gana la ronda
        {
            roundsWonByPlayer1++ ;

            GameManager gameManager = GameObject.FindAnyObjectByType<GameManager>();

            if(roundsWonByPlayer1 == 2)
            {
                gameManager.CongratulationsPlayer1.SetActive(true);

                return;
            }

            gameManager.readyToPLay1.SetActive(true);

            gameManager.readyToPLay2.SetActive(false);

            ClimaDespeje climaDespeje = GameObject.FindObjectOfType<ClimaDespeje>();

            climaDespeje.backToTheNormalPowerInFilas();

            BuffFila buffFila = GameObject.FindObjectOfType<BuffFila>();

            buffFila.backToNormalPowerInCards();

            if(whichPlayerIs == false)
            {
                whichPlayerIs = true;
            }

            GameObject trophyInstance = Instantiate(gameManager.trophy, gameManager.mainCanvas.transform, false);

            trophyInstance.transform.localPosition = new Vector3(-338, -196, 0);
        }

        else if(pointsPlayer1 < pointsPlayer2)
        {
            roundsWonByPlayer2++ ;

            GameManager gameManager = GameObject.FindAnyObjectByType<GameManager>();

            if(roundsWonByPlayer2 == 2)
            {
                gameManager.CongratulationsPlayer2.SetActive(true);

                return;
            }

            gameManager.readyToPLay1.SetActive(false);

            gameManager.readyToPLay2.SetActive(true);

            if(whichPlayerIs == true)
            {
                whichPlayerIs = false;
            }

            GameObject trophyInstance = Instantiate(gameManager.trophy, gameManager.mainCanvas.transform, false);  //Instancia una imagen para simbolizar que player gano la ronda.

            trophyInstance.transform.localPosition = new Vector3(-338, 195, 0);

            ClimaDespeje climaDespeje = GameObject.FindObjectOfType<ClimaDespeje>();

            climaDespeje.backToTheNormalPowerInFilas();

            BuffFila buffFila = GameObject.FindObjectOfType<BuffFila>();

            buffFila.backToNormalPowerInCards();
        }



    }

    public static GameObject FindRowWithLeastCards(GameObject[] rows)
    {
        int minCards = int.MaxValue;

        GameObject  rowWithLeastCards = null;

        foreach (GameObject row in rows)
        {
            if (row.transform.childCount == 0)
            {
                continue; // Si la fila esta vacia, salta a la siguiente  iteracio
            }

            bool hasCardDisplay = row.transform.Cast<Transform>().Any(child => child.GetComponent<CardDisplay>() != null);

            if (!hasCardDisplay)
            {
                continue; // Si la fila no tiene al menos una carta con componente CardDisplay , salta a la siguiente iteracion
            }

            int currentRowCards = row.transform.childCount;

            if(currentRowCards < minCards)
            {
                minCards = currentRowCards;

                rowWithLeastCards = row;
            }
        }

        return rowWithLeastCards;
    }

    public static void MoveCardsToCementery(GameObject rowWithLeastCards)
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

        foreach (Transform child in rowWithLeastCards.transform)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

            if (cardDisplay.card.classCard != "GoldenCard")
            {
                if (cardDisplay.card.playerID == true)
                {
                    child.transform.SetParent(gameManager.Graveyard1.transform, false);

                    continue;
                }

                else if(cardDisplay.card.playerID == false)
                {
                    child.transform.SetParent(gameManager.Graveyard2.transform, false);

                    continue;
                }
            }
        }
    }

}
