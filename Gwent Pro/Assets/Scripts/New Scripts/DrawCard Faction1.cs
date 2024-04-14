
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCard : MonoBehaviour 
{
    public InteractableObject interactableObject;
    private bool drawOrNot = true;
    public GameObject Card1;
    public GameObject Card2;
    public GameObject Card3;
    public GameObject Card4;
    public GameObject Card5;
    public GameObject Card6;
    public GameObject Card7;
    public GameObject Card8;
    public GameObject Card9;
    public GameObject Card10;
    public GameObject Card11;
    public GameObject Card12;
    public GameObject Card13;
    public GameObject Card14;
    public GameObject Card15;
    public GameObject Card16;
    public GameObject Card17;
    public GameObject Card18;
    public GameObject Card19;
    public GameObject Card20;
    public GameObject Card21;
    public GameObject Card22;
    public GameObject Card23;
    public GameObject Card24;
    public GameObject Card25;
    public GameObject Card26;
    public GameObject Card27;
    public GameObject Card28;
    public GameObject Card29;
    public GameObject Card30;
    public GameObject Card31;
    public GameObject Card32;
    public GameObject Card33;
    public GameObject Card34;
    public GameObject Card35;
    public GameObject Card36;
    public GameObject Card37;
    public GameObject Card38;
    public GameObject Card39;
    public GameObject Card40;
    public GameObject Card41;
    public GameObject Card42;
    public GameObject Hand;
    public Players players;
    List<GameObject> Cards = new List<GameObject>();

    private GridLayoutGroup gridLayoutGroup; //Referencia al GridLayout Group

    // Start is called before the first frame update
    void Start()
    {
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

    public void OnClick() // metodo para robar cartas e instanciar la mano del jugador
    {
        List<GameObject> playerCards1 = players.Player1.Cards; // Trabajando con la lista de cartas del player1

        if(drawOrNot == true)
        {
            int cardsToDrawn = Mathf.Min(10, Cards.Count); // Asegura no dibujar mas cartas de las que ya tengo.

            for(var i = 0 ; i < cardsToDrawn ; i++)
            {
                int randomIndex = Random.Range(0, Cards.Count);

                GameObject playerCard = Instantiate(Cards[randomIndex], new Vector3((-450 + (i*100)),0,0), Quaternion.identity ) ;
                
                playerCard.transform.SetParent(Hand.transform, false);

                //Asignar las cartas al jugador 1
                players.Player1.AddCard(playerCard);

                // Eliminar la carta de la lista para evitar repeticion
                Cards.RemoveAt(randomIndex);

                drawOrNot = false;
           }
        }

        else // Continuar con la dinamica de turnos [instancia la lista de player2 en la escena]
        {
            foreach(GameObject card in playerCards1)
            {
                card.SetActive(true);
            }
        }

    }

    public void drawnTwoCards()
    {
        int cardsToDrawn = Mathf.Min(2, Cards.Count);

        for(var i = 0 ; i < cardsToDrawn ; i++)
        {

            if (Cards.Count == 0)
            {
                Debug.LogError("No hay mas cartas en el deck para dibujar");
            }

            int randomIndex = Random.Range(0, Cards.Count);

            GameObject playerCard = Instantiate(Cards[randomIndex], new Vector3((-450 + (i*100)),0,0), Quaternion.identity ) ;
                
            playerCard.transform.SetParent(Hand.transform, false);

            //Asignar las cartas al jugador 1
            players.Player1.AddCard(playerCard);

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
