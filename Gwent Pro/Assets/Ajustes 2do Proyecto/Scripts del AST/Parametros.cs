using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Clase para representar un nombre de parametro en el árbol abstracto de sintaxis (AST)
public class ParamName : AST
{
    // Variable para almacenar el nombre del parametro
    public string paramName;

    // Constructor que toma un token y extrae su lexema para usarlo como nombre del parametro
    public ParamName (Token token)
    {
        paramName = token.Lexeme;
    }

    public override void Express(string height)
    {
        if (paramName != null)
        {
            // Imprime el nivel de indentacion actual seguido del nombre del parametro
            Debug.Log(height + "-> Name: " + paramName);  
        } 
    }
}