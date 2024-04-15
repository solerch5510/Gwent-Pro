using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points_Counter : MonoBehaviour
{
    public int points;
    public Text pointsText;
    public GameObject melee;
    public GameObject range;
    public GameObject siege;


    private List<GameObject> filas = new List<GameObject>(3);


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

        //Actualizar el texto de los puntos
        pointsText.text = points.ToString();
    }
}
