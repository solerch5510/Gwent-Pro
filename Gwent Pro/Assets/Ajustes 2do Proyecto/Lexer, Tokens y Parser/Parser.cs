using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parser
{
    public Lexer lexer;
    public Token ThisToken;

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
    }

}
