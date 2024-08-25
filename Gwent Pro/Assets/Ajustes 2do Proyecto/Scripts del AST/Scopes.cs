using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Define una clase generica Scope<T> para manejar ambitos en el codigo
public class Scope <T>
{
    //Diccionario para almacenar variables locales en el ambito actual
    public Dictionary<string, T> LOCAL_SCOPE;
    // Referencia al ambito global, si existe
    public Scope<T> GLOBAL_SCOPE;


    // Constructor predeterminado que inicializa un nuevo ambito local sin ambito global
    public Scope()
    {
        GLOBAL_SCOPE = null; // No hay ambito global

        LOCAL_SCOPE = new Dictionary<string, T>(); // Inicializa el ambito local vacio
    }

    // Constructor que acepta un ambito global y crea un nuevo ambito local dentro de el
    public Scope(Scope<T> GLOBAL_SCOPE)
    {
        this.GLOBAL_SCOPE = GLOBAL_SCOPE;

        LOCAL_SCOPE = new Dictionary<string, T>();
    }

     // Verifica si una variable esta definida en el ambito actual o en el ambito global
    public bool IsInScope(string name)
    {
         // Si no hay ambito global, solo busca en el ambito local
        if (GLOBAL_SCOPE == null) 
        {
            return LOCAL_SCOPE.ContainsKey(name);
        }

        // Si hay ambito global, primero busca en el ambito local, luego en el global
        else if (LOCAL_SCOPE.ContainsKey(name)) 
        {
            return true;
        }

        else return GLOBAL_SCOPE.IsInScope(name);
    }

    // Metodos similares a IsInScope pero adaptados para diferentes tipos de nombres
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

// Clase adicional para manejar multiples ambitos (scopes) aniidados
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