using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token
{
    // Tipo de Token
    public TokenType Type {get; private set;}

    //Lexema asociado al Token
    public string Lexeme {get; private set;}

    // Liinea donde se encontro el Token
    public int Line {get; set;}

    // Columna donde se encontro el Token
    public int Column {get; set;}

    public Token(TokenType type, string lexeme, int line, int column)
    {
        Type = type;

        Lexeme = lexeme;

        Line = line;

        Column = column;
    }
}

public enum TokenType 
{
    // Literales
    Identifier, StringLiteral, NumberLiteral, Bool,

    // Tokens Condicionales 
    And, Or, Not, Greater, Less, GreaterEqual, LessEqual, EqualGreater,
    Equal, Bang, Differ, Null,

    // Tokens de un solo caracter
    Colon, SemiColon, Slash,  Comma, LeftParenthesis, RightParenthesis, LeftBrace, RightBrace,
    LeftBracket,RightBracket, Dot,

    // Palabras Clave
    If, For, While, Action, Card, Effect, Name, Params, Type, Faction, Power, Range,
    OnActivation, Targets, Context, Single, Predicate, PostAction, In, OnActivation_Effect, Owner,

    // Indicador de fin de archivo.
    EndOfFile,

    //Funciones
    Function, Pointer,

    //Declaracion de variables
    Var_Int, Var_Bool, Var_String,

    //Operadores
    Plus, Minus, Multiply, Divide, Mod, Pow, Plus1, String_Sum, String_Sum_S, Decrement, Assign,




}