using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditionButtom : MonoBehaviour
{
    public GameObject TextField;

    bool IsActive = false;
    private string ReadInput ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivationField()
    {
        if(IsActive == false)
        {
            TextField.SetActive(true);
            
            IsActive = true;
        }

        else if(IsActive == true)
        {
            TextField.SetActive(false);
            IsActive = false;
        }
        

    }

    public void ReadStringInput (string Input)
    {
        ReadInput = Input;
        
        if (ReadInput != null)
        {
            Debug.Log(ReadInput);


            Lexer lexer = new Lexer(ReadInput);

           /*sforeach ( Token token in lexer.tokenList )
            {
                Debug.Log(token.Lexeme);

                Debug.Log(token.Type);
            }*/

            Debug.Log(lexer.tokenList.Count);

           Parser parser = new Parser(lexer);

           Interpreter interpreter = new Interpreter(parser);
        }

        else
        {
            Debug.Log(" error 50");

            Debug.Log(ReadInput);
        }
    }
}
