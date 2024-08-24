using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EffectNode : AST
{
    // Nombre del parametro del efecto
    public ParamName name;
    // Parametros asociados al efecto
    public Args parameters;
    // Accion asociada al efecto
    public Action action;

    public EffectNode(ParamName name, Action action)
    {
        this.name = name;

        parameters = null;

        this.action = action;
    }

    public EffectNode(ParamName name, Args parameters, Action action)
    {
        this.name = name;

        this.parameters = parameters;

        this.action = action;
    }

    // Metodo para expresar el nodo de efecto en un formato legible
    public override void Express(string Height)
    {
        Debug.Log(Height + "-> EffectNode: ");

        if (name != null) 
        {
            name.Express(Height + "\t");
        }

        if (parameters != null) 
        {
            parameters.Express(Height + "\t");
        }

        if (action != null) 
        {
            action.Express(Height + "\t");
        }
    }
}

// Clase para representar acciones dentro de un nodo de efecto
public class Action : AST
{
    // Objetivos de la accion
    public Var targets;
    // Contexto de la accion
    public Var context;
    // Cuerpo de la accion, que puede contener multiples instrucciones
    public Compound body;

    public Action(Var targets, Var context, Compound body)
    {
        this.targets = targets;

        this.context = context;

        this.body = body;
    }

    // Metodo para expresar la accion en un formato legible
    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Action: ");

        if (targets != null) 
        {
            targets.Express(Height + "-> Targets: \t");
        }
        if (context != null) 
        {
            context.Express(Height + "-> Context: \t");
        }
        if (body != null) 
        {
            body.Express(Height + "-> Body: \t");
        }
    }
}