using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Contexto del tablero completo
//    public ContextStruct boardContext;

    // Referencias a los jugadores
    public GameObject player1;
    public GameObject player2;

    // Referencia al GameManager
    private GameManager gameManager;

    protected virtual void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

       // ResetBoard();
    }
    /*

    public void ResetBoard()
    {
        boardContext = new ContextStruct();

        hand = new FieldStruct(handArea);
        siege = new FieldStruct(siegeArea);
        range = new FieldStruct(rangeArea);
        melee = new FieldStruct(meleeArea);
        graveyard = new FieldStruct(graveyardArea);
        deck = new FieldStruct(deckArea);
        climateZone = new FieldStruct(climateZoneArea);

        AddAreasToContext();
    }*/
/*
    private void AddAreasToContext()
    {
        boardContext.Add(hand);
        boardContext.Add(siege);
        boardContext.Add(range);
        boardContext.Add(melee);
        boardContext.Add(graveyard);
        boardContext.Add(deck);
        boardContext.Add(climateZone);
    }*/
}
