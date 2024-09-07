using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Le faltan cosas todavia******
public class GameBoardReferences : MonoBehaviour
{
    // Referencias a áreas del tablero
    public GameObject handArea;
    public GameObject siegeArea;
    public GameObject rangeArea;
    public GameObject meleeArea;
    public GameObject graveyardArea;
    public GameObject deckArea;
    public GameObject climateZoneArea;

    // Estructuras de campo para cada área
    public FieldStruct hand;
    public FieldStruct siege;
    public FieldStruct range;
    public FieldStruct melee;
    public FieldStruct graveyard;
    public FieldStruct deck;
    public FieldStruct climateZone;

    // Referencias a los jugadores
    public GameObject player1;
    public GameObject player2;

    // Referencia al GameManager
    private GameManager gameManager;

    protected virtual void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        
    }

    
}
