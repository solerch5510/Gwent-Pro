using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Clase para representar operadores binarios en el AST
public class BinaryOperators : AST
{
    //Referencia al operando izquierdo
    public AST Left;
    //Referencia al operador
    public AST Right;
    //Referencia al operando derecho
    public Token Operator;

    //Constructor que inicializa los operandos y el operador
    public BinaryOperators(AST Left, Token Operator, AST Right)
    {
        this.Left = Left;
        
        this.Operator = Operator;

        this.Right = Right;
    }

    // Metodo para expresar la estructura del operador binario en el AST
    public override void Express(string Height)
    {
        // Imprime el nivel actual en la jerarquía del AST
        Debug.Log(Height + "-> Binary Operator:");
        
        // Si el operando izquierdo no es nulo, lo expresa
        if (Left != null)
        {
            Left.Express(Height + "\t -> Left Operand: \t");
        }

        // Imprime el tipo y valor del operador
        Debug.Log(Height + "-> Operator:" + Operator.ToString() + "(" + Operator.Lexeme + ")");
        
        // Si el operando derecho no es nulo, lo expresa
        if (Right != null)
        {
            Right.Express(Height + "\t -> Right Operand: \t");
        }
    }
}

// Clase para representar operadores unarios en el AST
public class UnaryOperators: AST
{
    // Referencia a la operacion
    public Token Operation;
    
    // Referencia a la expresion sobre la cual se aplica la operacion
    public AST Expression;

    // Constructor que inicializa la operacion y la expresion
    public UnaryOperators(Token Operation, AST Expression)
    {
        this.Operation = Operation;

        this.Expression = Expression;
    }

    // Metodo para expresar la estructura del operador unario en el AST
    public override void Express(string Height)
    {
        // Imprime el nivel actual en la jerarquia del AST
        Debug.Log(Height + "-> Unary Operator:");

        // Imprime el tipo y valor de la operacion
        Debug.Log(Height + "-> Operator" + Operation.ToString() + "(" + Operation.Lexeme + ")");
        
        // Si la expresion no es nula, la expresa
        if(Expression != null)
        {
            Expression.Express(Height + "-> Expression: \t");
        }
    }
}