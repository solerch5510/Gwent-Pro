using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card" , menuName = "Card")]
public class Card2 : ScriptableObject
{

    public new string name;
    public string description;
    public string classCard ;
    public int power; // Poder que varia con los efectos
    public int basedPower; //Poder base de la carta.
    public int whichEffectIs;
    public Sprite spriteImage;

    public string Zone;

    public bool playerID; // True para Player1 , False para Player2
}
