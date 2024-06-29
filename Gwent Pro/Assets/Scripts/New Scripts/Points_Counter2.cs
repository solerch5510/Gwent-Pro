
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points_Counter2 : MonoBehaviour
{
    public int points;
    public int finalPoints;
    public Text pointsText;
    public GameObject melee;
    public GameObject range;
    public GameObject siege;
    public GameObject cementerio;


    private List<GameObject> filas = new List<GameObject>(3);

// cell size x: 60 y:100  Spacing y:-100
    // Start is called before the first frame update
    void Start()
    {
        CountPoints();

        filas.Add(melee);

        filas.Add(siege);

        filas.Add(range);
    }

    // Update is called once per frame
    void Update()
    {
        CountPoints();
    }

    void CountPoints()
    {
        points = 0; //Reiniciar el contador de puntos

        foreach (GameObject fila in filas)
        {
            foreach (Transform card in fila.transform)
            {
                //Obtener el componente CardDisplay de la carta
                CardDisplay cardComponent = card.GetComponent<CardDisplay>();

                if (cardComponent != null)
                {
                    //Acceder al poder de la carta a traves del componente CardDisplay
                    int cardPower = cardComponent.card.power;
                    //Sumaar el poder de la carta al contador de puntos
                    points += cardPower;
                }
            }
        }

        finalPoints = points;

        if((GameManager.endTurn1 && GameManager.endTurn2) == true)
        {
            moveToCementerio();
        }

        GameManager.pointsPlayer2 = finalPoints;
        
        //Actualizar el texto de los puntos
        pointsText.text = points.ToString();
    }

    void moveToCementerio () // este metodo mueve todas las cartas del player2 tras acabar una ronda; ademas implementa el efecto del jefe de la faction2
    {
        int totalCards = 0; // Contar el total de cartas en todas las filas

        foreach (GameObject fila in filas)
        {
            totalCards += fila.transform.childCount;   
        }

        //Generar un indice aleatorio para seleccionar una carta de todas las filas
        int randomIndex = Random.Range(0, totalCards);

        //Contador de cartas visitadas
        int cardCounter = 0;

        foreach(GameObject fila in filas)
        {
            foreach(Transform card in fila.transform)
            {
                CardDisplay cardComponent = card.GetComponent<CardDisplay>();

                if(cardComponent != null)
                {
                    if(cardCounter == randomIndex)
                    {
                        cardCounter ++;

                        continue;
                    }
                    card.SetParent(cementerio.transform, true);

                    cardCounter ++;
                }
            }
        }

    }
}
