using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Clase para representar un literal entero en el arbol abstracto de sintaxis (AST
public class Int: ASTType
{
    // Token que representa la entrada lexica asociada con este literal
    public Token IntToken;
    // Valor numerico del literal
    public int value;

    // Constructor que inicializa el token y parsea su valor como entero
    public Int(Token token)
    {
        this.IntToken = token;

        value = int.Parse(token.Lexeme);

        type = Type.Int;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Int:  Token:"  + IntToken.Type.ToString() + " " + IntToken.Lexeme);
    }
}

// Clase para representar un literal de cadena en el AST
public class String : ASTType
{
    // Token que representa la entrada lexica asociada con esta cadena
    public Token StringToken;

    // Cadena de texto representada por este nodo    
    public string Expression;

    // Constructor que inicializa el token y asigna su lexema como la expresion
    public String(Token token)
    {
        this.StringToken = token;

        Expression = token.Lexeme;

        type = Type.String;
    }

    public override void Express(string Height)
    {
        // Si hay una expresion, imprime la cadena de texto
        if (Expression != null) Debug.Log(Height + "-> String: \nText: " + Expression);
    }
}

// Clase para representar un literal booleano en el AST
public class Bool: ASTType
{
    // Token que representa la entrada lexica asociada con este valor booleano
    public Token BoolToken;

    // Valor booleano representado por este nodo
    public bool TOF;

    // Constructor que inicializa el token y parsea su valor como booleano
    public Bool(Token token)
    {
        this.BoolToken = token;

        TOF = bool.Parse(token.Lexeme);

        type = Type.Bool;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Bool:  Value:" + TOF);
    }
}