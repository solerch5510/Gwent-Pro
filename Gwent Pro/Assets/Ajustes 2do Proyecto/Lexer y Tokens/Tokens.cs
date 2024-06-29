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
    Identifier, StringLiteral, NumberLiteral,

    // Palabras Clave
    And, Else, False, True, For, If, Or, Context, Targets, While, Card, Predicate, Effect, PostAction,

    // Tokens de un solo caracter
    Colon, SemiColon, Slash, Star, Comma, LeftParenthesis, RightParenthesis, LeftBrace, RightBrace,
    LeftBracket,RightBracket, Dot,

    // Tokens de dos caracteres
    Bang, BangEqual, Equal, DoubleEqual, Greater, GreaterEqual, Less, LessEqual, Plus, Minus, Increment, 
    Decrement,  PlusEqual, MinusEqual, EqualGreater,

    // Indicador de fin de archivo.
    EndOfFile
}