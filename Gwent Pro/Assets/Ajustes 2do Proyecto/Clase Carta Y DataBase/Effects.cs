using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectsDefinition : MonoBehaviour
{
    public int Params;

    public string effectName;


}

public class Effects: EffectsDefinition
{
    // Atributo privado para almacenar una referencia al GameManager
    private GameManager gameManager;

    // Metodo virtual de inicio que se ejecuta cuando el objeto se activa
    protected virtual void Start()
    {
        // Busca el componente GameManager en el objeto actual y lo asigna a gameManager
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Metodo publico que aplica un efecto basado en el parametro whichEffectIs
    public void ApplyEffect(int whichEffectIs)
    {
        // Obtener las referencias a las filas
        List<CardDisplay> cardPowersStrong = new List<CardDisplay>();
        List<CardDisplay> cardPowersWeak = new List<CardDisplay>();

        // Iterar sobre los hijos de cada fila
        IterateOverRows(cardPowersStrong, cardPowersWeak);

        // Encuentra las cartas con menor y mayor poder
        CardDisplay minPowerCardDisplay = null;
        CardDisplay maxPowerCardDisplay = null;

        int minPower = int.MaxValue;
        int maxPower = int.MinValue;

        FindMinAndMaxPowerCards(cardPowersStrong, cardPowersWeak, ref minPowerCardDisplay, ref maxPowerCardDisplay, ref minPower, ref maxPower);

        // Aplica el efecto correspondiente segun whichEffectIs
        switch (whichEffectIs)
        {
            case 0:
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            case 1:
                ApplyEffect1(minPowerCardDisplay);
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            case 2:
                ApplyEffect2(maxPowerCardDisplay);
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            case 3:
                ApplyEffect3();
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            case 4:
                ApplyEffect4();
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            case 5:
                ApplyEffect5();
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            case 9:
                ApplyEffect9();
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            case 10:
                ApplyEffect10(maxPowerCardDisplay);
                ClearLists(cardPowersStrong, cardPowersWeak);
                break;
            default:
            // Maneja el caso de un efecto desconocido
                Debug.LogError("Efecto no reconocido");
                break;
        }
    }

    // Metodo privado que itera sobre las filas de cartas de ambos jugadores
    private void IterateOverRows(List<CardDisplay> cardPowersStrong, List<CardDisplay> cardPowersWeak)
    {
        foreach (Transform child in gameManager.player1M.transform)
        {
            AddToLists(child, cardPowersStrong, cardPowersWeak, false);
        }

        foreach (Transform child in gameManager.player1S.transform)
        {
            AddToLists(child, cardPowersStrong, cardPowersWeak, false);
        }

        foreach (Transform child in gameManager.player1R.transform)
        {
            AddToLists(child, cardPowersStrong, cardPowersWeak, false);
        }

        foreach (Transform child in gameManager.player2S.transform)
        {
            AddToLists(child, cardPowersStrong, cardPowersWeak, true);
        }

        foreach (Transform child in gameManager.player2R.transform)
        {
            AddToLists(child, cardPowersStrong, cardPowersWeak, true);
        }

        foreach (Transform child in gameManager.player2M.transform)
        {
            AddToLists(child, cardPowersStrong, cardPowersWeak, true);
        }
    }
 
    // Metodo privado que agrega cartas a las listas correspondientes
    private void AddToLists(Transform child, List<CardDisplay> cardPowersStrong, List<CardDisplay> cardPowersWeak, bool isPlayer2)
    {
        // Obtiene el componente CardDisplay del objeto hijo
        CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

        if (cardDisplay != null && cardDisplay.card.classCard != "GoldenCard")
        {
            // Agrega la carta a la lista de cartas fuertes
            cardPowersStrong.Add(cardDisplay);

            // Si no es el jugador 2, también agrega la carta a la lista de cartas débiles
            if (!isPlayer2)
            {
                cardPowersWeak.Add(cardDisplay);
            }
        }
    }


    // Metodo privado que encuentra las cartas con menor y mayor poder
    private void FindMinAndMaxPowerCards(List<CardDisplay> cardPowersStrong, List<CardDisplay> cardPowersWeak, ref CardDisplay minPowerCardDisplay, ref CardDisplay maxPowerCardDisplay, ref int minPower, ref int maxPower)
    {
        // Busca la carta con mayor poder entre las cartas fuertes
        foreach (CardDisplay cardDisplay1 in cardPowersStrong)
        {
            if (cardDisplay1.card.power > maxPower && cardDisplay1 != null)
            {
                maxPower = cardDisplay1.card.power;

                maxPowerCardDisplay = cardDisplay1;
            }
        }

        // Busca la carta con menor poder entre las cartas debiles
        foreach (CardDisplay cardDisplay1 in cardPowersWeak)
        {

            if (cardDisplay1.card.power < minPower && cardDisplay1 != null)
            {
                minPower = cardDisplay1.card.power;

                minPowerCardDisplay = cardDisplay1;
            }
        }
    }

    // Metodo privado que limpia las listas de cartas
    private void ClearLists(List<CardDisplay> cardPowersStrong, List<CardDisplay> cardPowersWeak)
    {
        cardPowersStrong.Clear();

        cardPowersWeak.Clear();
    }

    // Metodos protegidos para aplicar los efectos especificos
    protected virtual void ApplyEffect1(CardDisplay minPowerCardDisplay)
    {
        // Mueve la carta con menor poder al cementerio del jugador 1 si es el jugador 1, o al del jugador 2 si es el jugador 2
        if (minPowerCardDisplay == null) 
        {
            return;
        }

        if (minPowerCardDisplay.card.playerID)
        {
            minPowerCardDisplay.gameObject.transform.SetParent(gameManager.Graveyard1.transform, false);
        }

        else
        {
            minPowerCardDisplay.gameObject.transform.SetParent(gameManager.Graveyard2.transform, false);
        }
    }

    protected virtual void ApplyEffect2(CardDisplay maxPowerCardDisplay)
    {
        // Mueve la carta con mayor poder al cementerio del jugador 1 si es el jugador 1, o al del jugador 2 si es el jugador 2
        if (maxPowerCardDisplay == null) return;

        if (maxPowerCardDisplay.card.playerID)
        {
            maxPowerCardDisplay.gameObject.transform.SetParent(gameManager.Graveyard1.transform, false);
        }

        else
        {
            maxPowerCardDisplay.gameObject.transform.SetParent(gameManager.Graveyard2.transform, false);
        }
    }

    protected virtual void ApplyEffect3()
    {
        // Roba una carta si es el turno del jugador 1, o otra si es el turno del jugador 2
        if (GameManager.whichPlayerIs)
        {
            gameManager.drawCard.drawnOneCards();
        }

        else
        {
            gameManager.drawCard2.drawnOneCards();
        }
    }

    protected virtual void ApplyEffect4()
    {
        GameObject[] rows = new GameObject[]
        {
            // Encuentra la fila con menos cartas y mueve todas las cartas a los cementerios
            gameManager.player1M, gameManager.player1R, gameManager.player1S,

            gameManager.player2S, gameManager.player2M, gameManager.player2R
        };

        GameObject rowWithLeastCards = GameManager.FindRowWithLeastCards(rows);

        for (int i = 0; i < 4; i++)
        {
            if (rowWithLeastCards != null)
            {
                GameManager.MoveCardsToCementery(rowWithLeastCards);
            }
        }
    }

    protected virtual void ApplyEffect5()
    {
        // Calcula el promedio de poder de todas las cartas y lo aplica a todas las cartas
        GameObject[] rows = new GameObject[]
        {
            gameManager.player1M, gameManager.player1R, gameManager.player1S,

            gameManager.player2S, gameManager.player2M, gameManager.player2R
        };

        int totalPoints = GameManager.pointsPlayer1 + GameManager.pointsPlayer2;

        int totalCards = 0;

        foreach (GameObject row in rows)
        {
            totalCards += row.transform.childCount;
        }

        int averagePower = (totalPoints + 3) / totalCards;

        if (averagePower == 0)
        {
            averagePower = 3;
        }

        foreach (GameObject row in rows)
        {
            foreach (Transform child in row.transform)
            {
                CardDisplay cardDisplay = child.GetComponent<CardDisplay>();

                if (cardDisplay != null && cardDisplay.card.classCard != "GoldenCard")
                {
                    cardDisplay.card.power = averagePower;

                    cardDisplay.powerText.text = cardDisplay.card.power.ToString();
                }
            }
        }
    }

    protected virtual void ApplyEffect9()
    {
        Interpreter interpreter = GetComponent<CardDisplay>().card.interpreter;

        interpreter.InterpretEffectToPlay();
    }   

    public virtual void ApplyEffect10(CardDisplay maxPowerCardDisplay)
    {
        if (maxPowerCardDisplay == null) return;

        if (maxPowerCardDisplay.card.playerID)
        {
            maxPowerCardDisplay.card.power -=5;
        }

        else
        {
            maxPowerCardDisplay.card.power -=5;
        }
    }
}