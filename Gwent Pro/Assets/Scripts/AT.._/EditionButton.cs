using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditionButtom : MonoBehaviour
{
    private string ReadInput ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadStringInput (string Input)
    {
        ReadInput = Input;
        
        if (ReadInput != null)
        {
            Debug.Log(ReadInput);

            int i = 0;

            Lexer lexer = new Lexer(ReadInput);

            while(true)
            {
                if(lexer.tokenList[i].Type == TokenType.EndOfFile)
                {
                    break;
                }
                

                Debug.Log(lexer.tokenList[i].Lexeme);

                i++;

                lexer.LexToken();
            }

            Debug.Log(lexer.tokenList.Count);

            foreach ( Token token in lexer.tokenList )
            {
                Debug.Log(token.Lexeme);

                Debug.Log(token.Type);
            }

           /*Parser parser = new Parser(lexer);

           parser.Parse();*/
        }

        else
        {
            Debug.Log(" error 50");

            Debug.Log(ReadInput);
        }
        //Parser parser= new Parser(lexer);
    }
}
