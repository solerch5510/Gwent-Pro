using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope <T>
{
    public Dictionary<string, T> LOCAL_SCOPE;
    public Scope<T> GLOBAL_SCOPE;

    public Scope()
    {
        GLOBAL_SCOPE = null;

        LOCAL_SCOPE = new Dictionary<string, T>();
    }
    public Scope(Scope<T> GLOBAL_SCOPE)
    {
        this.GLOBAL_SCOPE = GLOBAL_SCOPE;

        LOCAL_SCOPE = new Dictionary<string, T>();
    }


    public bool IsInScope(string name)
    {
        if (GLOBAL_SCOPE == null) 
        {
            return LOCAL_SCOPE.ContainsKey(name);
        }

        else if (LOCAL_SCOPE.ContainsKey(name)) 
        {
            return true;
        }

        else return GLOBAL_SCOPE.IsInScope(name);
    }

    public bool IsInScope(ParamName name)
    {
        return IsInScope(name.paramName);
    }
    public bool IsInScope(Var variable)
    {
        return IsInScope(variable.value);
    }
    public bool IsInScope(EffectNode effect)
    {
        return IsInScope(effect.name.paramName);
    }
    public bool IsInScope(CardNode card)
    {
        return IsInScope(card.name.paramName);
    }


    public T Get(string name)
    {
        if (GLOBAL_SCOPE == null)
        {
            if (LOCAL_SCOPE.ContainsKey(name)) 
            {
                return LOCAL_SCOPE[name];
            }

            else
            {
                Debug.Log($"'{name}' not found");
                return default;
            }
        }

        else if (LOCAL_SCOPE.ContainsKey(name)) 
        {
            return LOCAL_SCOPE[name];
        }

        else return GLOBAL_SCOPE.Get(name);
    }
    public T Get(ParamName name)
    {
        return Get(name.paramName);
    }
    public T Get(Var variable)
    {
        return Get(variable.value);
    }
    public T Get(EffectNode effect)
    {
        return Get(effect.name.paramName);
    }
    public T Get(CardNode card)
    {
        return Get(card.name.paramName);
    }


    public void Set(string name, T value)
    {
        if (!IsInScope(name) || LOCAL_SCOPE.ContainsKey(name)) 
        {
            LOCAL_SCOPE[name] = value;
        }

        else GLOBAL_SCOPE.Set(name, value);
    }
    public void Set(ParamName name, T value)
    {
        Set(name.paramName, value);
    }
    public void Set(Var variable, T value)
    {
        Set(variable.value, value);
    }
    public void Set(EffectNode effect, T value)
    {
        Set(effect.name.paramName, value);
    }
    public void Set(CardNode card, T value)
    {
        Set(card.name.paramName, value);
    }
}

public class MultiScope
{
    Scope<object> scope;

    public MultiScope()
    {
        scope = new Scope<object>();
    }

    public MultiScope(MultiScope globalScope)
    {
        scope = new Scope<object>(globalScope.scope);
    }

    public bool IsInScope(string key)
    {
        return scope.IsInScope(key);
    }

    public object Get(string key)
    {
        return scope.Get(key);
    }

    public void Set(string key, object value)
    {
        scope.Set(key, value);
    }
}