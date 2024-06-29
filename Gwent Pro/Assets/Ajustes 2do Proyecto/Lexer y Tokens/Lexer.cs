using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class Lexer
{
    // Variable para almacenar el texto fuente que se va a analizar
    private string sourceText;
    //Lista para almacenar todos los tokens identificados durante el analisis
    private List<Token> tokenList = new List<Token>();
    // Variable para almacenar el indice de la posicion actual en el texto fuente
    private int currentPosition = 0;
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
        InitializeReservedWords();
    }

    // Metdo privado para inicializar el diccionario de palabras reservadas.
    private void InitializeReservedWords()
    {
        reservedWords.Add("while", new Token(TokenType.While, "while", 0, 0));

        reservedWords.Add("for", new Token(TokenType.For, "for", 0, 0));

        reservedWords.Add("false", new Token(TokenType.False, "false", 0, 0));

        reservedWords.Add("true", new Token(TokenType.True, "true", 0, 0));

        reservedWords.Add("card", new Token(TokenType.Card, "card", 0, 0));

        reservedWords.Add("effect", new Token(TokenType.Effect, "effect", 0, 0));

        reservedWords.Add("predicate", new Token(TokenType.Predicate, "predicate", 0, 0));

        reservedWords.Add("postaction", new Token(TokenType.PostAction, "postaction", 0, 0));

    }

    //Verifica si se llego al final del texto fuente
    private bool isAtEnd()
    {
        return currentPosition >= sourceText.Length;
    }

    //Avanza la posicion actual (currentPosition) y devuelve el caracter sin avanzar
    private char Advance()
    {
        return sourceText[currentPosition++];
    }

    //Anade un nuevo token a la lista de tokens
    private void AddToken(TokenType type)
    {
        string text = sourceText.Substring(startPosition, currentPosition - startPosition);

        tokenList.Add(new Token(type, text, currentLine, currentPosition));
    }

    //Analisis de Tokens
    private void LexToken()
    {
        char character = Advance();

        switch (character)
        {
            //Casos para operadores y delimitadores.
            case '(':
                AddToken(TokenType.LeftParenthesis);
                break;
            case ')':
                AddToken(TokenType.RightParenthesis);
                break;
            case '{':
                AddToken(TokenType.LeftBrace);
                break;
            case '}':
                AddToken(TokenType.RightBrace);
                break;
            case '[':
                AddToken(TokenType.LeftBracket);
                break;
            case ']':
                AddToken(TokenType.RightBracket);
                break;
            case ',':
                AddToken(TokenType.Comma);
                break;
            case '.':
                AddToken(TokenType.Dot);
                break;
            case ':':
                AddToken(TokenType.Colon);
                break;
            case ';':
                AddToken(TokenType.SemiColon);
                break;
            case '*':
                AddToken(TokenType.Star);
                break;
            case '!':
                AddToken(character == '='? TokenType.BangEqual : TokenType.Bang);
                break;
            case '<':
                AddToken(character == '='? TokenType.LessEqual : TokenType.Less);
                break;
            case '>':
                AddToken(character == '='? TokenType.GreaterEqual : TokenType.Greater);
                break;
            case '|':
                if (IsMatch('|')) AddToken(TokenType.Or);
                break;
            case '&':
                if (IsMatch('&')) AddToken(TokenType.And);
                break;
            case '=':
                if (IsMatch('=')) AddToken(TokenType.DoubleEqual);
                else if (IsMatch('>')) AddToken(TokenType.EqualGreater);
                else AddToken(TokenType.Equal);
                break;
            case '+':
                if (IsMatch('+')) AddToken(TokenType.Increment);
                else AddToken(TokenType.Plus);
                break;
            case '-':
                if (IsMatch('-')) AddToken(TokenType.Decrement);
                else AddToken(TokenType.Minus);
                break;
            case '/':
                if (IsMatch('/'))
                {
                    while(!isAtEnd() && Peek()!= '\n')
                    {
                        Advance();
                    }
                }
                else AddToken(TokenType.Slash);
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
        while (Peek()!= '"' && isAtEnd())
        {
            if (Peek() == '\n') currentLine++;

            Advance();
        }

        if (isAtEnd())
        {
            System.Console.WriteLine("Error");

            return;
        }

        Advance();

        string value = sourceText.Substring(startPosition + 1, currentPosition - startPosition - 1);

        tokenList.Add(new Token(TokenType.StringLiteral, value, currentLine, currentPosition));
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

        AddToken(TokenType.NumberLiteral);
    }

    //
    private void ScanIdentifier()
    {
        while(IsAlphanumeric(Peek())) Advance();

        AddToken(TokenType.Identifier);
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