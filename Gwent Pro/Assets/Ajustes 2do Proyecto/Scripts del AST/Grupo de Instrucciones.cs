using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Compound : AST
{
    // Lista para almacenar las instrucciones individuales que conforman este grupo
    public List<AST> children;

    // Constructor para inicializar un nuevo grupo de instrucciones vacio
    public Compound()
    {
        children = new List<AST>();
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Compound:");

        Debug.Log(Height + "-> Childrens: " + children.Count);

        // Verifica si hay instrucciones en el grupo y, en caso afirmativo, imprime cada una de ellas
        if (children != null) 
        {
            foreach (AST child in children)
            {
                child.Express(Height + '\t');
            }
        }
    }
}

public class IfNode : AST // Clase para representar un nodo IF en el AST
{
    public AST condition; // Atributo para almacenar la condicion del nodo IF
    public Compound body; // Atributo para almacenar el cuerpo del bloque IF

    public IfNode(AST condition, Compound body)
    {
        // Expresa la condición del nodo IF
        this.condition = condition;

        // Expresa el cuerpo del bloque IF
        this.body = body;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> IfNode: ");

        if (condition != null)
        {
            condition.Express(Height + "-> Condition: ");
        } 
        if (body != null) 
        {
            body.Express(Height + "-Body: \t");
        }
    }
}

// Clase para representar un bucle FOR en el AST
public class ForLoop : AST
{
    // Atributo para almacenar el cuerpo del bucle FOR
    public Compound body;

    // Atributos para almacenar la variable objetivo y los objetivos del bucle FOR
    public Var target;
    public Var targets;

    public ForLoop(Var target, Var targets, Compound body)
    {
        this.target = target;

        this.body = body;

        this.targets = targets;
    }

    public override void Express(string Height)
    {
        if (target != null) 
        {
            target.Express(Height + "-> Target: ");
        }

        if (targets != null) 
        {
            targets.Express(Height + "-> Targets: ");
        }

        if (body != null) 
        {
            body.Express(Height + "-> Body: \t");
        }
    }
}

// Clase para representar un bucle WHILE en el AST
public class WhileLoop : AST
{
    // Atributo para almacenar la condición del bucle WHILE
    public AST condition;

    // Atributo para almacenar el cuerpo del bucle WHILE
    public Compound body;

    public WhileLoop (AST condition, Compound body)
    {
        this.condition = condition;

        this.body = body;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> WhileLoop: ");

        if (condition != null) 
        {
            condition.Express(Height + "-> Condition:");
        }
        if (body != null) 
        {
            body.Express(Height + "-> Body: \t");
        }
    }
}

// Clase para representar una funcion en el AST
public class Function : ASTType
{
    // Nombre de la funcion
    public string functionName;
    // Argumentos de la funcion
    public Args args;

    public Function (string functionName)
    {
        this.functionName = functionName;

        TypeToReturn();
    }

    public Function(string functionName, Args args)
    {
        this.functionName = functionName;

        this.args = args;

        TypeToReturn();
    }

    public void TypeToReturn()
    {
        // Determina el tipo de retorno basado en el nombre de la funcion
        if (functionName == "FieldOfPlayer")
        {
            type = Type.Field;
        }

        if (functionName == "HandOfPlayer")
        {
            type = Type.Field;
        }
            
        if (functionName == "GraveyardOfPlayer")
        {
            type = Type.Field;
        }
            
        if (functionName == "DeckOfPlayer")
        {
            type = Type.Field;
        }
            
        if (functionName == "Push")
        {
            type = Type.Void;
        }
            
        if (functionName == "SendBottom")
        {
            type = Type.Void;
        }
            
        if (functionName == "Pop")
        {
            type = Type.Card;
        }
            
        if (functionName == "Remove")
        {
            type = Type.Void;
        }
            
        if (functionName == "Shuffle")
        {
            type = Type.Void;
        }

        if (functionName == "Add")
        {
            type = Type.Void;
        }

        if (functionName == "Find")
        {
            type = Type.Field;
        }
            
    }

    public override void Express(string Height)
    {
        // Imprime informacion sobre la funcion actual
        if (functionName != null) 
        {
            Debug.Log(Height + "-> Function " + functionName + ":");
        }

        // Imprime el tipo de retorno de la funcion
        Debug.Log("Type to return: " + type.ToString());

        // Si existen argumentos, los imprime
        if (args != null) 
        {
            args.Express(Height + "-> Parameters: ");
        }
    }
}

public class Assign : AST
{
    public Var left; // Variable a asignar
    public Token op; // Operador de asignación
    public ASTType right; // Valor o expresión a asignar

    public Assign(Var left, Token op, ASTType right)
    {
        this.left = left; // Asigna la variable izquierda

        this.op = op; // Asigna el operador

        this.right = right; // Asigna el valor o expresión derecha
    }

    public override void Express(string Height)
    {
        // Imprime información sobre la asignacion
        Debug.Log(Height + "-> Assignment:");

        // Si existe una variable izquierda, muestra su nombre
        if (left != null) 
        {
            left.Express(Height + "\t-> Variable: \t");
        }
        
        // Muestra el operador de asignacion
        Debug.Log(Height + "-> Operator: " + op.Type.ToString() + " (" + op.Lexeme + ")");

        // Si existe un valor o expresión derecha, lo muestra
        if (right != null) 
        {
            right.Express(Height + "\t-Value: \t");
        }
    }
}