using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimaDespeje : MonoBehaviour
{
    public GameObject fila1; // las dos primeras son las propias de la zona ; las cuatro restantes son para poder cancelar el efecto del clima (hacer funcionar el despeje)
    public GameObject fila2;
    public GameObject fila3;
    public GameObject fila4;
    public GameObject fila5;
    public GameObject fila6;
    public GameObject CementerioPlayer1;
    public GameObject CementerioPlayer2;
    private bool enableEffect;

    //Lista para almacenar las cartas a las que ya se les ha aplicado efecto
    private List<CardDisplay> affectedCards = new List<CardDisplay>();
    // Start is called before the first frame update

    private void OnTransformChildrenChanged() 
    {
        foreach (Transform child in transform)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

            if(cardDisplay != null)
            {
                if (cardDisplay.card.name == "Sale el Sol")
                {
                    backToTheNormalPowerInFilas();

                    if(cardDisplay.card.playerID == true)
                    {
                        cardDisplay.gameObject.transform.SetParent(CementerioPlayer1.transform, false);
                    }

                    else if(cardDisplay.card.playerID == false)
                    {
                        cardDisplay.gameObject.transform.SetParent(CementerioPlayer2.transform, false);
                    }

                    return;
                }
                enableEffect = true;

                ApplyClimaEffectToCardsInFilas();
            }
        }
    }

    void ApplyClimaEffectToCardsInFilas() // metodo para aplicar clima sobre las filas
    {
        // Aplica el efecto a la fila del player1
        ApplyClimaEffectToCardsInFila(fila1);

        //Aplica el efecto a la fila del player2
        ApplyClimaEffectToCardsInFila(fila2);
    }

    public void backToTheNormalPowerInFilas()
    {
        // Vuelve el ataque normal de las cartas a su ataque normal en la fila del player1\
        backToTheNormalPowerInFila(fila1);

        // Vuelve el ataque normal de las cartas a su ataque normal en la fila del player2\
        backToTheNormalPowerInFila(fila2);

        backToTheNormalPowerInFila(fila3);

        backToTheNormalPowerInFila(fila4);

        backToTheNormalPowerInFila(fila5);

        backToTheNormalPowerInFila(fila6);

        eraseCardsClima();
    }

    void eraseCardsClima()
    {
        // Busca todos los componentes CardDisplay en la escena
        CardDisplay[] allCards = GameObject.FindObjectsOfType<CardDisplay>();

        ClimaDespeje[] climaDespejes = GameObject.FindObjectsOfType<ClimaDespeje>();

        foreach (ClimaDespeje climaDespeje in climaDespejes)  // Hacer falso el bool que permite que se active el clima.
        {
            climaDespeje.enableEffect = false;
        }

        Players players = GameObject.FindObjectOfType<Players>();

        List<GameObject> player1Cards = players.Player1.Cards; // referencias a las lista de cartas de player
        List<GameObject> player2Cards = players.Player2.Cards; 

        List<CardDisplay> playersCardDisplay = new List<CardDisplay>(); // Lista de carta donde se guardaran todas las cartas de ambos players

        foreach (GameObject card in player1Cards)  
        {
            CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

            if (cardDisplay != null)
            {
                playersCardDisplay.Add(cardDisplay);
            }
        }

        foreach (GameObject card in player2Cards)
        {
            CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

            if (cardDisplay != null)
            {
                playersCardDisplay.Add(cardDisplay);
            }
        }

        // Itera sobre cada CardDisplay encontrado
        foreach (CardDisplay cardDisplay in allCards)
        {
            // Verifica si la carta es de tipo Clima
            if (cardDisplay.card.classCard == "Clima" && !playersCardDisplay.Contains(cardDisplay) || cardDisplay.card.name == "Sale el Sol" && !playersCardDisplay.Contains(cardDisplay) ) // Restriccion para pasar las cartas clima al cementerio cuando se use el despeje o acabe la ronda, pero que la de la mano de los players no se vean afectadas.
            {
                //Cambia el padre de la carta segun su playerID
                if (cardDisplay.card.playerID)
                {
                    cardDisplay.gameObject.transform.SetParent(CementerioPlayer1.transform, false);
                }

                else 
                {
                    cardDisplay.gameObject.transform.SetParent(CementerioPlayer2.transform, false);
                }
            }
            
        }

        
    }

    void backToTheNormalPowerInFila(GameObject fila)
    {
        // Itera sobre todos los hijos del objeto fila
        foreach (Transform child in fila.transform)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

            cardDisplay.card.power = cardDisplay.card.basedPower;

            cardDisplay.powerText.text = cardDisplay.card.power.ToString();

            affectedCards.Clear();

            BuffFila.affectedCardsByBuff.Clear();

            SameCardEffect.affectedCardsBySC.Clear();

            SameCardEffect.affectedCardsBySC2.Clear();
        }

    }

    void ApplyClimaEffectToCardsInFila(GameObject fila) // metodo para aplicar clima sobre la fila
    {
        //Itera sobre todos los hijos del objeto fila
        foreach (Transform child in fila.transform)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

            if(cardDisplay != null && !affectedCards.Contains(cardDisplay) && !BuffFila.affectedCardsByBuff.Contains(cardDisplay) && cardDisplay.card.classCard == "SilverCard" && enableEffect == true)
            {
                cardDisplay.card.power = 1;

                cardDisplay.powerText.text = cardDisplay.card.power.ToString();

                affectedCards.Add(cardDisplay);

                SameCardEffect.affectedCardsBySC.Clear();

                SameCardEffect.samePowerCards.Clear();
                
                SameCardEffect.affectedCardsBySC2.Clear();

                SameCardEffect.samePowerCards2.Clear();
            }

            else if(cardDisplay != null && !affectedCards.Contains(cardDisplay) && BuffFila.affectedCardsByBuff.Contains(cardDisplay) && cardDisplay.card.classCard == "SilverCard" && enableEffect == true)
            {
                cardDisplay.card.power = 1;

                cardDisplay.powerText.text = cardDisplay.card.power.ToString();

                affectedCards.Add(cardDisplay);

                BuffFila.affectedCardsByBuff.Remove(cardDisplay);

                SameCardEffect.affectedCardsBySC.Clear();

                SameCardEffect.samePowerCards.Clear();
                
                SameCardEffect.affectedCardsBySC2.Clear();

                SameCardEffect.samePowerCards2.Clear();
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       ApplyClimaEffectToCardsInFilas(); 
    }
}
