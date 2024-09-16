using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Clase abstracta base para estructuras de datos
public abstract class Struct
{
    // Referencia al gestor del juego
    public GameManager gameBoardReferences;

    // Metodo abstracto para acceder a un elemento
    public abstract object Acces(object key);

    // Metodo abstracto para establecer un elemento
    public abstract object SetAcces(object key, object value, bool isLast = false);
}

// Clase concreta para representar una carta
public class CardStruct : Struct
{
    // Referencia al objeto de la carta en el juego
    public GameObject card;

    // Metodo para acceder a informacion de la carta
    public override object Acces(object key)
    {
        // Convertir la clave a string
        string k = key as string;

        // Obtener el componente CardDisplay de la carta
        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

        // Verificar el tipo de informacion solicitada
        if (k == "Power") 
        {
            // Retornar el poder base de la carta
            return cardDisplay.card.basedPower;
        }

        if (k == "Name") 
        {
            // Retornar el nombre de la carta
            return cardDisplay.card.name;
        }

        if (k == "Type") 
        {
            // Determinar el tipo de carta (GoldenCard o SilverCard)
            if(cardDisplay.card.classCard == "GoldenCard")
            {
                return  "GoldenCard";
            }
            
            else 
            {
                return  "SilverCard";
            }
        }
        
        if (k == "Range")
        {
            // Determinar la zona de la carta
            string zone = cardDisplay.card.Zone;
            
            if (zone == "Melee") 
            {
                return "Melee";
            }

            if (zone == "Range") 
            {
                return "Range";
            }

            if (zone == "Siege") 
            {
                return "Siege";
            }

            if (zone == "PowerUp") 
            {
                return "Buff";
            }

            return "C";
        }

        if (k == "Faction")
        {   
            // Determinar la facción de la carta
            // Player uno es la faccion con true
            if( cardDisplay.card.playerID == true)
            {
                return true;
            }

            // Player dos es la Faccion que se identifica con false
            else
            {
                return false;
            }
        } 

        // Si no se encuentra la información solicitada, retornar el valor por defecto
        return default;
    }

    // Metodo para establecer informacion de la carta
    public override object SetAcces(object key, object value, bool isLast = false)
    {
        // Convertir la clave a string
        string k = key as string;

        // Obtener el componente CardDisplay de la carta
        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

        // Verificar si es la ultima operacion
        if (isLast)
        {
            // Establecer el poder base de la carta
            if (k == "Power")
            {
                cardDisplay.card.basedPower = ((int)value);
            }

            // Establecer el nombre de la carta
            if (k == "Name")
            {
                cardDisplay.card.name = value as string;

                cardDisplay.Start();
            }

            // Establecer el tipo de carta
            if (k == "Type")
            {
                if (value as string == "Oro") 
                {
                    cardDisplay.card.Zone = "GoldenCard";
                }

                if (value as string == "Plata") 
                {
                    cardDisplay.card.Zone = "SilverCard";
                }
            }

            // Establecer la zona de la carta
            if (k == "Range")
            {
                if (value as string == "Melee")
                {
                    cardDisplay.card.Zone = "Melee";
                } 

                if (value as string == "Ranged") 
                {
                    cardDisplay.card.Zone = "Range";
                }

                if (value as string == "Siege") 
                {
                    cardDisplay.card.Zone = "Siege";
                }

                if (value as string == "Climate") 
                {
                    cardDisplay.card.Zone = "C";
                }

                if (value as string == "PowerUp") 
                {
                    cardDisplay.card.Zone = "Buff";
                }
                
            }

            // Establecer la faccion de la carta
            if (k == "Faction")
            {
                if (value as string == "Paladins") 
                {
                    cardDisplay.card.playerID = true;
                }

                if (value as string == "Monsters") 
                {
                    cardDisplay.card.playerID = false;
                }
                
            }
        }
        
        // Si no se encuentra la informacion solicitada, retornar el valor por defecto
        return default;
    }

    // Constructor para inicializar una nueva CardStruct
    public CardStruct(GameObject card)
    {
        this.card = card;

        gameBoardReferences = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
}

// Clase para representar un campo del tablero (mano, fila, mazo, cementerio)
public class FieldStruct : Struct
{
    // Lista de cartas en este campo
    public List<CardStruct> cardList;

    // Metodo para acceder a una carta especifica del campo
    public override object Acces (object key)
    {
        // Convertir la clave a indice entero
        int index = (int)key;

        // Retornar la carta en el indice especificado
        return cardList[index];
    }

    // Metodo para establecer una carta especifica del campo
    public override object SetAcces(object key, object value, bool isLast = false)
    {
        // Convertir la clave a indice entero
        int index = (int)key;

        // Si es la ultima operacion
        if (isLast)
        {
            // Establecer la carta en el indice especificado
            cardList[index] = value as CardStruct;
        }

        // Retornar la carta en el indice especificado
        return cardList[index];
    }

    // Metodo para verificar si el campo contiene una carta especifica
    public bool Contains(CardStruct card)
    {
        // Retornar si la lista de cartas contiene la carta especificada
        return cardList.Contains(card);
    }

    // Constructor para inicializar un nuevo FieldStruct
    public FieldStruct()
    {
        // Inicializar la referencia al gestor del juego
        gameBoardReferences= GameObject.Find("GameManager").GetComponent<GameManager>();

        // Inicializar la lista de cartas
        cardList = new List<CardStruct>();
    }

    // Constructor para inicializar un FieldStruct con un GameObject
    public FieldStruct(GameObject cardZone)
    {
        // Inicializar la lista de cartas
        cardList = new List<CardStruct>();

        // Inicializar la referencia al gestor del juego
        gameBoardReferences = GameObject.Find("GameManager").GetComponent<GameManager>();
 
        // Añadir todas las cartas del GameObject al campo
        foreach (Transform card in cardZone.transform)
        {
            cardList.Add(new CardStruct(card.gameObject));
        }
    }

    // Constructor para inicializar un FieldStruct con una lista de GameObjects
    public FieldStruct(List<GameObject> list)
    {
        // Inicializar la lista de cartas
        cardList = new List<CardStruct>();

        // Inicializar la referencia al gestor del juego
        gameBoardReferences = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Llamar al metodo GetDisplay para procesar la lista de GameObjects
        GetDisplay(list);
    }

    // Metodo para procesar una lista de GameObjects y añadirlos al campo
    void GetDisplay(List<GameObject> list)
    {
        foreach (GameObject card in list)
        {
            card.GetComponent<CardDisplay>().Start();

            cardList.Add(new CardStruct(card));
        }
    }

    // Constructor para inicializar un FieldStruct con una lista de CardStructs
    public FieldStruct(List<CardStruct> cardList)
    {
        // Inicializar la referencia al gestor del juego
        gameBoardReferences = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Establecer la lista de cartas
        this.cardList = cardList;
    }

    // Metodo para añadir una carta al campo
    public void Add(CardStruct card)
    {
        // Añadir la carta a la lista de cartas
        cardList.Add(card);
    }

    // Metodo para barajar las cartas del campo
    public void Shuffle() // See this crap...
    {
        // Barajar las cartas de la lista
        for (int i = 0; i < cardList.Count; i++)
        {
            CardStruct temp = cardList[i];

            int randomIndex = Random.Range(0, cardList.Count);
            
            cardList[i] = cardList[randomIndex];
            
            cardList[randomIndex] = temp;
        }
    }

    // Metodo para eliminar una carta del campo
    public void Remove(CardStruct card)
    {
        int index = -1;
        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i] == card)
            {
                index = i;
                break;
            }
        }

        if (index != -1) cardList.RemoveAt(index);
    }

    // Metodo para eliminar y devolver la primera carta del campo
    public CardStruct Pop()
    {
        CardStruct toReturn = cardList[0];
        cardList.RemoveAt(0);
        return toReturn;
    }

    // Metodo para mover una carta al fondo del campo
    public void SendBottom(CardStruct card)
    {
        Remove(card);
        Add(card);
    }

    // Metodo para añadir una carta al principio del campo
    public void Push(CardStruct card)
    {
        cardList.Insert(0, card);
    }

    // Metodo para unir dos campos
    public void Join(FieldStruct fieldToJoin)
    {
        foreach (CardStruct card in fieldToJoin.cardList)
        {
            cardList.Add(card);
        }
    }
}


public class ContextStruct : Struct
{
    // Lista de estructuras de campo en este contexto
    public List<FieldStruct> fieldList;

    // Estructura que contiene todas las cartas del juego
    public FieldStruct allCards;

    // Constructor para inicializar un nuevo ContextStruct
    public ContextStruct()
    {
        // Inicializar la referencia al gestor del juego
        gameBoardReferences = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Inicializar la lista de campos
        fieldList = new List<FieldStruct>();
        
        // Inicializar la estructura de todas las cartas
        allCards = new FieldStruct();
    }

    // Metodo para añadir un campo al contexto
    public void Add(FieldStruct field)
    {
        // Añadir el campo a la lista de campos
        fieldList.Add(field);

        // Añadir todas las cartas del campo a la estructura de todas las cartas
        foreach (CardStruct card in field.cardList)
        {
            allCards.Add(card);
        }
    }

    // Metodo para acceder a informacion del contexto
    public override object Acces(object key)
    {
        // Convertir la clave a string
        string k = key as string;

        // Determinar la faccion actual
        string faction ;

        if(allCards.cardList.Count > 0)
        {
            faction = allCards.cardList[0].Acces("Faction") as string;
        } 

        else
        {
            faction = GetFaction(GameManager.whichPlayerIs);
        }

        // Acceder a informacion especifica basada en la faccion y la clave
        if (faction == "Paladins")
        {
            if (k == "Hand") 
            {
                return gameBoardReferences.PaladinsHand;
            }

            if (k == "Graveyard") 
            {
                return gameBoardReferences.PGraveyard;
            }

            if (k == "Deck") 
            {
                return gameBoardReferences.PDeck;
            }

            if (k == "Melee") 
            {
                return gameBoardReferences.PMelee;
            }

            if (k == "Range") 
            {
                return gameBoardReferences.PRange;
            }

            if (k == "Siege") 
            {
                return gameBoardReferences.PSiege;
            }

            if (k == "TriggerPlayer") 
            {
                return gameBoardReferences.PaladinsFaction;
            }
        }

        else
        {
            if (k == "Hand") 
            {
                return gameBoardReferences.MonstersHand;
            }

            if (k == "Graveyard") 
            {
                return gameBoardReferences.MGraveyard;
            }

            if (k == "Deck") 
            {
                return gameBoardReferences.MDeck;
            }

            if (k == "Melee") 
            {
                return gameBoardReferences.MMelee;
            }

            if (k == "Range") 
            {
                return gameBoardReferences.MRange;
            }

            if (k == "Siege") 
            {
                return gameBoardReferences.MSiege;
            }

            if (k == "TriggerPlayer") 
            {
                return gameBoardReferences.MonsterFaction;
            }
        }

        // Si no se encuentra la informacion solicitada, retornar el valor por defecto
        return default;
    }

    // Metodo para establecer informacion en el contexto
    public override object SetAcces(object key, object value, bool isLast = false)
    {
        // Convertir la clave a string
        string k = key as string;

        // Determinar la faccion actual
        string faction ;

        if(allCards.cardList.Count > 0) 
        {
            faction = allCards.cardList[0].Acces("Faction") as string;
        }

        else
        {
            faction = GetFaction(GameManager.whichPlayerIs);
        }

        // Establecer informacion especifica basada en la faccion y la clave
        if (isLast)
        {
            if (faction == "Paladins")
            {
                if (k == "Hand") 
                {
                    gameBoardReferences.PaladinsHand = value as FieldStruct;
                }

                if (k == "Graveyard") 
                {
                    gameBoardReferences.PGraveyard = value as FieldStruct;
                }

                if (k == "Deck") 
                {
                    gameBoardReferences.PDeck = value as FieldStruct;
                }
                if (k == "Melee") 
                {
                    gameBoardReferences.PMelee = value as FieldStruct;
                }

                if (k == "Range") 
                {
                    gameBoardReferences.PRange = value as FieldStruct;
                }

                if (k == "Siege") 
                {
                    gameBoardReferences.PSiege = value as FieldStruct;
                }
            }

            else
            {
                if (k == "Hand") 
                {
                    gameBoardReferences.MonstersHand = value as FieldStruct;
                }

                if (k == "Graveyard") 
                {
                    gameBoardReferences.MGraveyard = value as FieldStruct;
                }

                if (k == "Deck") 
                {
                    gameBoardReferences.MDeck = value as FieldStruct;
                }

                if (k == "Melee") 
                {
                    gameBoardReferences.MMelee = value as FieldStruct;
                }

                if (k == "Range") 
                {
                    gameBoardReferences.MRange = value as FieldStruct;
                }

                if (k == "Siege") 
                {
                    gameBoardReferences.MSiege = value as FieldStruct;
                }
            }
        }

        // Retornar el resultado de Acces
        return Acces(k);
    }

    // Metodo para obtener el mazo de un jugador especifico
    public FieldStruct DeckOfPlayer(ContextStruct player)
    {
        if (player == gameBoardReferences.PaladinsFaction)
        {
            return gameBoardReferences.PDeck;
        }
        else
        {
            return gameBoardReferences.MDeck;
        }
    }

    // Metodo para obtener la mano de un jugador especifico
    public FieldStruct HandOfPlayer(ContextStruct player)
    {
        if (player == gameBoardReferences.PaladinsFaction)
        {
            return gameBoardReferences.PaladinsHand;
        }
        else
        {
            return gameBoardReferences.MonstersHand;
        }
    }

    // Metodo para obtener el cementerio de un jugador especifico
    public FieldStruct GraveyardOfPlayer(ContextStruct player)
    {
        if (player == gameBoardReferences.PaladinsFaction)
        {
            return gameBoardReferences.PGraveyard;
        }
        else
        {
            return gameBoardReferences.MGraveyard;
        }
    }

    // Metodo para obtener todas las cartas de un jugador especifico
    public FieldStruct FieldOfPlayer(ContextStruct player)
    {
        if (player == gameBoardReferences.PaladinsFaction)
        {
            return gameBoardReferences.PaladinsFaction.allCards;
        }
        else
        {
            return gameBoardReferences.MonsterFaction.allCards;
        }
    }

    // Metodo estatico para obtener la faccion de un jugador
    public static string GetFaction(bool whichPlayerIs)
    {
        if (whichPlayerIs == true)
        {
            return "Paladins";
        }

        else
        {
            return "Monsters";
        }
    }

    // Metodo estatico para establecer un puntero en el tablero
    public static GameObject SetPointer(GameManager refToBoard, Pointer pointer)
    {
        string k = pointer.pointer;

        string faction = GetFaction(GameManager.whichPlayerIs);

        if (faction == "Paladins")
        {
            if (k == "Hand") 
            {
                return refToBoard.gameObject;
            }

            if (k == "Graveyard") 
            {
                return refToBoard.Graveyard1;
            }

            if (k == "Deck") 
            {
                return refToBoard.gameObject;
            }

            if (k == "Melee") 
            {
                return refToBoard.player1M;
            }

            if (k == "Range") 
            {
                return refToBoard.player1R;
            }

            if (k == "Siege") 
            {
                return refToBoard.player1S;
            }
        }
        else
        {
            if (k == "Hand") 
            {
                return refToBoard.gameObject;
            }

            if (k == "Graveyard") 
            {
                return refToBoard.Graveyard2;
            }

            if (k == "Deck") 
            {
                return refToBoard.gameObject;
            }

            if (k == "Melee") 
            {
                return refToBoard.player2M;
            }

            if (k == "Range") 
            {
                return refToBoard.player2R;
            }

            if (k == "Siege") 
            {
                return refToBoard.player2S;
            }
        }

        // Si no se encuentra la informacion solicitada, retornar el valor por defecto
        return default;
    }
}