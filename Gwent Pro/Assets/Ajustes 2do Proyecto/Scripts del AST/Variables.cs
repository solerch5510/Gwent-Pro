using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Var : AST
{
    public enum Type
    {
        TARGETS, CONTEXT, CARD, FIELD, INT, STRING, BOOL, VOID, NULL
    }

    public Token token;
    public string value;
    public Type type;

    public Var(Token token)
    {
        this.token = token;

        value = token.Lexeme;

        type = Type.NULL;
    }

    public void TypeInParams(TokenType t)
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

public class VarComp : Var
{
    public List<AST> args;

    public VarComp(Token token) : base(token)
    {
        args = new List<AST>();
    }

    public override void Express(string Height)
    {
        Debug.Log(Height + "-> VarCompound:");

        base.Express(Height);

        if (args != null) foreach (AST ast in args)
        {
            ast.Express(Height + "\t");
        }
    }
}