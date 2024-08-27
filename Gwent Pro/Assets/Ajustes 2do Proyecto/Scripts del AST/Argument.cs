using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Clase para representar argumentos en el AST
public class Args : AST
{
     // Lista para almacenar los argumentos pasados a una funcion o metodo
    public List<AST> args;

    public Args()
    {
        args = new List<AST>();
    }

    // Metodo para agregar un nuevo argumento a la lista
    public void Add(AST argument)
    {
        args.Add(argument);
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Arguments: ");

        if (args != null)
        {
            foreach (AST child in args)
            {
                child.Express(Height + '\t');
            }
        } 
    }
}

public class NoOp : ASTType
{
    public NoOp()
    {

    }
    
    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Empty");
    }
}