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


            Lexer lexer = new Lexer(ReadInput);

           /* foreach ( Token token in lexer.tokenList )
            {
                Debug.Log(token.Lexeme);

                Debug.Log(token.Type);
            }*/

            Debug.Log(lexer.tokenList.Count);

           Parser parser = new Parser(lexer);

           parser.Parse();
        }

        else
        {
            Debug.Log(" error 50");

            Debug.Log(ReadInput);
        }
    }
}
