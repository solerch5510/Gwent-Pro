using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Xml.Serialization;

// Clase base para representar una variable en el arbol abstracto de sintaxis (AST)
public class Var : ASTType
{
    // Variables para almacenar el token asociado con esta variable, su valor y su tipo
    public Token token;
    public string value;

    // Constructor que inicializa la variable con un token dado
    public Var(Token token)
    {
        this.token = token;

        value = token.Lexeme;

        type = Type.Null;
    }

    public Var (Token token, TokenType type)
    {
        this.token = token;

        value = token.Lexeme;

        TypeInParams(type);
    }

    public void TypeInParams(TokenType t) // Metodo para determinar el tipo de la variable basandose en el tipo de token
    {
        if (t == TokenType.Var_Bool) 
        {
            type = Type.Bool;
        }

        if (t == TokenType.Var_Int) 
        {
            type = Type.Int;
        }

        if (t == TokenType.Var_String) 
        {
            type = Type.String;
        }
    }

    public Var(Token token, Type type)
    {
        this.token = token;

        value = token.Lexeme;

        this.type = type;
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
    public List<ASTType> args;

    // Constructor que inicializa la variable compuesta con un token dado
    public VarComp(Token token) : base(token)
    {
        args = new List<ASTType>();
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> VarCompound:");

        base.Express(Height);

        string Tab = "\t";

        // Si la lista de argumentos no es nula, imprime cada uno de ellos
        if (args != null) 
        {
            foreach (AST ast in args)
            {
                ast.Express(Height + Tab);

                Tab += "\t";
            }
        }
    }
}

public class Pointer : ASTType
{
    public string pointer;

    public Pointer(Token token)
    {
        pointer = token.Lexeme;

        type = Type.Field;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-POINTER: " + pointer);
    }
}

public class Indexer : ASTType
{
    public ASTType index;

    public Indexer(ASTType index)
    {
        this.index = index;

        type = Type.Indexer;
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> Indexer: ");

        if (index != null) 
        {
            index.Express(Height + "\t-> Index: ");
        }
    }
}