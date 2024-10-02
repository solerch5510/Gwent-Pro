using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool whichPlayerIs = true; // para controlar el flujo del juego (es decir que player esta jugando en el momento (true-player1 ; false-player2))
    public static bool endTurn1;
    public static bool endTurn2;   

    private bool IsAllInstantiate = false;
    public static int pointsPlayer1;
    public static int pointsPlayer2; 
    public static int roundsWonByPlayer1;
    public static int roundsWonByPlayer2;
    public  GameObject readyToPLay1; 
    public GameObject readyToPLay2;
    public GameObject deck1;
    public GameObject deck2;
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
    public Players players;

    //Estructuras//

    public FieldStruct PaladinsHand;
    public FieldStruct MonstersHand;
    public FieldStruct PSiege;
    public FieldStruct MSiege;
    public FieldStruct PRange;
    public FieldStruct MRange;
    public FieldStruct PMelee;
    public FieldStruct MMelee;
    public FieldStruct PGraveyard;
    public FieldStruct MGraveyard;
    public FieldStruct PDeck;
    public FieldStruct MDeck;
    public string Player1Faction = "Paladins";
    public string Player2Faction = "Monsters";
    public ContextStruct PaladinsFaction;
    public ContextStruct MonsterFaction;
    public ContextStruct allBoard;

    public static void passTurn (GameObject thisCard ,bool playerID)
    {
        //Obtener la referencia a Players y a la lista de cartas de cada uno.
        Players players = GameObject.FindObjectOfType<Players>();

        List<GameObject> player1Cards = players.Player1.Cards;

        List<GameObject> player2Cards = players.Player2.Cards;

        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

        List<GameObject> deck1 , deck2;

        deck1 = gameManager.drawCard.Cards;
        
        deck2 = gameManager.drawCard2.Cards;

        //gameManager.VerifyCardsRemoved(gameManager);

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

    // Metodo para restablecer el estado del tablero
    public void ResetBoard()
    {
        // Reiniciar el tablero completo
        allBoard = new ContextStruct();
    
        // Reiniciar las facciones
        PaladinsFaction = new ContextStruct();
        MonsterFaction = new ContextStruct();
    
        // Reiniciar las manos de los jugadores
        PaladinsHand = new FieldStruct(players.Player1.Cards);
        MonstersHand = new FieldStruct(players.Player2.Cards);
    
        // Reiniciar las filas del tablero
        PSiege = new FieldStruct(player1S);
        MSiege = new FieldStruct(player2S);
        PRange = new FieldStruct(player1R);
        MRange = new FieldStruct(player2R);
        PMelee = new FieldStruct(player1M);
        MMelee = new FieldStruct(player2M);
    
        // Reiniciar los mazos y cementerios
        PDeck = new FieldStruct(drawCard.Cards);
        MDeck = new FieldStruct(drawCard2.Cards);
        PGraveyard = new FieldStruct(Graveyard1);
        MGraveyard = new FieldStruct(Graveyard2);
    
        // Agregar todas las estructuras a sus facciones correspondientes
        PaladinsFaction.Add(PSiege); 
        MonsterFaction.Add(MSiege);
        PaladinsFaction.Add(PRange); 
        MonsterFaction.Add(MRange);
        PaladinsFaction.Add(PMelee); 
        MonsterFaction.Add(MMelee);
        PaladinsFaction.Add(PGraveyard); 
        MonsterFaction.Add(MGraveyard);
    
        // Agregar todas las estructuras al tablero completo
        allBoard.Add(PaladinsHand); 
        allBoard.Add(MonstersHand);
        allBoard.Add(PSiege); 
        allBoard.Add(MSiege);
        allBoard.Add(PRange); 
        allBoard.Add(MRange);
        allBoard.Add(PMelee); 
        allBoard.Add(MMelee);
        allBoard.Add(PDeck); 
        allBoard.Add(MDeck);
        allBoard.Add(PGraveyard); 
        allBoard.Add(MGraveyard);
    }

    public void Start()
    {
        StartCoroutine(StartDalayedCoroutine());
    }

    IEnumerator StartDalayedCoroutine()
    {
        yield return new WaitForEndOfFrame();

        InstantiateAllCards();
        
        ResetBoard();
    }

    // Metodo para llenar el tablero completo
    public ContextStruct FillBoard()
    {
        // Crear una nueva estructura de contexto vacía
        ContextStruct context = new ContextStruct();
    
        // Agregar todas las estructuras de campo al contexto
        context.Add(PaladinsHand); 
        context.Add(MonstersHand); 
        context.Add(PSiege); 
        context.Add(MSiege); 
        context.Add(PRange); 
        context.Add(MRange); 
        context.Add(PMelee); 
        context.Add(MMelee); 
        context.Add(PDeck); 
        context.Add(MDeck); 
        context.Add(PGraveyard); 
        context.Add(MGraveyard);
    
        // Retornar el contexto lleno
        return context;
    }

    // Método para establecer un campo específico del tablero
    public FieldStruct SetField(FieldStruct field, FieldStruct target, bool single = false)
    {
        // Estructura de contexto donde se añadira el resultado
        ContextStruct toAdd;

        // Determinar a que faccion pertenece el campo objetivo
        if(target == PDeck || target == PaladinsHand || target == PSiege || target == PRange || target == PMelee || target ==PGraveyard)
        {
            toAdd = PaladinsFaction;
        }

        else
        {
            toAdd = MonsterFaction;
        }

        // Filtrar las cartas del campo objetivo
        target = FilterCards(field, target);

        // Si se especifico single, reducir el campo a una sola carta
        if (single) 
        {
            target = SingleFilter(target);
        }

        // Agregar el campo filtrado a la faccion correspondiente y al tablero completo
        toAdd.Add(target);

        allBoard.Add(target);

        // Retornar el campo actualizado
        return target;
    }

    // Metodo para aplicar un filtro predeterminado a todo el tablero
    public void AfterPredicateFilter(FieldStruct field, bool single = false)
    {
        // Reiniciar el tablero completo y las facciones
        allBoard = new ContextStruct();

        PaladinsFaction = new ContextStruct();

        MonsterFaction = new ContextStruct();

        // Aplicar el metodo SetField a cada campo del tablero
        PDeck = SetField(field, PDeck, single);

        MDeck = SetField(field, MDeck, single);

        PaladinsHand = SetField(field, PaladinsHand, single);

        MonstersHand = SetField(field, MonstersHand, single);

        PSiege = SetField(field, PSiege, single);

        MSiege = SetField(field, MSiege, single);

        PRange = SetField(field, PRange, single);

        MRange = SetField(field, MRange, single);

        PMelee = SetField(field, PMelee, single);

        MMelee = SetField(field, MMelee, single);

        PGraveyard = SetField(field, PGraveyard, single);
        
        MGraveyard = SetField(field, MGraveyard, single);
    }

    // Metodo para filtrar cartas de un campo objetivo
    public FieldStruct FilterCards(FieldStruct toGet, FieldStruct target)
    {
        // Crear una nueva estructura de campo vacia para almacenar las cartas filtradas
        FieldStruct toReturn = new FieldStruct();

        // Iterar sobre todas las cartas del campo objetivo
        foreach (CardStruct card in target.cardList)
        {
            // Verificar si la carta está contenida en el campo toGet
            if (toGet.Contains(card)) 
            {
                // Si esta contenida, añadirla al campo de retorno
                toReturn.Add(card);
            }
        }

        // Retornar el campo filtrado
        return toReturn;
    }

    // Metodo para reducir un campo a una sola carta
    public FieldStruct SingleFilter(FieldStruct target)
    {
        // Verificar si el campo tiene cartas
        if (target.cardList.Count > 0)
        {
            // Si tiene mas de una carta, eliminar todas excepto la primera
            while (target.cardList.Count != 1)
            {
                target.cardList.RemoveAt(1);
            }
        }
            
        // Retornar el campo con solo una carta
        return target;
    }

    public void InstantiateAllCards()
    {
        if( IsAllInstantiate == true)
        {
            return;
        }

        else
        {
            List<GameObject> deckCards1 , deckCards2;

            deckCards1 = drawCard.Cards;

            deckCards2 = drawCard2.Cards;

            GameObject parent1 = deck1;

            GameObject parent2 = deck2;

            foreach(GameObject card in deckCards1)
            {
                GameObject instantiateCard = Instantiate(card, new Vector3(0,0,0), Quaternion.identity);

                instantiateCard.SetActive(false);

                instantiateCard.transform.SetParent(parent1.transform, false);
            }

            foreach (GameObject card in deckCards2)
            {
                GameObject instantiateCard = Instantiate(card,new Vector3(0,0,0), Quaternion.identity);

                instantiateCard.SetActive(false);

                instantiateCard.transform.SetParent(parent2.transform, false);                
            }
        }

    }  

    void VerifyCardsRemoved(GameManager gameManager)
    {
        GameObject deckInScene1 = deck1;

        GameObject deckInScene2 = deck2;

        List<GameObject> temporal1 = new List<GameObject>();

        List<GameObject> temporal2 = new List<GameObject>();

        foreach(Transform child in deckInScene1.transform )
        {
            GameObject goChild = child.gameObject;
            
            if(gameManager.drawCard.Cards.Contains(goChild))
            {
                temporal1.Add(goChild);
            }
        }

        foreach (Transform child in deckInScene2.transform )
        {
            GameObject goChild = child.gameObject;
           
            if (gameManager.drawCard2.Cards.Contains(goChild))
            {
                temporal2.Add(goChild);
            } 
        }

        gameManager.drawCard.Cards = temporal1;

        gameManager.drawCard2.Cards = temporal2;

    }

}
