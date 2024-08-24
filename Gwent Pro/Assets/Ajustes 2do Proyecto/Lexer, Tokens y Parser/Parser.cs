using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parser
{
    public Lexer lexer;
    public List<Token> TokensList;
    public int NumberOfToken;
    public string CurrentError;

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;

        TokensList = lexer.tokenList;

        NumberOfToken = 0;

        CurrentError = "";
    }

    public void DebugError(Token CurrentToken)
    {
        CurrentError = "Invalid syntax \n";

        CurrentError += "Current token type: " + CurrentToken.Type + "\n";

        CurrentError += "Current token value: " + CurrentToken.Lexeme + "\n";

        CurrentError += "Code:\n";

        CurrentError += (lexer.sourceText.Substring(0, lexer.currentPosition)) + "\n";

        CurrentError += "                             ERROR                             \n";

        CurrentError += (lexer.sourceText.Substring(lexer.currentPosition)) + "\n";

        Debug.Log(CurrentError);
    }







}
