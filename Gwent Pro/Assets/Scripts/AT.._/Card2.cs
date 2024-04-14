using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card" , menuName = "Card")]
public class Card2 : ScriptableObject
{

    public new string name;
    public string description;
    public string classCard ;
    public int summonCost;
    public int power;
    public Sprite spriteImage;

    public string Zone;

    public bool playerID; // True para Player1 , False para Player2

    


    public void Print()
    {
        Debug.Log(name + ":" + description + "Recuerda solo se puede colocar una carta por turno" + summonCost);
        
    }
}
