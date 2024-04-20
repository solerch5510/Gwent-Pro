using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFila : MonoBehaviour
{
    public GameObject fila;
    public GameObject CementerioPlayer1;
    public GameObject CementerioPlayer2;
    private bool enableEffect;
    public static List<CardDisplay> affectedCardsByBuff = new List<CardDisplay>(); // Lista para almacenar las cartas a las que ya se les ha aplicado el efecato del buff
    
    // Metodo  que se llama cuando se agrega un nuevo objeto como hijo
    void  OnTransformChildrenChanged()
    {
        // Verifica si el objeto que se agrego es una carta
        foreach (Transform child in transform)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

            if (cardDisplay != null)
            {
                enableEffect = true;

                ApplyBuffEffectToCardsInFila();
            }
        }
    }

    //Metodo para activar el efecto de multiplicar por 2 el poder de todas las cartas de la fila
    void ApplyBuffEffectToCardsInFila()
    {
        //Itera sobre todos los hijos del objeto fila
        foreach (Transform child in fila.transform)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

            if(cardDisplay != null && !affectedCardsByBuff.Contains(cardDisplay) && cardDisplay.card.classCard == "SilverCard" && enableEffect == true)
            {
                // Aplica el efecto de multiplicar por 2 el poder de la carta
                cardDisplay.card.power *= 2;

                cardDisplay.powerText.text = cardDisplay.card.power.ToString();

                // Agregue la carta a la lista de cartas afectadas
                affectedCardsByBuff.Add(cardDisplay);
            }
        }
    }

    public void backToNormalPowerInCards()
    {
        BuffFila[] disableBuffFilas = GameObject.FindObjectsOfType<BuffFila>(); // Desactivar el bool que permite activar el efecto de buff

        foreach (BuffFila buffFila in disableBuffFilas)
        {
            buffFila.enableEffect = false;
        }

        CardDisplay[] allCards = GameObject.FindObjectsOfType<CardDisplay>();

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
            if (cardDisplay.card.classCard == "Buff" && !playersCardDisplay.Contains(cardDisplay)) // Restriccion para pasar las cartas Buff al cementerio cuando se acabe la ronda, pero que las de la mano de los players no se vean afectadas.
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

        foreach (Transform child in fila.transform)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

            if(cardDisplay != null && affectedCardsByBuff.Contains(cardDisplay) && cardDisplay.card.classCard == "SilverCard")
            {
                // Devuelve el poder base de la carta afectada
                cardDisplay.card.power = cardDisplay.card.basedPower;

                cardDisplay.powerText.text = cardDisplay.card.power.ToString();

                // Agregue la carta a la lista de cartas afectadas
                affectedCardsByBuff.Clear();
            }
        }
    }

    void Update()
    {
        ApplyBuffEffectToCardsInFila();
    }

}
