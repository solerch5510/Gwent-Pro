using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Args : AST
{
    public List<AST> args;

    public Args()
    {
        args = new List<AST>();
    }

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

public class NoOp : AST
{
    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Empty");
    }
}