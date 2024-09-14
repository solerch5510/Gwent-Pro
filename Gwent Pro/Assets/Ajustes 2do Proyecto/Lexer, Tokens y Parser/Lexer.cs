using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using Unity.VisualScripting;
using TMPro;

public class Lexer
{
    // Variable para almacenar el texto fuente que se va a analizar
    public string sourceText;
    //Lista para almacenar todos los tokens identificados durante el analisis
    public List<Token> tokenList = new List<Token>();
    // Variable para almacenar el indice de la posicion actual en el texto fuente
    public int currentPosition = 0;
    //Posicion inicial desde donde comenzo el ultimo Token
    private int startPosition = 0;
    // Numero de linea actual en el texto fuente.
    private int currentLine = 1;
    // Diccionario para almacenar palabras reservadas y sus correspondientes Tokens
    public Dictionary<string, Token> reservedWords = new Dictionary<string, Token>();
    
    //Constructor que inicializa el lexer con el texto fuente dado
    public Lexer(string sourceText)
    {
        this.sourceText = sourceText;

        for(int i = 0 ; i < sourceText.Length + 1; i ++ )
        {
            if(i == sourceText.Length)
            {
                AddToken(TokenType.EndOfFile, currentLine , currentPosition);
            }

            LexToken();
        }

        InitializeReservedWords();
    }

    // Metdo privado para inicializar el diccionario de palabras reservadas.
    private void InitializeReservedWords()
    {
        reservedWords.Add("while", new Token(TokenType.While, "while", 0, 0));

        reservedWords.Add("for", new Token(TokenType.For, "for", 0, 0));

        reservedWords.Add("in", new Token(TokenType.In, "in", 0, 0));

        reservedWords.Add("Name", new Token(TokenType.Name, "Name", 0, 0));

        reservedWords.Add("card", new Token(TokenType.Card, "Card", 0, 0));

        reservedWords.Add("effect", new Token(TokenType.Effect, "effect", 0, 0));

        reservedWords.Add("Predicate", new Token(TokenType.Predicate, "Predicate", 0, 0));

        reservedWords.Add("Params", new Token(TokenType.Params, "Params", 0, 0));

        reservedWords.Add("Number", new Token(TokenType.Var_Int, "Number", 0, 0));

        reservedWords.Add("String", new Token(TokenType.Var_String, "String", 0, 0));

        reservedWords.Add("Bool", new Token(TokenType.Var_Bool, "Bool", 0, 0));

        reservedWords.Add("true", new Token(TokenType.Bool, "true", 0, 0));

        reservedWords.Add("false", new Token(TokenType.Bool, "false", 0, 0));

        reservedWords.Add("TriggerPlayer", new Token(TokenType.Pointer, "TriggerPlayer", 0, 0));

        reservedWords.Add("Action", new Token(TokenType.Action, "Action", 0, 0));

        reservedWords.Add("Hand", new Token(TokenType.Pointer, "Hand", 0, 0));

        reservedWords.Add("Field", new Token(TokenType.Pointer, "Field", 0, 0));

        reservedWords.Add("Graveyard", new Token(TokenType.Pointer, "Graveyard", 0, 0));

        reservedWords.Add("Deck", new Token(TokenType.Pointer, "Deck", 0, 0));

        reservedWords.Add("HandOfPlayer", new Token(TokenType.Function, "HandOfPlayer", 0, 0));

        reservedWords.Add("FieldOfPlayer", new Token(TokenType.Function, "FieldOfPlayer", 0, 0));

        reservedWords.Add("GraveyardOfPlayer", new Token(TokenType.Function, "GraveyardOfPlayer", 0, 0));

        reservedWords.Add("DeckOfPlayer", new Token(TokenType.Function, "DeckOfPlayer", 0, 0));

        reservedWords.Add("Find", new Token(TokenType.Function, "Find", 0, 0));

        reservedWords.Add("Push", new Token(TokenType.Function, "Push", 0, 0));

        reservedWords.Add("SendBottom", new Token(TokenType.Function, "SendBottom", 0, 0));

        reservedWords.Add("Pop", new Token(TokenType.Function, "Pop", 0, 0));

        reservedWords.Add("Remove", new Token(TokenType.Function, "Remove", 0, 0));

        reservedWords.Add("Shuffle", new Token(TokenType.Function, "Shuffle", 0, 0));

        reservedWords.Add("Type", new Token(TokenType.Type, "Type", 0, 0));

        reservedWords.Add("Faction", new Token(TokenType.Faction, "Faction", 0, 0));

        reservedWords.Add("Power", new Token(TokenType.Power, "Power", 0, 0));

        reservedWords.Add("Range", new Token(TokenType.Range, "Range", 0, 0));

        reservedWords.Add("OnActivation", new Token(TokenType.OnActivation, "OnActivation", 0, 0));

        reservedWords.Add("Effect", new Token(TokenType.OnActivation_Effect, "Effect", 0, 0));

        reservedWords.Add("PostAction", new Token(TokenType.PostAction, "PostAction", 0, 0));

        reservedWords.Add("Target", new Token(TokenType.Targets, "Target", 0, 0));

        reservedWords.Add("Context", new Token(TokenType.Context, "Context", 0, 0));

        reservedWords.Add("Single", new Token(TokenType.Single, "Single", 0, 0));

        reservedWords.Add("if", new Token(TokenType.If , "if", 0, 0));

        reservedWords.Add("Board", new Token(TokenType.Pointer, "Board", 0, 0));

        reservedWords.Add("Add", new Token(TokenType.Function, "Add", 0, 0));

    }

    //Verifica si se llego al final del texto fuente
    private bool isAtEnd()
    {
        return currentPosition >= sourceText.Length;
    }

    //Avanza la posicion actual (currentPosition) y devuelve el caracter sin avanzar
    private char Advance()
    {
        if(sourceText.Length == currentPosition || sourceText.Length == currentPosition + 1)
        {
            return '\0';
        }

        return sourceText[currentPosition++];
    }

    //Anade un nuevo token a la lista de tokens
    private void AddToken(TokenType type, int line, int position)
    {
        string text = sourceText.Substring(startPosition, currentPosition - startPosition);

        tokenList.Add(new Token(type, text, line, position));

        startPosition = currentPosition;
    }

    //Analisis de Tokens
    public void LexToken()
    {
        char character = Advance();

        switch (character)
        {
            //Casos para operadores y delimitadores.
            case '(':
                AddToken(TokenType.LeftParenthesis, currentLine, currentPosition);
                break;
            case ')':
                AddToken(TokenType.RightParenthesis, currentLine, currentPosition);
                break;
            case '{':
                AddToken(TokenType.LeftBrace, currentLine, currentPosition);
                break;
            case '}':
                AddToken(TokenType.RightBrace, currentLine, currentPosition);
                break;
            case '[':
                AddToken(TokenType.LeftBracket, currentLine, currentPosition);
                break;
            case ']':
                AddToken(TokenType.RightBracket, currentLine, currentPosition);
                break;
            case ',':
                AddToken(TokenType.Comma, currentLine, currentPosition);
                break;
            case '.':
                AddToken(TokenType.Dot, currentLine, currentPosition);
                break;
            case ':':
                AddToken(TokenType.Colon, currentLine, currentPosition);
                break;
            case ';':
                AddToken(TokenType.SemiColon, currentLine, currentPosition);
                break;
            case '*':
                AddToken(TokenType.Multiply, currentLine, currentPosition);
                break;
            case '!':
                if(character == '=') AddToken(TokenType.BangEqual, currentLine, currentPosition);
                else AddToken(TokenType.Bang,currentLine, currentPosition);
                break;
            case '<':
                if(character == '=') AddToken(TokenType.LessEqual, currentLine, currentPosition);
                else AddToken(TokenType.Less, currentLine, currentPosition);
                break;
            case '>':
                if(character == '=') AddToken(TokenType.GreaterEqual, currentLine, currentPosition);
                else AddToken(TokenType.Greater, currentLine, currentPosition);
                break;
            case '|':
                if (IsMatch('|')) 
                {
                    AddToken(TokenType.Or, currentLine, currentPosition+1);
                    
                    currentPosition++;
                }
                break;
            case '&':
                if (IsMatch('&')) 
                {
                    AddToken(TokenType.And, currentLine, currentPosition+1);
                    
                    currentPosition++;
                }
                break;
            case '=':
                if (IsMatch('=')) 
                {
                    AddToken(TokenType.Equal, currentLine, currentPosition+1);
                    
                    currentPosition++;
                }
                else if (IsMatch('>')) 
                {
                    AddToken(TokenType.EqualGreater, currentLine, currentPosition+1);
                
                    currentPosition++;
                }
                else AddToken(TokenType.Assign, currentLine, currentPosition);
                break;
            case '+':
                if (IsMatch('+')) 
                {
                    AddToken(TokenType.Plus1, currentLine, currentPosition+1);
                    
                    currentPosition++;
                }
                
                else AddToken(TokenType.Plus, currentLine, currentPosition);
                break;
            case '-':
                if (IsMatch('-')) 
                {
                    AddToken(TokenType.Decrement, currentLine, currentPosition+1);
                    
                    currentPosition++;
                }
                else AddToken(TokenType.Minus, currentLine, currentPosition);
                break;
            case '/':
                if (IsMatch('/'))
                {
                    while(!isAtEnd() && Peek()!= '\n')
                    {
                        Advance();
                    }
                }
                else AddToken(TokenType.Slash, currentLine, currentPosition);
                break;
            case '%':
                AddToken(TokenType.Mod, currentLine, currentPosition);
                break;
            
            //Casos para saltos de linea y espacios
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                currentLine++;
                break;
            
            //Caso para cadenas de texto.
            case '"':
                ScanString();
                break;

            case '\0':
                tokenList.Add(new Token(TokenType.EndOfFile, "", currentLine, currentPosition));
                break;

            //Caso prederterminado para caracteres no reconocidos.
            default:
                if (IsDigit(character))
                {
                    ScanNumber();
                }
                else if (IsAlpha(character))
                {
                    ScanIdentifier();
                }
                else
                {
                    System.Console.WriteLine($"Unexpected character: {character}");
                }
                break;

        }
    }

    // Método para escanear múltiples tokens hasta llegar al final del texto
    private void ScanToken()
    {
        while (!isAtEnd())
        {
            startPosition = currentPosition;

            ScanToken();
        }

        tokenList.Add(new Token(TokenType.EndOfFile, "", currentLine, currentPosition));
    }

    // Método para verificar si el próximo carácter coincide con el esperado
    private bool IsMatch(char expected)
    {
        if (isAtEnd()) return false;

        return sourceText[currentPosition] == expected;
    }

    // Método para obtener el próximo carácter sin avanzar la posición actual.
    private char Peek()
    {
        if (isAtEnd()) return '\0';

        return sourceText[currentPosition];
    }

    // Método para escanear cadenas de texto.
    private void ScanString()
    {
        string text = "";

        startPosition = currentPosition;

        Debug.Log(sourceText[currentPosition]);

        while (sourceText[currentPosition] != '"')
        {
            if (Peek() == '\n') currentLine++;

            text += sourceText[currentPosition];

            Debug.Log(text);

            currentPosition++;
        }

        if (isAtEnd())
        {
            System.Console.WriteLine("Error");

            return;
        }

        currentPosition++;        

        tokenList.Add(new Token(TokenType.StringLiteral, text, currentLine, currentPosition));

        startPosition = currentPosition;

    }

    // Método para verificar si un carácter es un dígito.
    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    // Método para escanear números.
    private void ScanNumber()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && IsDigit(Peek()))
        {
            Advance();

            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        AddToken(TokenType.NumberLiteral, currentLine, currentPosition);
    }

    //
    private void ScanIdentifier()
    {
        while(IsAlphanumeric(Peek())) Advance();

        AddToken(TokenType.Identifier, currentLine, currentPosition);
    }
    // Método para verificar si un carácter es alfabético.
    private bool IsAlpha(char c)
    {
        return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
    }

    // Método para verificar si un carácter es alfanumérico.
    private bool IsAlphanumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

}