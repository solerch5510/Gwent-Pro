
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCard : MonoBehaviour 
{
    public InteractableObject interactableObject;

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
    public GameObject Hand;
    public Players players;
    List<GameObject> Cards = new List<GameObject>();

    private GridLayoutGroup gridLayoutGroup; //Referencia al GridLayout Group

    // Start is called before the first frame update
    void Start()
    {
        
        
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
        for(var i = 0 ; i < 10 ; i++)
        {
            GameObject playerCard = Instantiate(Cards[Random.Range(0,Cards.Count)], new Vector3((-450 + (i*100)),0,0), Quaternion.identity ) ;
                
            playerCard.transform.SetParent(Hand.transform, false);

            //Asignar las cartas al jugador 1
            players.Player1.AddCard(playerCard);

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
