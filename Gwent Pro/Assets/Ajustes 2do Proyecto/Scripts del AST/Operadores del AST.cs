using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Clase para representar operadores binarios en el AST
public class BinaryOperators : ASTType
{
    //Referencia al operando izquierdo
    public ASTType Left;
    //Referencia al operador
    public ASTType Right;
    //Referencia al operando derecho
    public Token Operator;

    //Constructor que inicializa los operandos y el operador
    public BinaryOperators(ASTType Left, Token Operator, ASTType Right)
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

    public void FindType()
    {
        if(Operator.Type == TokenType.Plus || Operator.Type == TokenType.Minus || Operator.Type == TokenType.Plus1 || Operator.Type == TokenType.Multiply || Operator.Type == TokenType.Divide || Operator.Type == TokenType.Decrement || Operator.Type == TokenType.Mod)
        {
            type = Type.Int;
        }

        if(Operator.Type == TokenType.And || Operator.Type == TokenType.Or || Operator.Type == TokenType.Greater || Operator.Type == TokenType.Less || Operator.Type == TokenType.LessEqual || Operator.Type == TokenType.GreaterEqual || Operator.Type == TokenType.BangEqual || Operator.Type == TokenType.Equal)
        {
            type = Type.Bool;
        }

        if(Operator.Type == TokenType.String_Sum || Operator.Type == TokenType.String_Sum_S) 
        {
            type = Type.String;
        }
    }
}

// Clase para representar operadores unarios en el AST
public class UnaryOperators: ASTType
{
    // Referencia a la operacion
    public Token Operation;
    
    // Referencia a la expresion sobre la cual se aplica la operacion
    public ASTType Expression;

    // Constructor que inicializa la operacion y la expresion
    public UnaryOperators(Token Operation, ASTType Expression)
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

    public void FindType()
    {
        if (Operation.Type == TokenType.Not)
        {
            type = Type.Bool;
        }

        else
        {
            type = Type.Int;
        }

    }
}