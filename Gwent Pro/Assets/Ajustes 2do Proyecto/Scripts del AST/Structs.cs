using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Struct
{
    public GameBoardReferences gameBoardReferences;

    public abstract object Acces(object key);

    public abstract object SetAcces(object key, object value, bool isLast = false);
}

public class CardStruct : Struct
{
    public GameObject card;

    public override object Acces(object key)
    {
        string k = key as string;

        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

        if (k == "Power") 
        {
            return cardDisplay.card.basedPower;
        }

        if (k == "Name") 
        {
            return cardDisplay.card.name;
        }

        if (k == "Type") 
        {
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
        
        return default;
    }

    public override object SetAcces(object key, object value, bool isLast = false)
    {
        string k = key as string;

        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

        if (isLast)
        {
            if (k == "Power")
            {
                cardDisplay.card.basedPower = ((int)value);
            }

            if (k == "Name")
            {
                cardDisplay.card.name = value as string;

                cardDisplay.Start();
            }

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
        
        return default;
    }

    public CardStruct(GameObject card)
    {
        this.card = card;

        gameBoardReferences = GameObject.Find("GameBoardReferences").GetComponent<GameBoardReferences>();
    }
}

public class FieldStruct : Struct
{
    public List<CardStruct> cardList;

    public override object Acces (object key)
    {
        int index = (int)key;

        return cardList[index];
    }

    public override object SetAcces(object key, object value, bool isLast = false)
    {
        int index = (int)key;

        if (isLast)
        {
            cardList[index] = value as CardStruct;
        }

        return cardList[index];
    }

    public bool Contains(CardStruct card)
    {
        return cardList.Contains(card);
    }

    public FieldStruct()
    {
        gameBoardReferences= GameObject.Find("GameBoardReferences").GetComponent<GameBoardReferences>();

        cardList = new List<CardStruct>();
    }

    /*public FieldStruct(GameObject cardZone)
    {
        cardList = new List<CardStruct>();

        gameBoardReferences = GameObject.Find("GameBoardReferences").GetComponent<GameBoardReferences>();

        string name = cardZone.name;

        if (name == "GraveyardPaladins" || name == "GraveyardMonsters")
        {
            if(name == "GraveyardPaladins")
            {
                GetDisplay(.GetComponent<GameManager>().Graveyard1);
            }
            
        }
        else if (name == "PlayerDeck" || name == "PlayerDeckBad")
        {
            GetDisplay(cardZone.GetComponent<Deck>().deck);
        }
        else 
            foreach (Transform card in cardZone.transform)
            {
                cardList.Add(new CardStruct(card.gameObject));
            }
    }*/

    void GetDisplay(List<GameObject> list)
    {
        foreach (GameObject card in list)
        {
            card.GetComponent<CardDisplay>().Start();

            cardList.Add(new CardStruct(card));
        }
    }

    public FieldStruct(List<CardStruct> cardList)
    {
        gameBoardReferences = GameObject.Find("GameBoardReferences").GetComponent<GameBoardReferences>();
        
        this.cardList = cardList;
    }

    public void Add(CardStruct card)
    {
        cardList.Add(card);
    }

    public void Shuffle() // See this crap...
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            CardStruct temp = cardList[i];

            int randomIndex = Random.Range(0, cardList.Count);
            
            cardList[i] = cardList[randomIndex];
            
            cardList[randomIndex] = temp;
        }
    }

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

    public CardStruct Pop()
    {
        CardStruct toReturn = cardList[0];
        cardList.RemoveAt(0);
        return toReturn;
    }

    public void SendBottom(CardStruct card)
    {
        Remove(card);
        Add(card);
    }

    public void Push(CardStruct card)
    {
        cardList.Insert(0, card);
    }

    public void Join(FieldStruct fieldToJoin)
    {
        foreach (CardStruct card in fieldToJoin.cardList)
        {
            cardList.Add(card);
        }
    }
}

/*public class ContextStruct : Struct
{
    public List<FieldStruct> fieldList;
    public FieldStruct allCards;

    public ContextStruct()
    {
        gameBoardReferences = GameObject.Find("GameBoardReferences").GetComponent<GameBoardReferences>();
        
        fieldList = new List<FieldStruct>();
        
        allCards = new FieldStruct();
    }

    public void Add(FieldStruct field)
    {
        fieldList.Add(field);
        foreach (CardStruct card in field.cardList)
        {
            allCards.Add(card);
        }
    }

    /*public override object Acces(object key)
    {
        string k = key as string;

        string faction = (allCards.cardList.Count > 0) ?
            allCards.cardList[0].Acces("Faction") as string : MyTools.GetFaction();

        if (faction == "Shrek")
        {
            if (k == "Hand") return refToBoard.shrekHand;
            if (k == "Graveyard") return refToBoard.shrekGraveyard;
            if (k == "Deck") return refToBoard.shrekDeck;
            if (k == "Melee") return refToBoard.shrekMelee;
            if (k == "Range") return refToBoard.shrekRange;
            if (k == "Siege") return refToBoard.shrekSiege;
            if (k == "TriggerPlayer") return refToBoard.shrekFaction;
        }
        else
        {
            if (k == "Hand") return refToBoard.badHand;
            if (k == "Graveyard") return refToBoard.badGraveyard;
            if (k == "Deck") return refToBoard.badDeck;
            if (k == "Melee") return refToBoard.badMelee;
            if (k == "Range") return refToBoard.badRange;
            if (k == "Siege") return refToBoard.badSiege;
            if (k == "TriggerPlayer") return refToBoard.badFaction;
        }

        return default;
    }

    public override object SetAcces(object key, object value, bool isLast = false)
    {
        string k = key as string;
        string faction = (allCards.cardList.Count > 0) ?
            allCards.cardList[0].Acces("Faction") as string : MyTools.GetFaction();

        if (isLast)
        {
            if (faction == "Shrek")
            {
                if (k == "Hand") refToBoard.shrekHand = value as FieldStruct;
                if (k == "Graveyard") refToBoard.shrekGraveyard = value as FieldStruct;
                if (k == "Deck") refToBoard.shrekDeck = value as FieldStruct;
                if (k == "Melee") refToBoard.shrekMelee = value as FieldStruct;
                if (k == "Range") refToBoard.shrekRange = value as FieldStruct;
                if (k == "Siege") refToBoard.shrekSiege = value as FieldStruct;
            }
            else
            {
                if (k == "Hand") refToBoard.badHand = value as FieldStruct;
                if (k == "Graveyard") refToBoard.badGraveyard = value as FieldStruct;
                if (k == "Deck") refToBoard.badDeck = value as FieldStruct;
                if (k == "Melee") refToBoard.badMelee = value as FieldStruct;
                if (k == "Range") refToBoard.badRange = value as FieldStruct;
                if (k == "Siege") refToBoard.badSiege = value as FieldStruct;
            }
        }

        return Acces(k);
    }

    public FieldStruct DeckOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekDeck;
        }
        else
        {
            return refToBoard.badDeck;
        }
    }

    public FieldStruct HandOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekHand;
        }
        else
        {
            return refToBoard.badHand;
        }
    }

    public FieldStruct GraveyardOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekGraveyard;
        }
        else
        {
            return refToBoard.badGraveyard;
        }
    }

    public FieldStruct FieldOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekFaction.allCards;
        }
        else
        {
            return refToBoard.badFaction.allCards;
        }
    }
}*/