
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCard2 : MonoBehaviour 
{
    public InteractableObject interactableObject;
    private bool drawOrNot = true;
    public GameObject Card1, Card2, Card3, Card4, Card5, Card6, Card7, Card8, Card9, Card10;
    public GameObject Card11, Card12, Card13, Card14, Card15, Card16, Card17, Card18, Card19, Card20;
    public GameObject Card21, Card22, Card23, Card24, Card25, Card26, Card27, Card28, Card29, Card30;
    public GameObject Card31, Card32, Card33, Card34, Card35, Card36, Card37, Card38, Card39, Card40;
    public GameObject Card41, Card42, Card43, Card44, Card45, Card46, Card47, Card48, Card49, Card50;
    public GameObject Card51, Card52, Card53, Card54;
    public GameObject Hand;
    public Players players;
    public List<GameObject> Cards = new List<GameObject>();

    private GridLayoutGroup gridLayoutGroup; //Referencia al GridLayout Group

    // Start is called before the first frame update
    void Start()
    {
        Cards.Add(Card54); 
        Cards.Add(Card53);
        Cards.Add(Card52);
        Cards.Add(Card51);
        Cards.Add(Card50);
        Cards.Add(Card49);
        Cards.Add(Card48);
        Cards.Add(Card47);
        Cards.Add(Card46);
        Cards.Add(Card45);
        Cards.Add(Card44);
        Cards.Add(Card43);
        Cards.Add(Card42);
        Cards.Add(Card41); 
        Cards.Add(Card40);
        Cards.Add(Card39);
        Cards.Add(Card38);
        Cards.Add(Card37);
        Cards.Add(Card36);
        Cards.Add(Card35);
        Cards.Add(Card34);
        Cards.Add(Card33);
        Cards.Add(Card32);
        Cards.Add(Card31);
        Cards.Add(Card30);
        Cards.Add(Card29);
        Cards.Add(Card28);
        Cards.Add(Card27);
        Cards.Add(Card26);
        Cards.Add(Card25);
        Cards.Add(Card24);
        Cards.Add(Card23);
        Cards.Add(Card22);
        Cards.Add(Card21); 
        Cards.Add(Card20);
        Cards.Add(Card19);
        Cards.Add(Card18);
        Cards.Add(Card17);
        Cards.Add(Card16);
        Cards.Add(Card15);
        Cards.Add(Card14);
        Cards.Add(Card13);
        Cards.Add(Card12);
        Cards.Add(Card11); 
        Cards.Add(Card10);
        Cards.Add(Card9);
        Cards.Add(Card8);
        Cards.Add(Card7);
        Cards.Add(Card6);
        Cards.Add(Card5);
        Cards.Add(Card4);
        Cards.Add(Card3);
        Cards.Add(Card2);
        Cards.Add(Card1);  
        

       gridLayoutGroup = Hand.GetComponentInChildren<GridLayoutGroup>(); //El objeto padre tiene el GridLayoutGroup adjunto
    }

    public void OnClick() // metodo para robar cartas
    {
        List<GameObject> playerCards2 = players.Player2.Cards; // Trabajando con la lista de cartas del player2

        if(drawOrNot == true)
        {
            int cardsToDrawn = Mathf.Min(10, Cards.Count); // Asegura no dibujar mas cartas de las que ya tengo.
            for(var i = 0 ; i < cardsToDrawn ; i++)
            {
                int randomIndex = Random.Range(0, Cards.Count);

                GameObject playerCard = Instantiate(Cards[randomIndex], new Vector3((-450 + (i*100)),0,0), Quaternion.identity ) ;
                
                playerCard.transform.SetParent(Hand.transform, false);

                //Asignar las cartas al jugador 1
                players.Player2.AddCard(playerCard);

                // Eliminar la carta de la lista para evitar repeticion
                Cards.RemoveAt(randomIndex);

                drawOrNot = false;

            }
        }

        else // Continuar con la dinamica de turnos [instancia la lista de player2 en la escena]
        {
            foreach(GameObject card in playerCards2)
            {
                card.SetActive(true);
            }
        }
    }

    public void drawnTwoCards()
    {
        int cardsToDrawn;

        if(players.Player2.Cards.Count == 10) // Condiciones para no tener mas de 10 cartas en mano.
        {
            cardsToDrawn = 0;
        }

        else if(players.Player2.Cards.Count == 9)
        {
            cardsToDrawn = 1;
        }

        else
        {
            cardsToDrawn = 2;
        }
        for(var i = 0 ; i < cardsToDrawn ; i++)
        {

            if (Cards.Count == 0)
            {
                Debug.LogError("No hay mas cartas en el deck para dar");
            }
            
            int randomIndex = Random.Range(0, Cards.Count);

            GameObject playerCard = Instantiate(Cards[randomIndex], new Vector3((-450 + (i*100)),0,0), Quaternion.identity ) ;
                
            playerCard.transform.SetParent(Hand.transform, false);

            playerCard.GetComponent<CardDisplay>().ActivateOnClick = false;

            //Asignar las cartas al jugador 1
            players.Player2.AddCard(playerCard);

            // Eliminar la carta de la lista para evitar repeticion
            Cards.RemoveAt(randomIndex);
        }

    }

    public void drawnOneCards() // Metodo para robar una carta del mazo (Efecto#3s)
    {
        int cardsToDrawn;

        if(players.Player2.Cards.Count == 10) // Condiciones para no tener mas de 10 cartas en mano.
        {
            cardsToDrawn = 0;
        }

        else
        {
            cardsToDrawn = 1;
        }

        for(var i = 0 ; i < cardsToDrawn ; i++)
        {

            if (Cards.Count == 0)
            {
                Debug.LogError("No hay mas cartas en el deck para dibujar");
            }

            int randomIndex = Random.Range(0, Cards.Count);

            GameObject playerCard = Instantiate(Cards[randomIndex], new Vector3((-450 + (i*100)),0,0), Quaternion.identity ) ;
                
            playerCard.transform.SetParent(Hand.transform, false);

            playerCard.GetComponent<CardDisplay>().ActivateOnClick = false;

            //Asignar las cartas al jugador 1
            players.Player2.AddCard(playerCard);

            // Eliminar la carta de la lista para evitar repeticion
            Cards.RemoveAt(randomIndex);
        }

    }

    // Metodo para desactivar el GridLayoutGroup

    public void DisableGridLayoutGroup()
    {
        if (gridLayoutGroup != null )
        {
            gridLayoutGroup.enabled = false;
        }
    }

    // Metodo para reactivar el GridLayoutGroup

    public void EnableGridLayoutGroup()
    {
        if (gridLayoutGroup != null )
        {
            gridLayoutGroup.enabled = true;
        }
    }

}
