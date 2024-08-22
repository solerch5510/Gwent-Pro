using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CardNode : AST
{
    public ParamName name;
    public Type type;
    public Faction faction;
    public BasedPower power;
    public Range range;
    public OnActivation onActivation;

    public CardNode()
    {
        name = null;

        type = null;

        faction = null;

        power = null;

        range = null;

        onActivation = null;
    }

    public CardNode(ParamName name, Type type, Faction faction, BasedPower power, Range range, OnActivation onActivation)
    {
        this.name = name;

        this.type = type;

        this.faction = faction;

        this.power = power;

        this.range = range;

        this.onActivation = onActivation;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Card: ");

        if (name != null) name.Express(Height + "\t");

        if (type != null) type.Express(Height + "\t");

        if (faction != null) faction.Express(Height + "\t");

        if (power != null) power.Express(Height + "\t");

        if (range != null) range.Express(Height + "\t");

        if (onActivation != null) onActivation.Express(Height + "\t");
    }
}

public class Type : AST
{
    public string type;

    public Type (Token token)
    {
        type = token.Lexeme;
    }

    public override void Express(string Height)
    {
        if (type != null)
        {
            Debug.Log(Height + "-> Type: " + type);
        } 
    }
}

public class Faction : AST
{
    public string faction;

    public Faction(Token token)
    {
        faction = token.Lexeme;
    }

    public override void Express(string Height)
    {
        if (faction != null)
        {
            Debug.Log(Height + "-> Faction: " + faction);  
        } 
    }
}

public class BasedPower : AST
{
    public int power;

    public BasedPower(Token token)
    {
        power = int.Parse(token.Lexeme);
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> BasedPower: " + power);
    }
}

public class ShowPower : AST
{
    public ShowPower()
    {

    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> ShowPower");
    }
}

public class Range : AST
{
    public string range;

    public Range(Token token)
    {
        range = token.Lexeme;
    }

    public override void Express(string Height)
    {
        if (range != null)
        {
            Debug.Log(Height + "-> Range: " + range);
        }
    }
}

public class OnActivation : AST
{
    public List<OnActivationElement> onActivation;

    public OnActivation()
    {
        onActivation = new List<OnActivationElement>();
    }

    public OnActivation(List<OnActivationElement> onActivation)
    {
        this.onActivation = onActivation;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> OnActivation: ");

        if (onActivation != null)
        {
            foreach (OnActivationElement element in onActivation)
            {
                element.Express(Height + "\t");
            }
        }
    }
}

public class OnActivationElement : AST
{
    public EffectOnActivation effectOnActivation;
    public Selector selector;
    public PostAction postAction;

    public OnActivationElement(EffectOnActivation effectOnActivation, Selector selector)
    {
        this.effectOnActivation = effectOnActivation;
        
        this.selector = selector;
        
        postAction = null;
    }

    public OnActivationElement(EffectOnActivation effectOnActivation, Selector selector, PostAction postAction)
    {
        this.effectOnActivation = effectOnActivation;
        
        this.selector = selector;
        
        this.postAction = postAction;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> OnActivationELEMENT: ");
        if (effectOnActivation != null)
        {
            effectOnActivation.Express(Height + "\t");
        } 

        if (selector != null)
        {
            selector.Express(Height + "\t");
        } 

        if (postAction != null)
        {
            postAction.Express(Height + "\t");
        } 
    }
}

public class EffectOnActivation : AST
{
    public ParamName name;
    public Args parameters;

    public EffectOnActivation(ParamName name)
    {
        this.name = name;

        parameters = null;
    }

    public EffectOnActivation(ParamName name, Args parameters)
    {
        this.name = name;

        this.parameters = parameters;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> EffectOnActivation: ");
        if (name != null)
        {
            name.Express(Height + "\t");
        } 
        
        if (parameters != null) 
        {
            parameters.Express(Height + "\t");
        }
    }
}

public class PostAction : AST
{
    public Type type;
    public Selector selector;

    public PostAction(Type type)
    {
        this.type = type;

        selector = null;
    }

    public PostAction(Type type, Selector selector)
    {
        this.type = type;

        this.selector = selector;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-PostAction: ");

        if (type != null) 
        {
            type.Express(Height + "\t");
        }

        if (selector != null) 
        {
            selector.Express(Height + "\t");
        }
    }
}

public class Selector : AST
{
    public Source source;
    public Single single;
    public Predicate predicate;

    public Selector (Source source, Predicate predicate)
    {
        this.source = source;

        this.predicate = predicate;

        single = new Single(new Token(TokenType.Bool, "false", 0, 0));
    }

    public Selector(Source source, Single single, Predicate predicate)
    {
        this.source = source;

        this.single = single;

        this.predicate = predicate;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Selector: ");

        if (source != null)
        {
            source.Express(Height + "\t");
        }

        if (single != null) 
        {
            single.Express(Height + "\t");
        }

        if (predicate != null) 
        {
            predicate.Express(Height + "\t");
        }
    }
}

public class Single : AST
{
    public bool single;

    public Single (Token token)
    {
        if (token.Type == TokenType.Bool)
        {
            if (token.Lexeme == "true") 
            {
                single = true;
            }

            else 
            {
                single = false;
            }
        }
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Single: " + single);
    }
}

public class Source : AST
{
    public string source;

    public Source(Token token)
    {
        source = token.Lexeme;
    }

    public override void Express(string Height)
    {
        if (source != null) 
        {
            Debug.Log(Height + "Source: " + source);
        }
    }
}

public class Predicate : AST
{
    public Var unit;
    public AST condition;

    public Predicate(Var unit, AST condition)
    {
        this.unit = unit;

        this.condition = condition;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Predicate: ");

        if (unit != null) 
        {
            unit.Express(Height + "\t");
        }

        if (condition != null)
        {
            condition.Express(Height + "\t");
        } 
    }
}