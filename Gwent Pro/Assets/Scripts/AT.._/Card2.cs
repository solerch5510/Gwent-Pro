using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card" , menuName = "Card", order = 1)]
public class Card2 : ScriptableObject
{

    public new string name;
    public int power; // Poder que varia con los efectos
    public int basedPower; //Poder base de la carta.
    public int whichEffectIs;
    public Sprite spriteImage;
    public string Zone;
    public bool playerID; // True para Player1 [Paladins] , False para Player2 [Monsters]
    public string description = "";
    public string classCard = "SilverCard";
    public Card2()
    {

    }
}
