using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameCardEffect : MonoBehaviour
{
    public GameObject fila;
    public static List<CardDisplay> affectedCardsBySC = new List<CardDisplay>(); // Lista para almacenar las cartas a las que ya se les ha aplicado el efecato del SameCards
    public static List<CardDisplay> samePowerCards = new List<CardDisplay>(); // Lista para almacenar las cartas que tienen el mismo efecto
    public static List<CardDisplay> affectedCardsBySC2 = new List<CardDisplay>(); // Lista para almacenar las cartas a las que ya se les ha aplicado el efecato del SameCards
    public static List<CardDisplay> samePowerCards2 = new List<CardDisplay>(); // Lista para almacenar las cartas que tienen el mismo efecto
    public bool WhoPlayerIs;

    void ApplyEffectsToSameCard () // Efecto de multiplicar por n el poder de una carta , siendo n la cantidad de cartas iguales con este efecto
    {
        if(WhoPlayerIs == true)
        {
            int countSamePower = 0;

            // Itera sobre todas las cartas de la fila
            foreach (Transform child in fila.transform)
            {
                CardDisplay cardDisplay = child. GetComponent<CardDisplay>();

                if(cardDisplay != null && cardDisplay.card.whichEffectIs == 6 && !samePowerCards.Contains(cardDisplay) && !affectedCardsBySC.Contains(cardDisplay))
                {
                    samePowerCards.Add(cardDisplay);                
                }
            }

            countSamePower = samePowerCards.Count;

            if(affectedCardsBySC.Count != samePowerCards.Count)
            {
                affectedCardsBySC.Clear();
            }

            foreach (Transform child in fila.transform)
            {
                CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

                if(cardDisplay != null && cardDisplay.card.whichEffectIs == 6 && !affectedCardsBySC.Contains(cardDisplay))
                {
                    if(cardDisplay.card.power != 1)
                    {
                        cardDisplay.card.power = cardDisplay.card.basedPower;
                    }  
                
                    cardDisplay.card.power *= countSamePower;

                    cardDisplay.powerText.text = cardDisplay.card.power.ToString();

                    Debug.Log(cardDisplay.card.name);

                    Debug.Log(cardDisplay.card.power);

                    affectedCardsBySC.Add(cardDisplay);
                }
            }

        }

        else
        {
            int countSamePower = 0;

            // Itera sobre todas las cartas de la fila
            foreach (Transform child in fila.transform)
            {
                CardDisplay cardDisplay = child. GetComponent<CardDisplay>(); //Condiciones para escoger las cartas con ese efeto (en este caso, el efecto # 6 y el efecto #7 (6 para las player 1 y 7 para las cartas de player2))

                if(cardDisplay != null && cardDisplay.card.whichEffectIs == 7 && !samePowerCards2.Contains(cardDisplay) && !affectedCardsBySC2.Contains(cardDisplay))
                {
                    samePowerCards2.Add(cardDisplay);                
                }
            }

            countSamePower = samePowerCards2.Count;

            if(affectedCardsBySC2.Count != samePowerCards2.Count)
            {
                affectedCardsBySC2.Clear();
            }

            foreach (Transform child in fila.transform)
            {
                CardDisplay cardDisplay = child.GetComponent<CardDisplay>(); // Usa casi el mismo pricipio que el script BuffFila

                if(cardDisplay != null && cardDisplay.card.whichEffectIs == 7 && !affectedCardsBySC2.Contains(cardDisplay))
                {
                    if(cardDisplay.card.power != 1)
                    {
                        cardDisplay.card.power = cardDisplay.card.basedPower;
                    }  
                
                    cardDisplay.card.power *= countSamePower;

                    cardDisplay.powerText.text = cardDisplay.card.power.ToString();

                    Debug.Log(cardDisplay.card.name);

                    Debug.Log(cardDisplay.card.power);

                    affectedCardsBySC2.Add(cardDisplay);
                }
            }

        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ApplyEffectsToSameCard();
    }
}
