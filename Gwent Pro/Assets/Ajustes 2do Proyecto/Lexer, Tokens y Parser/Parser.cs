using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Parser
{
    public Lexer lexer;
    public List<Token> TokensList;
    public int NumberOfToken;
    public string CurrentError;
    private Token CurrentToken;
    public Scope <ASTType.Type> GlobalScope;
    public Dictionary<string, EffectNode> EffectList;
    public CardNode ThisCardNode;

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;

        TokensList = lexer.tokenList;

        NumberOfToken = 0;

        CurrentError = "";

        CurrentToken = lexer.tokenList[0];

        GlobalScope = new Scope<ASTType.Type>();

        EffectList = new Dictionary<string, EffectNode>();
    }

    public void ErrorRepeat()
    {
        // Se llama cuando se encuentra una repeticion invalida de un campo en el arbol de sintaxis abstracto
        
        // Imprime un mensaje de error indicando que se ha encontrado una repeticion invalida
    
        DebugError($"Found invalid repetition of field {CurrentToken.Type} in current context");
    }

    public void ErrorNotCorrespondingField()
    {
        // Se llama cuando se encuentra un token que no corresponde al contexto actual
        
        // Imprime un mensaje de error y consume el token actual
        
        DebugError($"Invalid syntax in current context of {CurrentToken.Type.ToString()} token");

        Eat(CurrentToken.Type);
    }

    public void ErrorInNodeCreation(AST node)
    {
        // Se llama cuando hay un problema durante la creacion de un nodo en el AST
        
        // Imprime un mensaje de error indicando que puede haber faltantes campos en la construccion del nodo.
        
        DebugError($"Invalid construction of {node.GetType().ToString()} maybe you miss fields of {node.GetType().ToString()}");
    }

    public void ErrorInUnaryOp(UnaryOperators node)
    {

        // Se llama cuando hay un problema con un operador unario
        
        // Imprime un mensaje de error indicando que no se puede aplicar el operador a los operandos dados
        
        DebugError($"Unvalid use of Unary Operator ({node.Operation.Lexeme}) in {node.Expression.type}  cannot convert {node.Expression.type} to {node.type}");
    }

    public void ErrorInBinOp(BinaryOperators node)
    {
        // Se llama cuando hay un problema con un operador binario
        
        // Imprime un mensaje de error indicando que no se puede aplicar el operador a los operandos dados
        
        DebugError($"Invalid Binary Operator: Operator '{node.Operator.Lexeme}' cannot be applied to operands of type '{node.Left.type}' and '{node.Right.type}'");
    }

    public void ErrorHasNotBeenDeclared(Var variable)
    {
        // Se llama cuando se intenta usar una variable que no ha sido declarada
        
        // Imprime un mensaje de error indicando que la variable no ha sido declarada
        
        DebugError($"Variable '{variable.value}' has not been declared");
    }

    public void ErrorInAssignment(Assign assign)
    {
        // Se llama cuando hay un problema con una asignacion
        
        // Imprime un mensaje de error indicando que no se puede convertir el tipo de la izquierda a la derecha

        DebugError($"Invalid Assignment, cannot convert {assign.left.type}  to  {assign.right.type}");
    }

    public void DebugError(string errorTag)
    {

        // Imprime un mensaje de error detallado en la consola de depuracion

        // Muestra informacion sobre el tipo y valor actual del token, asi como el contexto del codigo.

        CurrentError = "Invalid syntax \n";

        CurrentError += "Current token type: " + CurrentToken.Type + "\n";

        CurrentError += "Current token value: " + CurrentToken.Lexeme + "\n";

        CurrentError += "Code:\n";

        CurrentError += (lexer.sourceText.Substring(0, lexer.currentPosition)) + "\n";

        CurrentError += "                             ERROR                             \n";

        CurrentError += (lexer.sourceText.Substring(lexer.currentPosition)) + "\n";

        Debug.Log(CurrentError);
    }

    // Metodo para obtener el siguiente token
    public void GetToken()
    {
        if(NumberOfToken < TokensList.Count)
        {
            NumberOfToken ++;

            CurrentToken = TokensList[NumberOfToken - 1];
        }
    }
    
    // Metodo para consumir un token especifico del tipo dado
    public void Eat(TokenType tokenType)
    {
        try
        {
            if (CurrentToken.Type == tokenType)
            {
                // Si el token actual coincide con el tipo esperado, avanza al siguiente token

                GetToken();
            }
            else
            {
                // Si no coincide, genera un error de sintaxis

                CurrentError = "Invalid syntax: ";

                CurrentError += "Current token type: " + CurrentToken.Type + ". ";

                CurrentError += "Expected token type: " + tokenType + ". ";

                CurrentError += "Code:\n";

                CurrentError += (lexer.sourceText.Substring(0, lexer.currentPosition)) + "\n";

                CurrentError += "                             ERROR                             \n";

                CurrentError += (lexer.sourceText.Substring(lexer.currentPosition)) + "\n";

                Debug.Log(CurrentError);
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    // Metodo para parsear un factor en la expresion
    public ASTType Factor(Scope<ASTType.Type> scope)
    {
        try
        {
            Token token = CurrentToken;

            // Manejo de operadores unarios
            if (token.Type == TokenType.Plus || token.Type == TokenType.Minus)
            {
                Eat(token.Type);

                ASTType factor = Factor(scope);

                UnaryOperators node = new UnaryOperators(token, factor);

                if(!IsPossibleUnaryOp(node))
                {
                    ErrorInUnaryOp(node);
                }

                return node;
            }

            // Manejo de literales booleanos
            if (token.Type == TokenType.Bool)
            {
                Bool node = new Bool(token);

                Eat(TokenType.Bool);

                return node;
            }

            // Manejo de literales numericos
            if (token.Type == TokenType.NumberLiteral)
            {
                Eat(TokenType.NumberLiteral);

                Int node = new Int(token);

                return node;
            }

            // Manejo de literales de cadena
            if (token.Type == TokenType.StringLiteral)
            {
                Eat(TokenType.StringLiteral);

                String node = new String(token);

                return node;
            }

            // Manejo de parentesis
            if (token.Type == TokenType.LeftParenthesis)
            {
                Eat(TokenType.LeftParenthesis);

                ASTType result = Expression(scope);

                Eat(TokenType.RightParenthesis);

                return result;
            }

            // Manejo de identificadores
            if (CurrentToken.Type == TokenType.Identifier)
            {
                Var node = Variable(scope);

                if(node.GetType() == typeof(Var) && !scope.IsInScope(node))
                {
                    ErrorHasNotBeenDeclared(node);
                }

                token = CurrentToken;

                if(token.Type == TokenType.Plus1 || token.Type == TokenType.Decrement)
                {
                    UnaryOperators unaryNode = new UnaryOperators(token, node);

                    if(node.type != ASTType.Type.Int)
                    {
                        ErrorInUnaryOp(unaryNode);
                    }

                    Eat(token.Type);

                    return node;
                }
            }

            // Manejo de funciones
            if (CurrentToken.Type == TokenType.Function)
            {
                return FunctionStatement(CurrentToken.Lexeme, scope);
            }

            // Manejo de errores
            DebugError($"Invalid Factor: {token.Lexeme}");

            return new NoOp();
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    // Metodo para parsear un termino en la expresion
    public ASTType Term(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = Factor(scope);

            Token token = CurrentToken;

            // Manejo de operadores de multiplicacion, division y modulo
            if (token.Type == TokenType.Multiply || token.Type == TokenType.Divide || token.Type == TokenType.Mod)
            {
                Eat(token.Type);

                node = new BinaryOperators(node, token, Term(scope));

                if(!IsPossibleBinOp(node as BinaryOperators))
                {
                    ErrorInBinOp(node as BinaryOperators);
                }
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    // Metodo para parsear una expresion
    public ASTType Expression(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = Term(scope);

            Token token = CurrentToken;

            // Manejo de operadores de suma, resta, concatenacion de strings
            if (token.Type == TokenType.Plus || token.Type == TokenType.Minus || token.Type == TokenType.String_Sum || token.Type == TokenType.String_Sum_S)
            {
                Eat(token.Type);

                node = new BinaryOperators(node, token, Expression(scope));

                if(!IsPossibleBinOp(node as BinaryOperators))
                {
                    ErrorInBinOp(node as BinaryOperators);
                }
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }


    // Metodo para parsear un factor booleano
    public ASTType BooleanFactor(Scope<ASTType.Type> scope)
    {
        try
        {
            Token token = CurrentToken;

            // Manejo del operador NOT
            if (token.Type == TokenType.Not)
            {
                Eat(TokenType.Not);

                Eat(TokenType.LeftParenthesis);

                UnaryOperators unaryOp = new UnaryOperators(token, BooleanExpression(scope));

                if (!IsPossibleUnaryOp (unaryOp)) 
                {
                    ErrorInUnaryOp(unaryOp);
                }
                
                Eat(TokenType.RightParenthesis);

                return unaryOp;
            }

            // Manejo de parentesis en expresiones booleanas
            if (token.Type == TokenType.LeftParenthesis)
            {
                Eat(TokenType.LeftParenthesis);

                ASTType result = BooleanExpression(scope);

                Eat(TokenType.RightParenthesis);

                return result;
            }

            // Manejo de comparaciones
            ASTType left = Expression(scope);

            token = CurrentToken;

            if (token.Type == TokenType.Equal) 
            {
                Eat(TokenType.Equal);
            }

            else if (token.Type == TokenType.BangEqual) 
            {
                Eat(TokenType.BangEqual);
            }

            else if (token.Type == TokenType.GreaterEqual)
            {
                Eat(TokenType.GreaterEqual);
            }
            
            else if (token.Type == TokenType.LessEqual) 
            {
                Eat(TokenType.LessEqual);
            }

            else if (token.Type == TokenType.Less) 
            {
                Eat(TokenType.Less);
            }

            else if (token.Type == TokenType.Greater) 
            {
                Eat(TokenType.Greater);
            }

            else if (token.Type == TokenType.RightParenthesis || token.Type == TokenType.And || token.Type == TokenType.Or) 
            {
                return left;
            }
            
            else DebugError($"Invalid Boolean operator: '{token.Lexeme}'");

            ASTType right = Expression(scope);

            BinaryOperators node = new BinaryOperators(left, token, right);

            if (!IsPossibleBinOp (node as BinaryOperators))
            {
                ErrorInBinOp(node as BinaryOperators);
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    // Metodo para parsear un termino booleano
    public ASTType BooleanTerm(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = BooleanFactor(scope);

            Token token = CurrentToken;

            // Manejo del operador OR
            if (token.Type == TokenType.Or)
            {
                Eat(TokenType.Or);

                node = new BinaryOperators(node, token, BooleanTerm(scope));

                if( !IsPossibleBinOp (node as BinaryOperators))
                {
                    ErrorInBinOp(node as BinaryOperators);
                }
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    // Metodo para parsear una expresion booleana
    public ASTType BooleanExpression(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = BooleanTerm(scope);

            Token token = CurrentToken;

            // Manejo del operador AND
            if (token.Type == TokenType.And)
            {
                Eat(TokenType.And);

                node = new BinaryOperators(node, token, BooleanExpression(scope));

                if( !IsPossibleBinOp (node as BinaryOperators))
                {
                    ErrorInBinOp(node as BinaryOperators);
                }
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Var Variable(Scope<ASTType.Type> scope)
    {
        try
        {
            // Inicializar una nueva variable con el token actual
            Var node = new Var(CurrentToken);

            // Consumir el token de identificador
            Eat(TokenType.Identifier);

            // Crear una variable compuesta para almacenar los argumentos
            VarComp otherNode = new VarComp(node.token);

            // Manejar indices y llamadas a funciones
            if (CurrentToken.Type == TokenType.Dot || CurrentToken.Type == TokenType.LeftBrace)
            {
                if(CurrentToken.Type == TokenType.LeftBrace)
                {
                    // Parsear un indice
                    Indexer indexer = IndexerParse(scope);

                    otherNode.args.Add(indexer);
                }
                
                // Manejar propiedades y metodos
                while (CurrentToken.Type == TokenType.Dot && CurrentToken.Type != TokenType.EndOfFile)
                {
                    Eat(TokenType.Dot);

                    // Manejar llamadas a funciones
                    if (CurrentToken.Type == TokenType.Function)
                    {
                        Function function = FunctionStatement(CurrentToken.Lexeme, scope);

                        otherNode.args.Add(function);

                        // Manejar indices despues de una funcion
                        if(CurrentToken.Type == TokenType.LeftBrace)
                        {
                            otherNode.args.Add(IndexerParse(scope));
                        }
                    }

                    // Manejar otros tipos de propiedades
                    else
                    {
                        Token token = CurrentToken;

                        if (token.Type == TokenType.Type || token.Type == TokenType.Name || token.Type == TokenType.Faction || token.Type == TokenType.Power || token.Type == TokenType.Range)
                        {
                            Eat(token.Type);

                            // Crear una variable para el valor de la propiedad
                            Var variable = new Var(token, ASTType.Type.String);

                            if(token.Type == TokenType.Power) 
                            {
                                variable.type = ASTType.Type.Int;
                            }

                            otherNode.args.Add(variable);
                        }

                        // Manejar punteros
                        else if (token.Type == TokenType.Pointer)
                        {
                            Eat(token.Type);

                            Pointer pointer = new Pointer(token);

                            otherNode.args.Add(pointer);

                            if(CurrentToken.Type == TokenType.LeftBrace)
                            {
                                otherNode.args.Add(IndexerParse(scope));
                            }
                        }

                        // Manejar errores
                        else
                        {
                            DebugError($"Invalid Field: '{CurrentToken.Lexeme}'");

                            Eat(CurrentToken.Type);
                        }
                    }
                }

                // Asignar la variable compuesta al nodo principal
                node = otherNode;
            }

            // Determinar el tipo de la variable si esta dentro del alcance
            if (node.GetType() == typeof(Var))
            {
                if(scope.IsInScope(node)) 
                {
                    node.type = scope.Get(node);
                }
            }

            // Manejar variables no declaradas
            else if(!scope.IsInScope(node))
            {
                ErrorHasNotBeenDeclared(node);
            }

            // Manejar variables compuestas
            else
            {
                VarComp varComp = node as VarComp;

                if(IsPossibleVarComp(varComp, scope))
                {
                    ASTType.Type lastType = varComp.args[varComp.args.Count-1].type;

                    if (lastType == ASTType.Type.Indexer)
                    {
                        varComp.type = ASTType.Type.Card;
                    }

                    else 
                    {
                        varComp.type = lastType;
                    }
                }
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Assign AssignmentStatement(Var variable, Scope<ASTType.Type> scope)
    {
        try
        {
            // Obtener la variable izquierda de la asignacion
            Var left = variable;

            // Obtener el token actual (operador de asignacion)
            Token token = CurrentToken;

            // Consumir el token de asignacion
            Eat(TokenType.Assign);

            // Parsear la expresion de la derecha
            ASTType right = Expression(scope);

            // Crear un nuevo nodo de asignacion
            Assign node = new Assign(left, token, right);

            // Verificar si es una asignacion compuesta
            if(token.Lexeme != "=")
            {
                // Verificar si la variable esta declarada
                if (!scope.IsInScope(variable)) 
                {
                    ErrorHasNotBeenDeclared(variable);
                }

                // Verificar compatibilidad de tipos para operaciones aritmeticas
                else if ((token.Lexeme == "+=" || token.Lexeme == "-=" || token.Lexeme == "*=" || token.Lexeme == "/=" || token.Lexeme == "%=")  && (variable.type == node.right.type && variable.type != ASTType.Type.Int))
                {
                    ErrorInAssignment(node);
                }

                // Verificar compatibilidad de tipos para concatenacion de strings
                else if ((token.Lexeme == "@=") && (variable.type == node.right.type && variable.type != ASTType.Type.String))
                {
                    ErrorInAssignment(node);
                }
            }

            // Manejar el tipo de la variable izquierda
            if (variable.GetType() == typeof(Var))
            {
                // Si la variable no esta en el alcance, establecer su tipo y agregarla al alcance
                if (!scope.IsInScope(variable))
                {
                    variable.type = node.right.type;

                    scope.Set(variable, variable.type);
                }

                // Si la variable ya existe pero los tipos no coinciden, generar un error
                else if (variable.type != node.right.type) 
                {
                    ErrorInAssignment(node);
                }
            }
            
            // Manejo similar para variables compuestas
            else
            {
                if (!scope.IsInScope(variable)) 
                {
                    ErrorHasNotBeenDeclared(variable);
                }

                else if (variable.type != node.right.type) 
                {
                    ErrorInAssignment(node);
                }
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Function FunctionStatement(string name, Scope<ASTType.Type> scope )
    {
        try
        {
            // Consumir el token de funcion
            Eat(TokenType.Function);

            // Consumir el parentesis izquierdo
            Eat(TokenType.LeftParenthesis);

            // Manejo de la funcion Find
            if (name == "Find") 
            {
                return FindFunction(scope);
            }

            // Manejo de funciones relacionadas con jugadores
            if (name == "HandOfPlayer" || name == "FieldOfPlayer" || name == "DeckOfPlayer" || name == "GraveyardOfPlayer")
            {
                return GetPlayerFunction(name, scope);
            }

            // Manejo de funciones Pop y Shuffle
            if (name == "Pop" || name == "Shuffle") 
            {
                return NoParametersFunction(name);
            }

            // Manejo de funciones Push, Remove, Add y SendBottom
            if (name == "Push" || name == "Remove" || name == "Add" || name == "SendBottom") 
            {
                return CardParameterFunction(name, scope);
            }
            
            // Si ninguna condicion anterior se cumple, devuelve una funcion nula
            return new Function("NULL_FUNCTION");
            
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public ForLoop FLStatement(Scope<ASTType.Type> scope)
    {
        try
        {
            // Consumir el token de 'for'
            Eat(TokenType.For);

            // Obtener la variable objetivo del bucle
            Var target = Variable(scope);
            
            // Establecer el tipo de la variable objetivo como Card
            target.type = ASTType.Type.Card;

            // Verificar si la variable ya esta en el alcance o es una variable compuesta
            if (scope.IsInScope(target) || target.GetType() == typeof(VarComp)) 
            {
                // Generar un error si la variable ya esta declarada
                ErrorUnvalidAssignment(target);
            }

            else 
            {
                // Agregar la variable al alcance si no esta declarada
                scope.Set(target,target.type);
            }

            // Consumir el token 'in'
            Eat(TokenType.In);

            // Obtener la variable objetivo del iterador
            Var targets = Variable(scope);

            // Verificar si la variable del iterador es un campo valido
            if (!scope.IsInScope(targets) || targets.type != ASTType.Type.Field) 
            {
                // Generar un error si no es un campo valido
                ErrorUnvalidAssignment(targets);
            }

            // Parsear el cuerpo del bucle
            Compound body = CompoundStatement(scope);

            // Crear un nuevo nodo de bucle for
            ForLoop node = new ForLoop(target, targets, body);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public WhileLoop WLStatement(Scope<ASTType.Type> scope)
    {
        try
        {
            // Consumir el token de 'while'
            Eat(TokenType.While);

            // Consumir el parentesis izquierdo
            Eat(TokenType.LeftParenthesis);

            // Parsear la condicion del bucle
            AST condition = BooleanExpression(scope);

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            // Parsear el cuerpo del bucle
            Compound body = CompoundStatement(scope);

            // Crear un nuevo nodo de bucle while
            WhileLoop node = new WhileLoop(condition, body);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public IfNode IfNodeStatement(Scope<ASTType.Type> scope)
    {
        try
        {
            // Consumir el token de 'if'
            Eat(TokenType.If);

            // Consumir el parentesis izquierdo
            Eat(TokenType.LeftParenthesis);

            // Parsear la condicion del if
            AST condition = BooleanExpression(scope);

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            // Parsear el cuerpo del if
            Compound body = CompoundStatement(scope);

            // Crear un nuevo nodo de if
            IfNode node = new IfNode(condition, body);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Statement(Scope<ASTType.Type> scope)
    {
        try
        {
            // Manejar diferentes tipos de declaraciones
            if (CurrentToken.Type == TokenType.While)
            {
                // Declaracion de bucle while
                return WLStatement(scope);
            }

            if (CurrentToken.Type == TokenType.If)
            {
                // Sentencia condicional if
                return IfNodeStatement(scope);
            }

            if (CurrentToken.Type == TokenType.For)
            {
                // Bucle for
                return FLStatement(scope);
            }

            if (CurrentToken.Type == TokenType.SemiColon)
            {
                // Declaracion vacia (solo punto y coma)
                return new NoOp();
            }

            if (CurrentToken.Type == TokenType.Identifier)
            {
                // Declaracion de variable o llamada a funcion
                Var variable = Variable(scope);

                // Manejar diferentes casos de uso de la variable
                if (variable.GetType() == typeof(VarComp) && CurrentToken.Type == TokenType.SemiColon)
                {
                    // Variable compuesta seguida de punto y coma
                    VarComp varComp = variable as VarComp;

                    int count = varComp.args.Count - 1;

                    // Verificar si el ultimo argumento es una funcion
                    if (varComp.args[count].GetType() == typeof(Function))
                    {
                        Function f = varComp.args[count] as Function;

                        if (f.type != ASTType.Type.Void) ErrorInvalidStatement();
                    }

                    else 
                    {
                        ErrorInvalidStatement();
                    }

                    return variable;
                }

                // Asignacion
                else if (CurrentToken.Type == TokenType.Assign)
                {
                    Assign node = AssignmentStatement(variable, scope);

                    return node;
                }

                // Operaciones unarias (incremento/decremento)
                else if (CurrentToken.Type == TokenType.Decrement || CurrentToken.Type == TokenType.Plus1)
                {
                    UnaryOperators plusnode = new UnaryOperators(CurrentToken, variable);

                    // Verificar si la variable es de tipo entero
                    if (variable.type != ASTType.Type.Int) 
                    {
                        ErrorInUnaryOp(plusnode);
                    }

                    Assign node = new Assign(variable, CurrentToken, plusnode);

                    Eat(CurrentToken.Type);

                    return node;
                }

                // Caso por defecto: error de sintaxis invalida
                DebugError($"Invalid Statement: token {CurrentToken.Type}");

                return new NoOp();
            }

            // Caso por defecto: error de sintaxis invalida
            DebugError($"Invalid Statement: token {CurrentToken.Type}");

            Eat(CurrentToken.Type);

            return new NoOp();
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public List<AST> StatementList(Scope<ASTType.Type> scope)
    {
        try
        {
            // Lista para almacenar las declaraciones
            List<AST> results = new List<AST>();

            // Bucle hasta encontrar el final del bloque o el fin del archivo
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Parsear una declaracion
                AST node = Statement(scope);

                // Agregar la declaracion a la lista
                results.Add(node);

                // Consumir el punto y coma
                Eat(TokenType.SemiColon);
            }

            return results;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Compound CompoundStatement(Scope<ASTType.Type> outScope)
    {
        try
        {
            // Crear un nuevo alcance para el bloque
            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(outScope);

            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);

            // Parsear la lista de declaraciones
            List<AST> nodes = StatementList(scope);

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            // Crear un nuevo nodo compuesto
            Compound root = new Compound();

            // Agregar todas las declaraciones al nodo compuesto
            for (int i = 0; i < nodes.Count; i++)
            {
                root.children.Add(nodes[i]);
            }

            return root;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public TypeNode TypeParse()
    {
        try
        {
            // Consumir el token de tipo
            Eat(TokenType.Type);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Crear un nuevo nodo de tipo
            TypeNode node = new TypeNode(CurrentToken);

            // Verificar si el tipo es valido (Gold o Silver)
            if (node.type != "Gold" && node.type != "Silver")
            {
                // Generar un error si el tipo no es valido
                DebugError("Invalid Type of Card: You may try with 'Gold' or 'Silver'");
            }

            // Consumir el literal de cadena del tipo
            Eat(TokenType.StringLiteral);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Faction FactionParse()
    {
        try
        {
            // Consumir el token de faccion
            Eat(TokenType.Faction);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Crear un nuevo nodo de faccion
            Faction node = new Faction(CurrentToken);

            // Verificar si la faccion es valida (Paladins o Monsters)
            if (node.faction != "Paladins" && node.faction != "Monsters")
            {
                // Generar un error si la facción no es valida
                DebugError("Invalid Faction for Card: You may try with 'Paladins' or 'Monsters'");
            }

            // Consumir el literal de cadena de la faccion
            Eat(TokenType.StringLiteral);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public BasedPower PowerParse()
    {
        try
        {
            // Consumir el token de poder
            Eat(TokenType.Power);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Crear un nuevo nodo de poder base
            BasedPower node = new BasedPower(CurrentToken);

            // Consumir el literal numerico del poder
            Eat(TokenType.NumberLiteral);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Range RangeParse()
    {
        try
        {
            // Consumir el token de rango
            Eat(TokenType.Range);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);
            
            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBrace);

            // Crear un nuevo nodo de rango            
            Range node = new Range(CurrentToken);

            // Verificar si el rango es valido (Melee, Range o Siege)
            if (node.range != "Melee" && node.range != "Range" && node.range != "Siege")
            {
                // Generar un error si el rango no es valido
                DebugError("Invalid Zone for Card: You may try with 'Melee', 'Range' or 'Siege'");
            }

            // Consumir el literal de cadena del rango            
            Eat(TokenType.StringLiteral);

            // Consumir el corchete derecho            
            Eat(TokenType.RightBrace);
            
            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public EffectOnActivation EffectOnActivationParse()
    {
        try
        {
            // Consumir el token de efecto al activarse
            Eat(TokenType.OnActivation_Effect);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);

            // Inicializar nombre y parametros
            ParamName name = null;

            Args parameters = new Args();

            // Bucle para procesar nombres y parametros
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Manejar nombres de efectos
                if (CurrentToken.Type == TokenType.Name)
                {
                    if (name == null)
                    {
                        name = NameParse();

                        // Verificar si el efecto esta definido globalmente
                        if (!GlobalScope.IsInScope(name)) 
                        {
                            ErrorEffectCalling(name);
                        }

                        else
                        {
                            // Verificar si el tipo del efecto es correcto
                            if (GlobalScope.Get(name) != ASTType.Type.Effect)
                            {
                                ErrorEffectCalling(GlobalScope.Get(name), name);
                            }
                                
                        }

                        // Manejar separadores entre nombres
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                // Manejar identificadores (variables o funciones)
                else if (CurrentToken.Type == TokenType.Identifier)
                {
                     EffectNode effect;

                    Var variable = Variable(GlobalScope);

                    if (variable.GetType() == typeof(VarComp) || variable.type != ASTType.Type.Null) 
                    {
                        ErrorUnvalidAssignment(variable);
                    }

                    Token token = CurrentToken;

                    Eat(TokenType.Colon);

                    ASTType value = Expression(GlobalScope);

                    variable.type = value.type;

                    Assign param = new Assign(variable, token, value);

                    if (name == null) 
                    {
                        DebugError("'Name' of effect has not been declared");
                    }

                    else if (!EffectList.ContainsKey(name.paramName)) 
                    {
                        ErrorEffectCalling(name);
                    }

                    else 
                    {
                        effect = EffectList[name.paramName];

                        if (effect.scope.IsInScope(variable))
                        {
                            ASTType.Type typeInEffect = effect.scope.Get(variable);

                            if (typeInEffect != variable.type)
                            {
                                DebugError($"Invalid type of parameter: cannot convert {typeInEffect} to {variable.type}");
                            }
                        }

                        else 
                        {
                            DebugError($"Param '{variable.value}' not found in effect '{name.paramName}'");
                        }
                    }

                    parameters.Add(param);

                    if (CurrentToken.Type != TokenType.RightBracket) 
                    {
                        Eat(TokenType.Comma);
                    }
                }

                // Manejar errores
                else
                { 
                    ErrorNotCorrespondingField(); 
                }
            }

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            // Crear el nodo de efecto al activarse
            EffectOnActivation node;

            if (parameters.args.Count == 0)
            {
                node = new EffectOnActivation(name);
            }
                
            else 
            {
                node = new EffectOnActivation(name, parameters);
            }

            // Verificar si el nombre del efecto esta definido
            if (name == null) 
            {
                ErrorInNodeCreation(node);
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Source ContextParse()
    {
        try
        {
            // Consumir el token de contexto
            Eat(TokenType.Context);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Obtener el token actual
            Token token = CurrentToken;

            // Consumir el literal de cadena del contexto
            Eat(TokenType.StringLiteral);

            // Verificar si el contexto es valido
            if (!IsValidSource(token))
            {
                DebugError($"'{token.Lexeme}' is not a valid source of context");
            }

            // Crear un nuevo nodo de fuente
            Source node = new Source(token);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Single SingleParse()
    {
        try
        {
            // Consumir el token de single
            Eat(TokenType.Single);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Obtener el token actual
            Token token = CurrentToken;

            // Consumir el literal booleano
            Eat(TokenType.Bool);

            // Crear un nuevo nodo de single
            Single node = new Single(token);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Predicate PredicateParse()
    {
        try
        {
            // Consumir el token de predicado
            Eat(TokenType.Predicate);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Consumir el paréntesis izquierdo
            Eat(TokenType.LeftParenthesis);

            // Obtener la variable unidad
            Var unit = Variable(GlobalScope);

            // Establecer el tipo de la variable como Card
            unit.type = ASTType.Type.Card;

            // Verificar si la variable es una variable compuesta
            if (unit.GetType() == typeof(VarComp)) 
            {
                ErrorUnvalidAssignment(unit);
            }

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            // Consumir el simbolo =>
            Eat(TokenType.EqualGreater);

            // Consumir el parentesis izquierdo
            Eat(TokenType.LeftParenthesis);

            // Crear un nuevo alcance para la condicion
            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(GlobalScope);

            // Establecer la variable unidad en el nuevo alcance
            scope.Set(unit, unit.type);

            // Parsear la condicion booleana
            ASTType condition = BooleanExpression(scope);

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            // Crear un nuevo nodo de predicado
            Predicate node = new Predicate(unit, condition);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Selector SelectorParse()
    {
        try
        {
            // Consumir el token de targets
            Eat(TokenType.Targets);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);

            // Inicializar variables para almacenar los resultados
            Source source = null;

            Single single = null;

            Predicate predicate = null;

            // Bucle para procesar diferentes partes del selector
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Manejar el contexto
                if (CurrentToken.Type == TokenType.Context)
                {
                    if (source == null)
                    {
                        source = ContextParse();

                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                // Manejar la unicidad
                else if (CurrentToken.Type == TokenType.Single)
                {
                    if (single == null)
                    {
                        single = SingleParse();

                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                // Manejar el predicado
                else if (CurrentToken.Type == TokenType.Predicate)
                {
                    if (predicate == null)
                    {
                        predicate = PredicateParse();

                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                // Manejar errores
                else 
                { 
                    ErrorNotCorrespondingField(); 
                }
            }

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            // Crear el nodo de selector
            Selector node;

            if (single == null) 
            {
                node = new Selector(source, predicate);
            }

            else 
            {
                node = new Selector(source, single, predicate);
            }

            // Verificar si faltan campos obligatorios
            if (source == null || predicate == null) 
            {
                ErrorInNodeCreation(node);
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public PostAction PostActionParse()
    {
        try
        {
            // Consumir el token de post-action
            Eat(TokenType.PostAction);

            // Consumir el token de dos puntos
            Eat(TokenType.Colon);

            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);

            // Inicializar variables para almacenar los resultados
            EffectOnActivation effectOnActivation = null;

            Selector selector = null;

            // Bucle para procesar diferentes partes de la post-action
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Manejar el efecto al activarse
                if (CurrentToken.Type == TokenType.OnActivation_Effect)
                {
                    if (effectOnActivation == null)
                    {
                        effectOnActivation = EffectOnActivationParse();

                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                // Manejar el selector
                else if (CurrentToken.Type == TokenType.Targets)
                {
                    if (selector == null)
                    {
                        selector = SelectorParse();

                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                // Manejar errores
                else 
                {
                    ErrorNotCorrespondingField();
                }
            }

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            // Crear el nodo de post-action
            PostAction node;

            if (selector == null) 
            {
                node = new PostAction(effectOnActivation);
            }

            else 
            {
                node = new PostAction(effectOnActivation, selector);
            }

            // Verificar si faltan campos obligatorios
            if (effectOnActivation == null) 
            {
                ErrorInNodeCreation(node);
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public OnActivationElement OnActivationElementParse()
    {
        try
        {
            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);
            
            // Inicializar variables para el efecto, selector y accion posterior
            EffectOnActivation effectOnActivation = null;
            
            Selector selector = null;

            PostAction postAction = null;

            // Bucle hasta que se alcance el corchete derecho
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Manejar token OnActivation_Effect
                if (CurrentToken.Type == TokenType.OnActivation_Effect)
                {
                    if (effectOnActivation == null)
                    {
                        // Analizar el efecto
                        effectOnActivation = EffectOnActivationParse();

                        // Verificar si hay otro elemento despues del efecto
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }
                    else 
                    {
                        // Repeticion invalida de OnActivation_Effect
                        ErrorRepeat();
                    }
                }

                // Manejar token Targets
                else if (CurrentToken.Type == TokenType.Targets)
                {
                    if (selector == null)
                    {
                        // Analizar el selector
                        selector = SelectorParse();

                        // Verificar si hay otro elemento despues del selector
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        // Repetición invalida de Targets
                        ErrorRepeat();
                    }
                }

                // Manejar token PostAction
                else if (CurrentToken.Type == TokenType.PostAction)
                {
                    if (postAction == null)
                    {
                        // Analizar la accion posterior
                        postAction = PostActionParse();

                        // Verificar si hay otro elemento despues de la accion posterior
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        // Repeticion invalida de PostAction
                        ErrorRepeat();
                    }
                }

                else 
                {
                    ErrorNotCorrespondingField();
                }
            }

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            // Crear el nodo OnActivationElement basado en los elementos analizados
            OnActivationElement node;

            if (selector == null && postAction == null) 
            {
                node = new OnActivationElement(effectOnActivation);
            }

            else if (postAction == null) 
            {
                node = new OnActivationElement(effectOnActivation, selector);
            }

            else if (selector == null) 
            {
                node = new OnActivationElement(effectOnActivation, postAction);
            }

            else 
            {
                node = new OnActivationElement(effectOnActivation, selector, postAction);
            }

            // Verificar si el efecto es valido
            if (effectOnActivation == null) 
            {
                ErrorInNodeCreation(node);
            }

            // Verificar si el selector es invalido (source es "parent")
            if (selector != null && selector.source.source == "parent") 
            {
                DebugError("Invalid source parent, Effect is not a Post Action node");
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public List<OnActivationElement> OnActivationList()
    {
        try
        {
            // Inicializar una lista vacia para almacenar los nodos
            List<OnActivationElement> nodes = new List<OnActivationElement>();

            // Bucle mientras no se alcance el corchete derecho o el fin del archivo
            while (CurrentToken.Type != TokenType.RightBrace && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Si el token actual es un corchete izquierdo
                if (CurrentToken.Type == TokenType.LeftBracket)
                {
                    // Analizar un elemento de activacion
                    OnActivationElement node = OnActivationElementParse();

                    // Agregar el nodo a la lista
                    nodes.Add(node);

                    // Si no estamos al final de los elementos, consumir una coma
                    if (CurrentToken.Type != TokenType.RightBrace) 
                    {
                        Eat(TokenType.Comma);
                    }
                }

                // Si no es un corchete izquierdo, generar un error
                else 
                {
                    ErrorNotCorrespondingField();
                }
            }

            // Devolver la lista de nodos
            return nodes;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public OnActivation OnActivationParse()
    {
        try
        {
            // Consumir el token OnActivation
            Eat(TokenType.OnActivation);

            // Consumir el signo de dos puntos
            Eat(TokenType.Colon);

            // Consumir el corchete izquierdo            
            Eat(TokenType.LeftBrace);

            // Obtener la lista de elementos de activacion            
            List<OnActivationElement> list = OnActivationList();
            
            // Consumir el corchete derecho
            Eat(TokenType.RightBrace);

            // Crear un nuevo nodo OnActivation
            OnActivation node = new OnActivation();
            
            // Agregar cada elemento de la lista al nodo
            for (int i = 0; i < list.Count; i++)
            {
                node.onActivation.Add(list[i]);
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public CardNode CardCreation()
    {
        try
        {
            // Consumir el token Card
            Eat(TokenType.Card);

            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);

            // Variables para almacenar los datos del card
            ParamName name = null;

            TypeNode type = null;

            Faction faction = null;

            BasedPower power = null;
            
            Range range = null;

            OnActivation onActivation = null;

            // Bucle mientras no se alcance el corchete derecho o el fin del archivo
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                if (CurrentToken.Type == TokenType.Name)
                {
                    // Manejar el token Name
                    if (name == null)
                    {
                        // Analizar el nombre de la carta
                        name = NameParse();

                        // Comprobar si ya existe un miembro definido globalmente con este nombre
                        if (GlobalScope.IsInScope(name)) 
                        {
                            // Error: Miembro ya definido globalmente
                            ErrorAlReadyDefinesMember(name.paramName);
                        }

                        else 
                        {
                            // Definir el miembro local
                            GlobalScope.Set(name, ASTType.Type.Card);
                        }

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }
                    
                    else 
                    {
                        // Repeticion inválida de Name
                        ErrorRepeat();
                    }
                }

                // Manejar el token Type
                else if (CurrentToken.Type == TokenType.Type)
                {
                    if (type == null)
                    {
                        // Analizar el tipo de la carta
                        type = TypeParse();

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        // Repeticion invalida de Type
                        ErrorRepeat();
                    }
                }

                // Manejar el token Faction
                else if (CurrentToken.Type == TokenType.Faction)
                {
                    if (faction == null)
                    {
                        // Analizar la facción de la carta
                        faction = FactionParse();

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        // Repeticion inválida de Faction
                        ErrorRepeat();
                    }
                }

                // Manejar el token Power
                else if (CurrentToken.Type == TokenType.Power)
                {
                    if (power == null)
                    {
                        // Analizar el poder de la carta
                        power = PowerParse();

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }
                    else 
                    {
                        // Repeticion invalida de Power
                        ErrorRepeat();
                    }
                }

                // Manejar el token Range
                else if (CurrentToken.Type == TokenType.Range)
                {
                    if (range == null)
                    {
                        // Analizar el rango de la carta
                        range = RangeParse();

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            //Repeticion invalida de Range
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                // Manejar el token OnActivation
                else if (CurrentToken.Type == TokenType.OnActivation)
                {
                    if (onActivation == null)
                    {
                        // Analizar las acciones de activacion de la carta
                        onActivation = OnActivationParse();

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }
                    
                    else 
                    {
                        // Repeticion invalida de OnActivation
                        ErrorRepeat();
                    }
                }

                else 
                { 
                    ErrorNotCorrespondingField(); 
                }
            }

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            // Crear un nuevo nodo CArdNode con todos los datos analizados
            CardNode node = new CardNode(name, type, faction, power, range, onActivation);

            // Verificar si hay algun elemento nulo en la lista de parametros
            List<AST> listOfParameters = new List<AST> { name, type, faction, power, range, onActivation };
            
            foreach (AST child in listOfParameters)
            {
                if (child == null) 
                {
                    // Error : Elemento nulo en la creacion del nodo
                    ErrorInNodeCreation(node);

                    break;
                }
            }

            // Asignar el nodo creado a la propiedad ThisCardNode y devolverlo
            ThisCardNode = node;

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public ParamName NameParse()
    {
        try
        {
            // Consumir el token Name
            Eat(TokenType.Name);

            // Consumir el signo dos puntos
            Eat(TokenType.Colon);
            
            // Crear un nuevo nodo ParamName con el lexema del token actual
            ParamName node = new ParamName(CurrentToken);
            
            // Verificar si el nombre del parametro está vacio
            if (node.paramName == "") 
            {
                DebugError("Name must not be an empty string");
            }
            
            // Consumir la cadena literal
            Eat(TokenType.StringLiteral);

            return node;

        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Args GetParametersInParams(Scope<ASTType.Type> scope)
    {
        try
        {
            // Crear una lista vacia para almacenar los parametros
            Args args = new Args();

            // Bucle mientras el token actual no sea corchete derecho o fin del archivo
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Analizar la variaable
                Var variable = Variable(scope);

                // Verificar si la variable ya esta en el arbol de sintaxis abstracto
                if (scope.IsInScope(variable) || variable.GetType() == typeof(VarComp))
                {
                    DebugError("Invalid declaration of param");
                }

                // Consumir el signo de dos puntos
                Eat(TokenType.Colon);

                // Verificar el tipo de variable
                if (CurrentToken.Type == TokenType.Var_Int || CurrentToken.Type == TokenType.Var_String || CurrentToken.Type == TokenType.Var_Bool)
                {
                    // Asignar el tipo al parametro y agregarlo a la lista de parametros
                    variable.TypeInParams(CurrentToken.Type);

                    scope.Set(variable, variable.type);

                    args.Add(variable);

                    // Consumir el token del tipo de variable
                    Eat(CurrentToken.Type);

                    // Si no estamos al final de los parametros, consumir una coma
                    if (CurrentToken.Type != TokenType.RightBracket)
                    {
                        Eat(TokenType.Comma);
                    }
                }

                else
                {
                    DebugError($"Invalid Type '{CurrentToken.Lexeme}' found in current context\n Expecting 'String', 'Bool', 'Number'");

                    Eat(CurrentToken.Type);

                    // Si es un token de coma, consumirlo
                    if (CurrentToken.Type == TokenType.Comma) 
                    {
                        Eat(CurrentToken.Type);
                    }
                }
            }

            // Devolver la lista de parametros
            return args;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Args ParamsEffectParse(Scope<ASTType.Type> scope)
    {
        try
        {
            // Consumir el token Params
            Eat(TokenType.Params);
            
            // Consumir el signo de dos puntos
            Eat(TokenType.Colon);

            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);

            // Obtener los parametros en el metodo GetParametersInParams
            Args node = GetParametersInParams(scope);

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Action ActionParse(Scope<ASTType.Type> outScope)
    {
        try
        {
            // Consumir el token Action
            Eat(TokenType.Action);

            // Consumir el signo de dos puntos
            Eat(TokenType.Colon);

            // Consumir el parentesis izquierdo
            Eat(TokenType.LeftParenthesis);

            // Analizar las variables para los targets y el contexto
            Var targets = Variable(outScope);

            Var context = Variable(outScope);

            // Configurar tipos para targets y contexto
            targets.type = ASTType.Type.Field;

            context.type = ASTType.Type.Context;

            // Verificar si las variables ya estan en el alcance o son de tipo VarComp
            if (outScope.IsInScope(targets) || targets.GetType() == typeof(VarComp))
            {
                // Error: Asignacion invalida
                ErrorUnvalidAssignment(targets);
            }

            else 
            {
                // Agregar targets al alcance
                outScope.Set(targets, targets.type);
            }

            // Consumir coma
            Eat(TokenType.Comma);
      
            if (outScope.IsInScope(context) || context.GetType() == typeof(VarComp))
            {
                // Error: Asignacion invalida
                ErrorUnvalidAssignment(context); 
            }   

            else 
            {
                // Agregar contexto al alcance
                outScope.Set(context, context.type);
            }

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            // Consumir el simbolo =>
            Eat(TokenType.EqualGreater);

            // Analizar el cuerpo de la accion
            Compound body = CompoundStatement(outScope);

            // Crear el nodo Action con los targets, contexto y cuerpo
            Action node = new Action(targets, context, body);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public EffectNode EffectCreation()
    {
        try
        {
            // Consumir el token Effect
            Eat(TokenType.Effect);

            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBracket);

            // Crear un nuevo alcance para los efectos
            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(GlobalScope);

            // Variables para almacenar los datos del efecto
            ParamName name = null;

            Args parameters = null;

            Action action = null;

            // Bucle para procesar los elementos del efecto
            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                // Manejar el token Name
                if (CurrentToken.Type == TokenType.Name)
                {
                    if (name == null)
                    {
                        name = NameParse();

                        // Verificar si ya existe un miembro definido globalmente con este nombre
                        if (GlobalScope.IsInScope(name)) 
                        {
                            // Error: Miembro ya definido globalmente
                            ErrorAlReadyDefinesMember(name.paramName);
                        }

                        else 
                        {
                            // Definir el miembro local
                            GlobalScope.Set(name, ASTType.Type.Effect);
                        }

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket)
                        {
                            Eat(TokenType.Comma);
                        }    
                    }

                    else 
                    {
                        // Repeticion invalida de Name
                        ErrorRepeat();
                    }
                }

                // Manejar el token Params
                else if (CurrentToken.Type == TokenType.Params)
                {
                    if (parameters == null)
                    {
                        parameters = ParamsEffectParse(scope);

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket)
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        // Repeticion invalida de Params
                        ErrorRepeat();
                    }
                }

                // Manejar el token Action
                else if (CurrentToken.Type == TokenType.Action)
                {
                    if (action == null)
                    {
                        action = ActionParse(scope);

                        // Si no estamos al final de los elementos, consumir una coma
                        if (CurrentToken.Type != TokenType.RightBracket)
                        {
                            Eat(TokenType.Comma);
                        }                            
                    }
                    
                    else 
                    {
                        // Repeticion invalida de Action
                        ErrorRepeat();
                    }
                }

                else 
                { 
                    ErrorNotCorrespondingField();
                }
            }

            // Consumir el corchete derecho
            Eat(TokenType.RightBracket);

            // Crear el nodo EffectNode basado en los elementos analizados
            EffectNode node;

            if (parameters == null)
            {
                node = new EffectNode(name, action);
            }
            else
            {
                node = new EffectNode(name, parameters, action, scope);
            }

            // Verificar si el nombre y la accion son validos
            if (name == null || action == null)
            {
                // Error en la creacion del nodo
                ErrorInNodeCreation(node);
            }

            // Agregar el efecto a la lista de efectos globales
            EffectList[name.paramName] = node;

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public List<AST> ListOfCardAndEffect()
    {
        try
        {
            // Crear una lista vacia para almacenar los nodos de cartas y efectos
            List<AST> listOfCardAndEffect = new List<AST>();

            // Bucle hasta que se alcance el fin del archivo
            while (CurrentToken.Type != TokenType.EndOfFile)
            {
                // Manejar token Card
                if (CurrentToken.Type == TokenType.Card)
                {
                    // Analizar la creacion de una carta
                    CardNode node = CardCreation();

                    // Agregar el nodo de carta a la lista
                    listOfCardAndEffect.Add(node);
                }

                // Manejar token Effect
                else if (CurrentToken.Type == TokenType.Effect)
                {
                    // Analizar la creacion de un efecto
                    EffectNode node = EffectCreation();

                    // Agregar el nodo de efecto a la lista
                    listOfCardAndEffect.Add(node);
                }

                else 
                {
                    DebugError($"Expecting (card) or (effect) token, found: {CurrentToken.Type.ToString()}");

                    // Consumir el token actual
                    Eat(CurrentToken.Type);
                }
            }

            return listOfCardAndEffect;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Compound Program()
    {
        try
        {
            // Obtener la lista de nodos de cartas y efectos
            List<AST> programList = ListOfCardAndEffect();

            // Crear un nuevo nodo Compound para representar el programa
            Compound program = new Compound();

            // Agregar cada nodo de la lista al programa
            foreach (AST node in programList)
            {
                program.children.Add(node);
            }

            return program;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Parse()
    {
        try
        {
            // Analizar el programa completo y obtener el nodo raiz
            Compound node = Program();

            // Verificar si se ha consumido todo el texto de entrada
            if (CurrentToken.Type != TokenType.EndOfFile)
            {
                DebugError("Cannot parse all of text");
            }

            // Ejecutar la expresion vacia en el nodo raiz
            node.Express("");

            return node;
        }

        catch (System.Exception)
        {
            // En caso de error, crear un nodo NoOp como fallback
            AST node = new NoOp();

            return node;
        }
    }
    public bool IsPossibleUnaryOp(UnaryOperators node)
    {
        // Verificar si el tipo del operador unario es igual al tipo de la expresion
        return (node.type == node.Expression.type);
    }

    public bool IsPossibleBinOp(BinaryOperators node)
    {
        // Verificar si los tipos de los operandos izquierdo y derecho son iguales
        if (node.Left.type == node.Right.type)
        {
            // Si el tipo es Bool
            if (node.type == ASTType.Type.Bool)
            {
                // Verificar si los operandos son de tipo Bool y el operador es OR o AND
                if (node.Left.type == ASTType.Type.Bool && (node.Operator.Type == TokenType.Or || node.Operator.Type == TokenType.And))
                {
                    // Operación valida para tipos Bool con OR o AND
                    return true;
                }

                // Verificar si el operador es igualdad o desigualdad
                if (node.Operator.Type == TokenType.Equal || node.Operator.Type == TokenType.BangEqual)
                {
                    // Operacion valida para comparaciones
                    return true;
                }
                
                // Si no es una comparacion, verificar si el tipo izquierdo es Int
                else 
                {
                    return (node.Left.type == ASTType.Type.Int);
                }
            }
            
            // Si no es un tipo Bool, verificar si el tipo izquierdo coincide con el tipo del nodo
            else 
            {
                return (node.Left.type == node.type);
            }
        }
        
        // Si los tipos no coinciden, devolver falso
        return false;
    }

    public bool IsValidIndexer(Indexer node)
    {
        // Verificar si el tipo del indice es Int
        return node.index.type == ASTType.Type.Int;
    }

    public Indexer IndexerParse(Scope<ASTType.Type> scope)
    {
        try
        {
            // Consumir el corchete izquierdo
            Eat(TokenType.LeftBrace);

            // Analizar la expresion para el indice
            ASTType index = Expression(scope);

            // Consumir el corchete derecho
            Eat(TokenType.RightBrace);

            // Crear un nuevo nodo Indexer
            Indexer node = new Indexer(index);

            // Verificar si el indice es valido
            if (!IsValidIndexer(node))
            {
                DebugError("Invalid indexer: Expression must be type 'INT'");
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public bool IsPossibleVarComp(VarComp varComp, Scope<ASTType.Type> scope)
    {
        // Iterar sobre los argumentos de la composicion de variable
        for (int i = 0; i < varComp.args.Count; i++)
        {
            // Para el primer argumento
            if (i == 0)
            {
                // Verificar si es posible la composicion interna
                if (!InternalIsPossibleVarComp(scope.Get(varComp.value), varComp.args[i])) 
                {
                    // Composicion no valida
                    return false;
                }
            }

            // Para los demas argumentos ; verificar si es posible la composicion interna entre el argumento anterior y el actual
            else if (!InternalIsPossibleVarComp(varComp.args[i - 1].type, varComp.args[i])) 
            {
                // Composicion no valida
                return false;
            }
        }

        // Si se han verificado todos los argumentos sin problemas, la composicion es valida
        return true;
    }

    public bool IsFunction(ASTType node)
    {
        // Verificar si el tipo del nodo es Function
        return (node.GetType() == typeof(Function));
    }

    public bool InternalIsPossibleVarComp(ASTType.Type fatherType, ASTType var)
    {
        // Caso para Context
        if (fatherType == ASTType.Type.Context)
        {
            // Si el hijo es un Pointer
            if (var.GetType() == typeof(Pointer))
            {
                Pointer p = var as Pointer;

                // Obtener el valor del puntero
                string s = p.pointer;

                // Verificar si el valor es válido (Hand, Graveyard, Deck, Melee, Range, Siege)
                return (s == "Hand" || s == "Graveyard" || s == "Deck" || s == "Melee" || s == "Range" || s == "Siege");
            }

            // Si el hijo es una funcion
            else if (IsFunction(var)) 
            {
                // Verificar si el tipo de retorno de la funcion es Context o Field
                return (var.type == ASTType.Type.Context || var.type == ASTType.Type.Field);
            }
        }

        // Caso para Field
        if (fatherType == ASTType.Type.Field)
        {
            // Si el hijo es un Indexer
            if (var.GetType() == typeof(Indexer)) 
            {
                // Composicion valida
                return true;
            }

            // Si el hijo es una funcion
            else if (IsFunction(var)) 
            {
                // Verificar si el tipo de retorno de la funcion es Field, Void o Card
                return (var.type == ASTType.Type.Field || var.type == ASTType.Type.Void || var.type == ASTType.Type.Card);
            }
        }

        // Caso para Indexer o Card
        if (fatherType == ASTType.Type.Indexer || fatherType == ASTType.Type.Card)
        {
            // Si el hijo es una Var
            if (var.GetType() == typeof(Var))
            {
                Var otherVar = var as Var;

                // Obtener el valor de la variable
                string s = otherVar.value;

                // Verificar si el valor es válido (Type, Name, Faction, Range, Power)
                return (s == "Type" || s == "Name" || s == "Faction" || s == "Range" || s == "Power");
            }

            // Si no es una Var, la composicion no es valida
            else 
            {
                return false;
            }
        }

        // Caso para Effect
        if (fatherType == ASTType.Type.Effect)
        {
            // Convertir el hijo a Var
            var otherValue = var as Var;

            // Obtener el valor de la variable
            string s = otherValue.value;

            // Verificar si el valor es valido (Name)
            return (s == "Name");
        }

        // Si ninguna condicion se cumple, generar un error
        ErrorInVarCompConstruction(fatherType, var);

        // Devolver falso por defecto
        return false;
    }

    public void ErrorInVarCompConstruction(ASTType.Type fatherType, ASTType var)
    {
        // Verificar si el nodo es de tipo Var
        if (var.GetType() == typeof(Var))
        {
            Var otherValue = var as Var;

            DebugError($"Invalid VarComp construction: '{otherValue.value}' is not a valid field of type '{fatherType.ToString()}'");
        }

        // Si no es de tipo Var, usar el metodo ToString() para el mensaje de error
        else DebugError($"Invalid VarComp construction: '{var.ToString()}' is not a valid field of type '{fatherType.ToString()}'");
    }

    public void ErrorInvalidParameterInFunction(string functionName)
    {
        // Generar un mensaje de error indicando que el parametro es invalido para la funcion dada
        DebugError($"Invalid parameter for Function '{functionName}'");
    }

    public Function FindFunction(Scope<ASTType.Type> outScope)
    {
        try
        {
            // Crear un nuevo alcance para la funcion Find
            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(outScope);

            // Consumir el parentesis izquierdo
            Eat(TokenType.LeftParenthesis);

            // Analizar la variable para los targets
            Var variable = Variable(scope);

            // Verificar si la variable ya está en el alcance o es de tipo VarComp
            if (scope.IsInScope(variable) || variable.GetType() == typeof(VarComp)) 
            {
                // Error: Parametro invalido
                ErrorInvalidParameterInFunction("Find");
            }

            else
            {
                // Establecer el tipo de la variable como Card
                variable.type = ASTType.Type.Card;

                // Agregar la variable al alcance
                scope.Set(variable, variable.type);
            }
            
            // Consumir el paréntesis derecho
            Eat(TokenType.RightParenthesis);
            
            // Consumir el simbolo =>
            Eat(TokenType.EqualGreater);

            // Consumir el parentesis izquierdo nuevamente
            Eat(TokenType.LeftParenthesis);

            // Analizar la condicion booleana
            ASTType condition = BooleanExpression(scope);

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            // Consumir otro parentesis derecho (para cerrar la funcion)
            Eat(TokenType.RightParenthesis);

            // Crear un nuevo objeto Args para los parametros
            Args predicate = new Args();

            // Agregar la variable y la condicion a los parametros
            predicate.Add(variable);

            predicate.Add(condition);

            // Crear un nuevo nodo Function para la funcion Find
            Function node = new Function("Find", predicate);

            return node;
        }
        catch
        {
            throw;
        }
    }

    public Function GetPlayerFunction (string name, Scope<ASTType.Type> scope)
    {
        try
        {
            // Analizar la variable para el jugador
            Var player = Variable(scope);

            // Verificar si la variable está en el alcance y es de tipo Context
            if (!scope.IsInScope(player) || player.type != ASTType.Type.Context) 
            {
                // Error: Parametro invalido
                ErrorInvalidParameterInFunction(name);
            }

            // Crear un nuevo objeto Args para los parametros
            Args args = new Args();

            // Agregar el jugador a los parametros
            args.Add(player);
            
            // Crear un nuevo nodo Function con el nombre dado y los argumentos
            Function node = new Function(name, args);

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            return node;
        }

        catch
        {
            throw;
        }
    }

    public Function NoParametersFunction (string name)
    {
        try
        {
            // Crear un nuevo nodo Function con el nombre dado y sin argumentos
            Function node = new Function(name);

            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);

            // Devolver el nodo Function creado
            return node;
        }

        catch
        {
            throw;
        }
    }

    public Function CardParameterFunction (string name, Scope<ASTType.Type> scope)
    {
        try
        {
            // Analizar la variable para la carta
            Var card = Variable(scope);

            // Verificar si la variable esta en el alcance y es de tipo Card
            if (!scope.IsInScope(card) || card.type != ASTType.Type.Card)
            {
                // Error: Parametro invalido
                ErrorInvalidParameterInFunction(name);
            }

            // Crear un nuevo objeto Args para los parametros
            Args args = new Args();

            // Agregar la carta a los parametros
            args.Add(card);

            // Crear un nuevo nodo Function con el nombre dado y los argumentos            
            Function node = new Function(name, args);
            
            // Consumir el parentesis derecho
            Eat(TokenType.RightParenthesis);
            
            return node;
        }

        catch
        {
            throw;
        }
    }

    public void ErrorUnvalidAssignment(Var variable)
    {
        // Se llama cuando hay un problema especifico con una asignacion de una variable.
        
        // Imprime un mensaje de error indicando que la asignacion es invalida

        DebugError($"Unvalid Assignment of '{variable.value}'");
    }

    public void ErrorInvalidStatement()
    {
        // Generar un mensaje de error indicando que solo ciertas expresiones pueden usarse como sentencias
        DebugError("Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement");
    }

    public void ErrorEffectCalling(ParamName name)
    {
        // Generar un mensaje de error indicando que el nombre del efecto no ha sido declarado
        DebugError($"Effect '{name.paramName}' has not been declared");
    }

    public void ErrorEffectCalling(ASTType.Type type, ParamName name)
    {
        // Crear una nueva variable auxiliar con el nombre y tipo dados
        Var aux = new Var(new Token(TokenType.Identifier, name.paramName ,0 ,0), type);

        // Llamar al metodo ErrorUnvalidAssignment para manejar el error
        ErrorUnvalidAssignment(aux);
    }

    public bool IsValidSource(Token token)
    {
        // Obtener el lexema del token
        string s = token.Lexeme;

        // Verificar si el lexema es uno de los valores validos para fuentes
        return (s == "board" || s == "hand" || s == "deck" || s == "field" || s == "parent" || s == "otherBoard" || s == "otherHand" || s == "otherDeck" || s == "otherField");
    }

    public void ErrorAlReadyDefinesMember(string name)
    {
        // Generar un mensaje de error indicando que ya existe un miembro definido globalmente con el nombre dado
        DebugError($"Card Editor already defines a member '{name}'");
    }
}
