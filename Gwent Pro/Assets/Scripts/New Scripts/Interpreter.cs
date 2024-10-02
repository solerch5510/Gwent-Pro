using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;

public class Interpreter
{
    // Fields of Interpreter
    Parser parser;
    GameManager References;
    public MultiScope globalScope;
    public ContextStruct allBoard;

    public Interpreter(Parser parser)
    {
        try
        {
            this.parser = parser;

            parser.Parse();

            globalScope = new MultiScope();

            References = GameObject.Find("GameManager").GetComponent<GameManager>();

            References.Start();
            
            ResetRefAndContext();

            if (parser.CurrentError == "")
            {
                Debug.Log("Parse-Semantic check: OK");
                
                CardVisit(parser.ThisCardNode);
            }

            else 
            {
                Debug.Log("Parse-Semantic check: ERROR");
            }
        }

        catch
        {
            Debug.Log("FATAL ERROR IN INTERPRETER CONSTRUCTOR");
        }
    }

    public object Visit(AST node, MultiScope scope)
    {
        if (node.GetType() == typeof(BinaryOperators)) 
        {
            return BinOpVisit(node as BinaryOperators, scope);
        }

        if (node.GetType() == typeof(UnaryOperators)) 
        {
            return UnaryOpVisit(node as UnaryOperators, scope);
        }

        if (node.GetType() == typeof(Int)) 
        {
            return IntVisit(node as Int);
        }

        if (node.GetType() == typeof(Bool)) 
        {
            return BoolVisit(node as Bool);
        }

        if (node.GetType() == typeof(String)) 
        {
            return StringVisit(node as String);
        }

        if (node.GetType() == typeof(Var)) 
        {
            return VarVisit(node as Var, scope);
        }

        if (node.GetType() == typeof(VarComp)) 
        {
            return VarCompVisit(node as VarComp, scope);
        }

        if (node.GetType() == typeof(Indexer)) 
        {
            return IndexerVisit(node as Indexer, scope);
        }

        if (node.GetType() == typeof(Assign)) 
        {
            AssignVisit(node as Assign, scope);
        }

        if (node.GetType() == typeof(IfNode)) 
        {
            IfNodeVisit(node as IfNode, scope);
        }

        if (node.GetType() == typeof(WhileLoop)) 
        {
            WhileLoopVisit(node as WhileLoop, scope);
        }

        if (node.GetType() == typeof(ForLoop)) 
        {
            Debug.Log("esta entrando al nodo del bucle for");

            ForLoopVisit(node as ForLoop, scope);
        }

        if (node.GetType() == typeof(NoOp)) 
        {
            return NoOpVisit();
        }

        return default;
    }

    public void ResetRefAndContext()
    {
        References.ResetBoard();
        
        allBoard = References.FillBoard();
    }

    public object BinOpVisit(BinaryOperators node, MultiScope scope)
    {
        object left = Visit(node.Left, scope);

        //Debug.Log(left.GetType() + "  este es el tipo del nodo , y esta es su expresion: " + node.Left.type );

        object right = Visit(node.Right, scope);

        if (node.type == ASTType.Type.Int)
        {
            int l = (int)left, r = (int)right;

            if (node.Operator.Type == TokenType.Plus) 
            {
                return l + r;
            }

            if (node.Operator.Type == TokenType.Minus) 
            {
                return l - r;
            }

            if (node.Operator.Type == TokenType.Multiply) 
            {
                return l * r;
            }

            if (node.Operator.Type == TokenType.Divide) 
            {
                return l / r;
            }

            if (node.Operator.Type == TokenType.Mod) 
            {
                return l % r;
            }
        }

        if (node.type == ASTType.Type.String)
        {
            string l = left as string, r = right as string;

            if (node.Operator.Type == TokenType.String_Sum) 
            {
                return l + r;
            }

            if (node.Operator.Type == TokenType.String_Sum_S) 
            {
                return l + " " + r;
            }
        }

        if (node.type == ASTType.Type.Bool)
        {
            if (left.GetType() == typeof(int))
            {
                int l = (int)left, r = (int)right;

                if (node.Operator.Type == TokenType.Equal) 
                {
                    return l == r;
                }

                if (node.Operator.Type == TokenType.Differ) 
                {
                    return l != r;
                }

                if (node.Operator.Type == TokenType.Greater) 
                {
                    return l > r;
                }

                if (node.Operator.Type == TokenType.GreaterEqual) 
                {
                    return l >= r;
                }

                if (node.Operator.Type == TokenType.Less) 
                {
                    return l < r;
                }

                if (node.Operator.Type == TokenType.LessEqual) 
                {
                    return l <= r;
                }
            }

            if (left.GetType() == typeof(bool))
            {
                bool l = (bool)left, r = (bool)right;

                if (node.Operator.Type == TokenType.Equal) 
                {
                    return l == r;
                }

                if (node.Operator.Type == TokenType.Differ) 
                {
                    return l != r;
                }

                if (node.Operator.Type == TokenType.And) 
                {
                    return l && r;
                }

                if (node.Operator.Type == TokenType.Or) 
                {
                    return l || r;
                }
            }

            if (left.GetType() == typeof(string))
            {
                string l = left as string, r = right as string;

                if (node.Operator.Type == TokenType.Equal) 
                {
                    return l == r;
                }

                if (node.Operator.Type == TokenType.Differ) 
                {
                    return l != r;
                }
            }
        }

        return default;
    }

    public object UnaryOpVisit(UnaryOperators node, MultiScope scope)
    {
        object expression = Visit(node.Expression, scope);

        if (node.type == ASTType.Type.Int)
        {
            int e = (int)expression;

            if (node.Operation.Type == TokenType.Plus) 
            {
                return +e;
            }

            else if (node.Operation.Type == TokenType.Minus) 
            {
                return -e;
            }

            else
            {
                Var variable = node.Expression as Var;

                if (node.Operation.Type == TokenType.Plus1) 
                {
                    e++;
                }

                if (node.Operation.Type == TokenType.Decrement) 
                {
                    e--;
                }

                SetValue(variable, scope, e);

                return e;
            }
            
            
        }

        if (node.type == ASTType.Type.Bool)
        {
            bool e = (bool)expression;

            if (node.Operation.Type == TokenType.Not) 
            {
                return !e;
            }
        }

        return default;
    }

    public int IndexerVisit(Indexer node, MultiScope scope)
    {
        int expression = (int)Visit(node.index, scope);

        return expression;
    }

    public int IntVisit(Int node)
    {
        return node.value;
    }

    public bool BoolVisit(Bool node)
    {
        return node.TOF;
    }

    public string StringVisit(String node)
    {
        Debug.Log("la expresion del nodo es : " + node.Expression);
        return node.Expression;
    }

    public void AssignVisit(Assign node, MultiScope scope)
    {
        Var v = node.left;

        string op = node.op.Lexeme;

        Debug.Log("el valor del nodo operador es :  =>|" + op + "|");
        
        string key = node.left.value;

        object left = default;

        if (node.left.GetType() == typeof(VarComp)) 
        {
            left = VarCompVisit(node.left as VarComp, scope);
        }

        else if (scope.IsInScope(key)) 
        {
            left = scope.Get(key);
        }

        Debug.Log("este es el valor del nodo derecho : " + node.right);

        object right = Visit(node.right, scope);

        if (op == "=" || op == " =") 
        {
            Debug.Log("va bien");

            SetValue(v, scope, right);
        }

        if (op == "+=" || op == " +=") 
        {
            SetValue(v, scope, (int)left + (int)right);
        }

        if (op == "-=" || op == " -=") 
        {
            SetValue(v, scope, (int)left - (int)right);
        }

        if (op == "*=" || op == " *=") 
        {
            SetValue(v, scope, (int)left * (int)right);
        }

        if (op == "/=" || op == " /=") 
        {
            SetValue(v, scope, (int)left / (int)right);
        }

        if (op == "%=" || op == " %=") 
        {
            SetValue(v, scope, (int)left % (int)right);
        }

        if (op == "@=" || op == " @=") 
        {
            SetValue(v, scope, (string)left + (string)right);
        }
    }

    public object NoOpVisit()
    {
        return default;
    }

    public object VarVisit(Var node, MultiScope scope)
    {
        return scope.Get(node.value);
    }

    public void SetValue(Var node, MultiScope scope, object value)
    {
        if (node.GetType() == typeof(Var))
        {
            scope.Set(node.value, value);
        }

        else
        {
            int index = 0;

            VarComp v = node as VarComp;
            
            object last = scope.Get(node.value);
            
            AST lastAST = v;

            foreach (var member in v.args)
            {
                bool isLast = (index == v.args.Count - 1);

                if (member.GetType() == typeof(Function))
                {
                    Function function = member as Function;
                   
                    last = FunctionVisit(function, lastAST, last, scope);
                }

                if (member.GetType() == typeof(Indexer))
                {
                    FieldStruct field = last as FieldStruct;
                    
                    Indexer indexer = member as Indexer;
                    
                    int ind = (int)Visit(indexer, scope);
                    
                    last = field.SetAcces(ind, value, isLast);
                }
                else if (member.GetType() == typeof(Pointer))
                {
                    ContextStruct context = last as ContextStruct;
                    
                    Pointer pointer = member as Pointer;
                    
                    last = context.SetAcces(pointer.pointer, value, isLast);
                }
                else
                {
                    CardStruct card = last as CardStruct;
                    
                    Var SetAcces = member as Var;
                    
                    last = card.SetAcces(SetAcces.value, value, isLast);
                }

                index++;
                
                lastAST = member;
            }
        }
    }

    public object VarCompVisit(VarComp node, MultiScope scope)
    {
        object last = scope.Get(node.value);
        
        AST lastAST = node;

        foreach (var member in node.args)
        {
            if (member.GetType() == typeof(Function))
            {
                Function function = member as Function;
                
                last = FunctionVisit(function, lastAST, last, scope);
            }
            
            else if (member.GetType() == typeof(Indexer))
            {
                FieldStruct field = last as FieldStruct;
                
                Indexer indexer = member as Indexer;
                
                int index = (int)Visit(indexer, scope);
                
                last = field.Acces(index);
            }
            
            else if (member.GetType() == typeof(Pointer))
            {
                ContextStruct context = last as ContextStruct;
                
                Pointer pointer = member as Pointer;
                
                last = context.Acces(pointer.pointer);
            }

            else
            {
                CardStruct card = last as CardStruct;
               
                Var acces = member as Var;
               
                last = card.Acces(acces.value);
            }

            lastAST = member;
        }

        return last;
    }

    public object FunctionVisit(Function node, AST realContext, object context, MultiScope scope)
    {
        string name = node.functionName;
        
        object parameter; 

        Debug.Log("este es el contexto : >>>  |" + context + "|");
        
        if(node.args != null && name != "Find")
        {
            parameter = Visit(node.args.args[0], scope);
        }
        
        else
        {
            parameter = default;
        }

        bool isReal = (realContext.GetType() == typeof(Pointer));

        if (name == "Add")
        {
            //FieldStruct field = context as FieldStruct;
            
            CardStruct card = parameter as CardStruct;
            
            //field.Add(card);

            if (isReal)
            {
                GameObject parent = ContextStruct.SetPointer(References, realContext as Pointer);
               
                GameObject playerCard = GameObject.Instantiate(card.card, parent.transform);

                Debug.Log("la carta es : => " + card.card.name);

                if(GameManager.whichPlayerIs == true)
                {
                    Debug.Log("esta llegando hasta la parte de pasar la carta a la lista de P1");

                    //References.drawCard.players.Player1.AddCard(card.card);

                    References.players.Player1.AddCard(playerCard);

                    if(References.drawCard.Cards.Contains(playerCard))
                    {
                        References.drawCard.Cards.Remove(playerCard);
                    }
                }

                else
                {
                    Debug.Log("esta pasando la carta a la lista de P2");

                    //References.drawCard2.players.Player2.AddCard(card.card);

                    References.players.Player2.AddCard(playerCard);

                    if(References.drawCard2.Cards.Contains(playerCard))
                    {
                        References.drawCard2.Cards.Remove(playerCard);
                    }
                }
            }
        }

        if (name == "Shuffle")
        {
            FieldStruct field = context as FieldStruct;
            
            field.Shuffle();
        }

        if (name == "Remove")
        {
            FieldStruct field = context as FieldStruct;
            
            CardStruct card = parameter as CardStruct;
            
            if (isReal)
            {
                GameObject parent = GameObject.Find("Hand");
                
                GameObject.Instantiate(card.card, parent.transform);
                
                GameObject.Destroy(card.card);
            }

            field.Remove(card);
        }

        if (name == "Pop")
        {
            FieldStruct field = context as FieldStruct;
            
            if (isReal)
            {
                //GameObject parent = GameObject.Find("Hand");
                
                //GameObject card = field.cardList[0].card;
                
                //GameObject.Instantiate(card, parent.transform);
                
                //GameObject.DestroyImmediate(card, true);
            }

            return field.Pop();
        }

        if (name == "SendBottom")
        {
            FieldStruct field = context as FieldStruct;
            
            CardStruct card = parameter as CardStruct;
            
            field.SendBottom(card);
        }

        if (name == "Push")
        {
            FieldStruct field = context as FieldStruct;
            
            CardStruct card = parameter as CardStruct;
            
            field.Push(card);
        }

        if (name == "Deck")
        {
            ContextStruct field = context as ContextStruct;
            
            ContextStruct player = parameter as ContextStruct;
            
            return field.DeckOfPlayer(player);
        }

        if (name == "Hand")
        {
            ContextStruct field = context as ContextStruct;
            
            ContextStruct player = parameter as ContextStruct;
            
            return field.HandOfPlayer(player);
        }

        if (name == "Graveyard")
        {
            ContextStruct field = context as ContextStruct;
            
            ContextStruct player = parameter as ContextStruct;
            
            return field.GraveyardOfPlayer(player);
        }

        if (name == "Field")
        {
            ContextStruct field = context as ContextStruct;
            
            ContextStruct player = parameter as ContextStruct;
            
            return field.FieldOfPlayer(player);
        }

        if (name == "Find")
        {
            FieldStruct field = context as FieldStruct;
            
            Var unit = node.args.args[0] as Var;
            
            AST condition = node.args.args[1];
            
            FieldStruct toReturn = new FieldStruct();
            
            foreach (CardStruct card in field.cardList)
            {
                SetValue(unit, scope, card);

                if ((bool)Visit(condition, new MultiScope(scope)))
                {
                    toReturn.Add(card);
                }
            }
            return toReturn;
        }

        return default;
    }

    public void OnActivationVisit(OnActivation node, MultiScope scope)
    {
        foreach (OnActivationElement element in node.onActivation)
        {
            ResetRefAndContext();
            
            MultiScope innerScope = new MultiScope(scope);
            
            OnActivationElementVisit(element, innerScope);
        }
    }

    public void PostActionVisit(PostAction node, MultiScope scope, string source)
    {
        ResetRefAndContext();
        
        if (node.selector.source.source == "parent") 
        {
            node.selector.source.source = source;
        }

        string name = node.effectOnActivation1.name.paramName;

        MultiScope effectParams = EffectOnActivationVisit(node.effectOnActivation1, scope);
        
        allBoard = SelectorVisit(node.selector, scope);
        
        EffectNode effect = parser.EffectList[name];
        
        EffectActionVisit(effect, effectParams);
    }

    public void OnActivationElementVisit(OnActivationElement node, MultiScope scope)
    {
        string name = node.effectOnActivation.name.paramName;

        Debug.Log(node.effectOnActivation.name.paramName + " este es el nombre del efecto");

        MultiScope effectParams = EffectOnActivationVisit(node.effectOnActivation, scope);
        
        allBoard = SelectorVisit(node.selector, scope);
        
        string source = node.selector.source.source;
        
        EffectNode effect = parser.EffectList[name];

        Debug.Log("va a visitar la activacion del efecto definido");
        
        EffectActionVisit(effect, effectParams);

        if (node.postAction != null)
        {
            PostActionVisit(node.postAction, scope, source);
        }
    }

    public ContextStruct SelectorVisit(Selector node, MultiScope scope)
    {
        ContextStruct context = new ContextStruct();
        
        string source = node.source.source;

        if (source == "board") 
        {
            context = References.allBoard;
        }

        else if (source == "hand")
        {
            if(scope.Get("$$$Card->\tFaction") as string == References.Player1Faction)
            {
                context.Add(References.PaladinsHand);
            }

            else
            {
                context.Add(References.MonstersHand);
            }
        }

        else if (source == "otherHand")
        {
            Debug.Log("paso por aqui para ver la otra mano");
            if(scope.Get("$$$Card->\tFaction") as string == References.Player1Faction)
            {
                context.Add(References.MonstersHand);
            }

            else
            {
                context.Add(References.PaladinsHand);
            }
        }

        else if (source == "deck")
        {
            if(scope.Get("$$$Card->\tFaction") as string == References.Player1Faction)
            {
                context.Add(References.PDeck);
            }

            else
            {
                context.Add(References.MDeck);
            }
        }

        else if (source == "Graveyard")
        {
            if(scope.Get("$$$Card->\tFaction") as string == References.Player1Faction)
            {
                context.Add(References.PGraveyard);
            }

            else
            {
                context.Add(References.MGraveyard);
            }
        }

        else if (source == "otherDeck")
        {
            if(scope.Get("$$$Card->\tFaction") as string == References.Player1Faction)
            {
                context.Add(References.MDeck);
            }

            else
            {
                context.Add(References.PDeck);
            }
        }

        else if (source == "field")
        {
            if(scope.Get("$$$Card->\tFaction") as string == References.Player1Faction)
            {
                context = References.PaladinsFaction;
            }

            else
            {
                context = References.MonsterFaction;
            }
        }

        else if (source == "otherField")
        {
            if(scope.Get("$$$Card->\tFaction") as string == References.Player1Faction)
            {
                context = References.MonsterFaction;
            }

            else
            {
                context = References.PaladinsFaction;
            }
        }
            
        FieldStruct allCardsAfterPredicate = new FieldStruct();
        
        foreach (CardStruct card in context.allCards.cardList)
        {
            Debug.Log("este es el nombre de la carta :  " + " " + card.card.name);
            
            SetValue(node.predicate.unit, scope, card);
            
            bool condition = (bool)Visit(node.predicate.condition, new MultiScope(scope));
            
            if (condition) 
            {
                allCardsAfterPredicate.Add(card);
            }
        }

        if (node.single == null) 
        {
            References.AfterPredicateFilter(allCardsAfterPredicate);
        }

        else 
        {
            References.AfterPredicateFilter(allCardsAfterPredicate, node.single.single);
        }

        context = References.allBoard;

        Debug.Log("Ahora viene el analisis de el nuevo contextstruct (se espera vacio)");

        foreach(CardStruct cardStruct in context.allCards.cardList)
        {
            string name = cardStruct.card.name;

            Debug.Log(name + " =>  esta carta pertenece al contexto que se esperaba vacio");
        }

        return context;
    }

    public MultiScope EffectOnActivationVisit(EffectOnActivation node, MultiScope outScope)
    {
        MultiScope scope = new MultiScope(outScope);

        if (node.parameters != null)
        {
            foreach (AST element in node.parameters.args)
            {
                Assign assign = element as Assign;
                
                scope.Set(assign.left.value, Visit(assign.right, scope));
            }
        }

        return scope;
    }

    public void EffectActionVisit(EffectNode node, MultiScope scope)
    {
        scope.Set(node.action.context.value, allBoard);

        Debug.Log(scope.scope.LOCAL_SCOPE[node.action.context.value]);

        scope.Set(node.action.targets.value, allBoard.allCards);

        Debug.Log("estos son los targets: " + node.action.targets.value);

        CompoundVisit(node.action.body, scope);
    }

    public void IfNodeVisit(IfNode node, MultiScope scope)
    {
        bool condition = (bool)Visit(node.condition, scope);
       
        if (condition) 
        {
            CompoundVisit(node.body, scope);
        }
    }

    public void WhileLoopVisit(WhileLoop node, MultiScope scope)
    {
        while ((bool)Visit(node.condition, scope))
        {
            CompoundVisit(node.body, scope);
        }       
    }

    public void ForLoopVisit(ForLoop node, MultiScope scope)
    {   
        FieldStruct field = Visit(node.targets, scope) as FieldStruct;

        foreach (CardStruct card in field.cardList)
        {
            Debug.Log("esta carta esta pasando por el bucle For : =>" + card.card.name);

            SetValue(node.target, scope, card);    
            
            CompoundVisit(node.body, scope);
        }
    }

    public void CompoundVisit(Compound node, MultiScope outScope)
    {
        MultiScope scope = new MultiScope(outScope);

        foreach (AST child in node.children)
        {

            Debug.Log("este nodo :  " + child + " es hijo de el cuerpo del efecto");

            Visit(child, scope);
        }
    }

    public void CardVisit(CardNode node)
    {
        string name = node.name.paramName;

        string type = (node.typeNode.type == "Gold") ? "GoldenCard" : "SilverCard" ;
        
        bool faction = (node.faction.faction == "Paladins") ? true : false;

        int faction1 = (node.faction.faction == "Paladins") ? 1 : 0;
        
        int power = node.power.power;
        
        string range = node.range.range;

        string scriptableObjectPath = "Assets/Prefabs/Cards/UserCards/UserCardSO.asset";

        Card2 cardScriptableObject = ScriptableObject.CreateInstance<Card2>();

        cardScriptableObject.name = name;

        cardScriptableObject.classCard = type;
        
        cardScriptableObject.playerID = faction;

        cardScriptableObject.Faction= faction1;
        
        cardScriptableObject.power = power;
        
        cardScriptableObject.basedPower = power;
        
        cardScriptableObject.Zone = range;
        
        cardScriptableObject.whichEffectIs = 9;
        
        cardScriptableObject.description = "Creacion del usuario (con suerte funciona)";
        
        cardScriptableObject.interpreter = this;

        AssetDatabase.CreateAsset(cardScriptableObject, scriptableObjectPath);

        AssetDatabase.SaveAssets();

        GameObject deck = References.gameObject;

        

        Debug.Log("esta llegando hasta aqui");
        
       if(cardScriptableObject.playerID)
       {
            Debug.Log("esta entrando en la faction 1");

            int count = deck.GetComponent<DrawCard>().Cards.Count; 

            deck.GetComponent<DrawCard>().Cards[count - 1].GetComponent<CardDisplay>().card = cardScriptableObject;

            deck.GetComponent<DrawCard>().Cards.Reverse();

            deck.GetComponent<DrawCard2>().Cards.Reverse();
       }

       else
       {
            Debug.Log("esta entrando en la faction 2");

            int count = deck.GetComponent<DrawCard2>().Cards.Count; 

            deck.GetComponent<DrawCard2>().Cards[count - 1].GetComponent<CardDisplay>().card = cardScriptableObject;

            deck.GetComponent<DrawCard2>().Cards.Reverse();

            deck.GetComponent<DrawCard>().Cards.Reverse();

       }
    }

    public void InterpretEffectToPlay()// RUN FROM OUTSIDE...
    {
        Debug.Log("Entro a interpretar el efecto");
        MultiScope outScope = new MultiScope(globalScope);
       
        CardNode card = parser.ThisCardNode;
       
        outScope.Set("$$$Card->\tFaction", (card.faction.faction == "Paladins") ? "Paladins" : "Monsters");

        MultiScope scope = new MultiScope(outScope);

        OnActivationVisit(card.onActivation, scope);

    }
}
