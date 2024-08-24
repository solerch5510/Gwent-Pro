using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Clase base para representar una variable en el arbol abstracto de sintaxis (AST)
public class Var : AST
{
    // Enumeracion para los diferentes tipos de variables que pueden existir
    public enum Type
    {
        TARGETS, CONTEXT, CARD, FIELD, INT, STRING, BOOL, VOID, NULL
    }

    // Variables para almacenar el token asociado con esta variable, su valor y su tipo
    public Token token;
    public string value;
    public Type type;

    // Constructor que inicializa la variable con un token dado
    public Var(Token token)
    {
        this.token = token;

        value = token.Lexeme;

        // Inicializa el tipo de la variable como NULL por defecto
        type = Type.NULL;
    }

    public void TypeInParams(TokenType t) // Metodo para determinar el tipo de la variable basandose en el tipo de token
    {
        if (t == TokenType.Var_Bool) type = Type.BOOL;

        if (t == TokenType.Var_Int) type = Type.INT;

        if (t == TokenType.Var_String) type = Type.STRING;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Variable: ");

        Debug.Log(Height + "Token: " + token.Type + " " + token.Lexeme);

        Debug.Log(Height + "Type: " + type.ToString());

        Debug.Log(Height + "Value: " + value);
    }
}

// Clase derivada de Var para representar una variable compuesta, es decir, una variable que puede contener multiples valores o operaciones
public class VarComp : Var
{
    // Lista para almacenar los argumentos o valores asociados con esta variable compuesta
    public List<AST> args;

    // Constructor que inicializa la variable compuesta con un token dado
    public VarComp(Token token) : base(token)
    {
        args = new List<AST>();
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> VarCompound:");

        base.Express(Height);

        // Si la lista de argumentos no es nula, imprime cada uno de ellos
        if (args != null) foreach (AST ast in args)
        {
            ast.Express(Height + "\t");
        }
    }
}