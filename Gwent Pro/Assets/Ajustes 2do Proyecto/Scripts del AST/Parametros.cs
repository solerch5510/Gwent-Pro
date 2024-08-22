using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
public class ParamName : AST
{
    public string paramName;

    public ParamName (Token token)
    {
        paramName = token.Lexeme;
    }

    public override void Express(string height)
    {
        if (paramName != null)
        {
            Debug.Log(height + "-> Name: " + paramName);  
        } 
    }
}