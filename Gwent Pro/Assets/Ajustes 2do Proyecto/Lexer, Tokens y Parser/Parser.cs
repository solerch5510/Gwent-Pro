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
        DebugError($"Found invalid repetition of field {CurrentToken.Type} in current context");
    }

    public void ErrorNotCorrespondingField()
    {
        DebugError($"Invalid syntax in current context of {CurrentToken.Type.ToString()} token");

        Eat(CurrentToken.Type);
    }

    public void ErrorInNodeCreation(AST node)
    {
        DebugError($"Invalid construction of {node.GetType().ToString()} maybe you miss fields of {node.GetType().ToString()}");
    }

    public void ErrorInUnaryOp(UnaryOperators node)
    {
        DebugError($"Unvalid use of Unary Operator ({node.Operation.Lexeme}) in {node.Expression.type}  cannot convert {node.Expression.type} to {node.type}");
    }

    public void ErrorInBinOp(BinaryOperators node)
    {
        DebugError($"Invalid Binary Operator: Operator '{node.Operator.Lexeme}' cannot be applied to operands of type '{node.Left.type}' and '{node.Right.type}'");
    }

    public void ErrorHasNotBeenDeclared(Var variable)
    {
        DebugError($"Variable '{variable.value}' has not been declared");
    }

    public void ErrorInAssignment(Assign assign)
    {
        DebugError($"Invalid Assignment, cannot convert {assign.left.type}  to  {assign.right.type}");
    }

    public void DebugError(string errorTag)
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

    public void GetToken()
    {
        if(NumberOfToken < TokensList.Count)
        {
            NumberOfToken ++;

            CurrentToken = TokensList[NumberOfToken - 1];
        }
    }
    
public void Eat(TokenType tokenType)
    {
        try
        {
            if (CurrentToken.Type == tokenType)
            {
                GetToken();
            }
            else
            {
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
public ASTType Factor(Scope<ASTType.Type> scope)
    {
        try
        {
            Token token = CurrentToken;
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

            if (token.Type == TokenType.Bool)
            {
                Bool node = new Bool(token);

                Eat(TokenType.Bool);

                return node;
            }

            if (token.Type == TokenType.NumberLiteral)
            {
                Eat(TokenType.NumberLiteral);

                Int node = new Int(token);

                return node;
            }

            if (token.Type == TokenType.StringLiteral)
            {
                Eat(TokenType.StringLiteral);

                String node = new String(token);

                return node;
            }

            if (token.Type == TokenType.LeftParenthesis)
            {
                Eat(TokenType.LeftParenthesis);

                ASTType result = Expression(scope);

                Eat(TokenType.RightParenthesis);

                return result;
            }

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

            if (CurrentToken.Type == TokenType.Function)
            {
                return FunctionStatement(CurrentToken.Lexeme, scope);
            }

            DebugError($"Invalid Factor: {token.Lexeme}");

            return new NoOp();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public ASTType Term(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = Factor(scope);

            Token token = CurrentToken;

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

    public ASTType Expression(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = Term(scope);

            Token token = CurrentToken;

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

    public ASTType BooleanFactor(Scope<ASTType.Type> scope)
    {
        try
        {
            Token token = CurrentToken;

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

            if (token.Type == TokenType.LeftParenthesis)
            {
                Eat(TokenType.LeftParenthesis);

                ASTType result = BooleanExpression(scope);

                Eat(TokenType.RightParenthesis);

                return result;
            }

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

    public ASTType BooleanTerm(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = BooleanFactor(scope);

            Token token = CurrentToken;

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

    public ASTType BooleanExpression(Scope<ASTType.Type> scope)
    {
        try
        {
            ASTType node = BooleanTerm(scope);

            Token token = CurrentToken;

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
            Var node = new Var(CurrentToken);

            Eat(TokenType.Identifier);

            VarComp otherNode = new VarComp(node.token);

            if (CurrentToken.Type == TokenType.Dot || CurrentToken.Type == TokenType.LeftBrace)
            {
                if(CurrentToken.Type == TokenType.LeftBrace)
                {
                    Indexer indexer = IndexerParse(scope);

                    otherNode.args.Add(indexer);
                }
                

                while (CurrentToken.Type == TokenType.Dot && CurrentToken.Type != TokenType.EndOfFile)
                {
                    Eat(TokenType.Dot);

                    if (CurrentToken.Type == TokenType.Function)
                    {
                        Function function = FunctionStatement(CurrentToken.Lexeme, scope);

                        otherNode.args.Add(function);

                        if(CurrentToken.Type == TokenType.LeftBrace)
                        {
                            otherNode.args.Add(IndexerParse(scope));
                        }
                    }

                    else
                    {
                        Token token = CurrentToken;

                        if (token.Type == TokenType.Type || token.Type == TokenType.Name || token.Type == TokenType.Faction || token.Type == TokenType.Power || token.Type == TokenType.Range)
                        {
                            Eat(token.Type);

                            Var variable = new Var(token, ASTType.Type.String);

                            if(token.Type == TokenType.Power) 
                            {
                                variable.type = ASTType.Type.Int;
                            }

                            otherNode.args.Add(variable);
                        }

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

                        else
                        {
                            DebugError($"Invalid Field: '{CurrentToken.Lexeme}'");

                            Eat(CurrentToken.Type);
                        }
                    }
                }

                node = otherNode;
            }

            if (node.GetType() == typeof(Var))
            {
                if(scope.IsInScope(node)) 
                {
                    node.type = scope.Get(node);
                }
            }

            else if(!scope.IsInScope(node))
            {
                ErrorHasNotBeenDeclared(node);
            }

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
            Var left = variable;

            Token token = CurrentToken;

            Eat(TokenType.Assign);

            ASTType right = Expression(scope);

            Assign node = new Assign(left, token, right);

            if(token.Lexeme != "=")
            {
                if (!scope.IsInScope(variable)) 
                {
                    ErrorHasNotBeenDeclared(variable);
                }

                else if ((token.Lexeme == "+=" || token.Lexeme == "-=" || token.Lexeme == "*=" || token.Lexeme == "/=" || token.Lexeme == "%=")  && (variable.type == node.right.type && variable.type != ASTType.Type.Int))
                {
                    ErrorInAssignment(node);
                }

                else if ((token.Lexeme == "@=") && (variable.type == node.right.type && variable.type != ASTType.Type.String))
                {
                    ErrorInAssignment(node);
                }
            }

            if (variable.GetType() == typeof(Var))
            {
                if (!scope.IsInScope(variable))
                {
                    variable.type = node.right.type;

                    scope.Set(variable, variable.type);
                }

                else if (variable.type != node.right.type) 
                {
                    ErrorInAssignment(node);
                }
            }
            
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
            Eat(TokenType.Function);

            Eat(TokenType.LeftParenthesis);

            if (name == "Find") 
            {
                return FindFunction(scope);
            }

            if (name == "HandOfPlayer" || name == "FieldOfPlayer" || name == "DeckOfPlayer" || name == "GraveyardOfPlayer")
            {
                return GetPlayerFunction(name, scope);
            }

            if (name == "Pop" || name == "Shuffle") 
            {
                return NoParametersFunction(name);
            }

            if (name == "Push" || name == "Remove" || name == "Add" || name == "SendBottom") 
            {
                return CardParameterFunction(name, scope);
            }

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
            Eat(TokenType.For);

            Var target = Variable(scope);

            target.type = ASTType.Type.Card;

            if (scope.IsInScope(target) || target.GetType() == typeof(VarComp)) 
            {
                ErrorUnvalidAssignment(target);
            }

            else 
            {
                scope.Set(target,target.type);
            }

            Eat(TokenType.In);

            Var targets = Variable(scope);

            if (!scope.IsInScope(targets) || targets.type != ASTType.Type.Field) 
            {
                ErrorUnvalidAssignment(targets);
            }

            Compound body = CompoundStatement(scope);

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
            Eat(TokenType.While);

            Eat(TokenType.LeftParenthesis);

            AST condition = BooleanExpression(scope);

            Eat(TokenType.RightParenthesis);

            Compound body = CompoundStatement(scope);

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
            Eat(TokenType.If);

            Eat(TokenType.LeftParenthesis);

            AST condition = BooleanExpression(scope);

            Eat(TokenType.RightParenthesis);

            Compound body = CompoundStatement(scope);

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
            if (CurrentToken.Type == TokenType.While)
            {
                return WLStatement(scope);
            }

            if (CurrentToken.Type == TokenType.If)
            {
                return IfNodeStatement(scope);
            }

            if (CurrentToken.Type == TokenType.For)
            {
                return FLStatement(scope);
            }

            if (CurrentToken.Type == TokenType.SemiColon)
            {
                return new NoOp();
            }

            if (CurrentToken.Type == TokenType.Identifier)
            {
                Var variable = Variable(scope);

                if (variable.GetType() == typeof(VarComp) && CurrentToken.Type == TokenType.SemiColon)
                {
                    VarComp varComp = variable as VarComp;

                    int count = varComp.args.Count - 1;

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

                else if (CurrentToken.Type == TokenType.Assign)
                {
                    Assign node = AssignmentStatement(variable, scope);

                    return node;
                }

                else if (CurrentToken.Type == TokenType.Decrement || CurrentToken.Type == TokenType.Plus1)
                {
                    UnaryOperators plusnode = new UnaryOperators(CurrentToken, variable);

                    if (variable.type != ASTType.Type.Int) 
                    {
                        ErrorInUnaryOp(plusnode);
                    }

                    Assign node = new Assign(variable, CurrentToken, plusnode);

                    Eat(CurrentToken.Type);

                    return node;
                }

                DebugError($"Invalid Statement: token {CurrentToken.Type}");

                return new NoOp();
            }

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
            List<AST> results = new List<AST>();

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                AST node = Statement(scope);

                results.Add(node);

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
            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(outScope);

            Eat(TokenType.LeftBracket);

            List<AST> nodes = StatementList(scope);

            Eat(TokenType.RightBracket);

            Compound root = new Compound();

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
            Eat(TokenType.Type);

            Eat(TokenType.Colon);

            TypeNode node = new TypeNode(CurrentToken);

            if (node.type != "Gold" && node.type != "Silver")
            {
                DebugError("Invalid Type of Card: You may try with 'Gold' or 'Silver'");
            }

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
            Eat(TokenType.Faction);

            Eat(TokenType.Colon);

            Faction node = new Faction(CurrentToken);

             if (node.faction != "Paladins" && node.faction != "Monsters")
            {
                DebugError("Invalid Faction for Card: You may try with 'Paladins' or 'Monsters'");
            }

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
            Eat(TokenType.Power);

            Eat(TokenType.Colon);

            BasedPower node = new BasedPower(CurrentToken);

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
            Eat(TokenType.Range);

            Eat(TokenType.Colon);
            
            Eat(TokenType.LeftBrace);
            
            Range node = new Range(CurrentToken);

            if (node.range != "Melee" && node.range != "Range" && node.range != "Siege")
            {
                DebugError("Invalid Zone for Card: You may try with 'Melee', 'Range' or 'Siege'");
            }
            
            Eat(TokenType.StringLiteral);
            
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
            Eat(TokenType.OnActivation_Effect);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftBracket);

            ParamName name = null;

            Args parameters = new Args();

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                if (CurrentToken.Type == TokenType.Name)
                {
                    if (name == null)
                    {
                        name = NameParse();

                        if (!GlobalScope.IsInScope(name)) 
                        {
                            ErrorEffectCalling(name);
                        }

                        else
                        {
                            if (GlobalScope.Get(name) != ASTType.Type.Effect)
                            {
                                ErrorEffectCalling(GlobalScope.Get(name), name);
                            }
                                
                        }

                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else ErrorRepeat();
                }

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

                else
                { 
                    ErrorNotCorrespondingField(); 
                }
            }

            Eat(TokenType.RightBracket);

            EffectOnActivation node;

            if (parameters.args.Count == 0)
            {
                node = new EffectOnActivation(name);
            }
                
            else 
            {
                node = new EffectOnActivation(name, parameters);
            }

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
            Eat(TokenType.Context);

            Eat(TokenType.Colon);

            Token token = CurrentToken;

            Eat(TokenType.StringLiteral);

             if (!IsValidSource(token))
             {
                DebugError($"'{token.Lexeme}' is not a valid source of context");
             }
                

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
            Eat(TokenType.Single);

            Eat(TokenType.Colon);

            Token token = CurrentToken;

            Eat(TokenType.Bool);

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
            Eat(TokenType.Predicate);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftParenthesis);

            Var unit = Variable(GlobalScope);

            unit.type = ASTType.Type.Card;

            if (unit.GetType() == typeof(VarComp)) 
            {
                ErrorUnvalidAssignment(unit);
            }

            Eat(TokenType.RightParenthesis);

            Eat(TokenType.EqualGreater);

            Eat(TokenType.LeftParenthesis);

            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(GlobalScope);

            scope.Set(unit, unit.type);

            ASTType condition = BooleanExpression(scope);

            Eat(TokenType.RightParenthesis);

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
            Eat(TokenType.Targets);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftBracket);

            Source source = null;

            Single single = null;

            Predicate predicate = null;

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
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

                else 
                { 
                    ErrorNotCorrespondingField(); 
                }
            }

            Eat(TokenType.RightBracket);

            Selector node;

            if (single == null) 
            {
                node = new Selector(source, predicate);
            }

            else 
            {
                node = new Selector(source, single, predicate);
            }

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
            Eat(TokenType.PostAction);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftBracket);

            EffectOnActivation effectOnActivation = null;

            Selector selector = null;

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
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

                else 
                {
                    ErrorNotCorrespondingField();
                }
            }

            Eat(TokenType.RightBracket);

            PostAction node;

            if (selector == null) 
            {
                node = new PostAction(effectOnActivation);
            }

            else 
            {
                node = new PostAction(effectOnActivation, selector);
            }

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
            Eat(TokenType.LeftBracket);

            EffectOnActivation effectOnActivation = null;

            Selector selector = null;

            PostAction postAction = null;

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
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

                else if (CurrentToken.Type == TokenType.Targets)
                {
                    if (selector == null)
                    {
                        selector = SelectorParse();

                        if (CurrentToken.Type != TokenType.RightBracket) Eat(TokenType.Comma);
                    }

                    else 
                    {
                        ErrorRepeat();
                    }
                }

                else if (CurrentToken.Type == TokenType.PostAction)
                {
                    if (postAction == null)
                    {
                        postAction = PostActionParse();

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

                else 
                {
                    ErrorNotCorrespondingField();
                }
            }

            Eat(TokenType.RightBracket);

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

            if (effectOnActivation == null) 
            {
                ErrorInNodeCreation(node);
            }

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
            List<OnActivationElement> nodes = new List<OnActivationElement>();

            while (CurrentToken.Type != TokenType.RightBrace && CurrentToken.Type != TokenType.EndOfFile)
            {
                if (CurrentToken.Type == TokenType.LeftBracket)
                {
                    OnActivationElement node = OnActivationElementParse();

                    nodes.Add(node);

                    if (CurrentToken.Type != TokenType.RightBrace) 
                    {
                        Eat(TokenType.Comma);
                    }
                }

                else 
                {
                    ErrorNotCorrespondingField();
                }
            }

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
            Eat(TokenType.OnActivation);

            Eat(TokenType.Colon);
            
            Eat(TokenType.LeftBrace);
            
            List<OnActivationElement> list = OnActivationList();
            
            Eat(TokenType.RightBrace);

            OnActivation node = new OnActivation();
            
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
            Eat(TokenType.Card);

            Eat(TokenType.LeftBracket);

            ParamName name = null;

            TypeNode type = null;

            Faction faction = null;

            BasedPower power = null;
            
            Range range = null;

            OnActivation onActivation = null;

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                if (CurrentToken.Type == TokenType.Name)
                {
                    if (name == null)
                    {
                        name = NameParse();

                        if (GlobalScope.IsInScope(name)) 
                        {
                            ErrorAlReadyDefinesMember(name.paramName);
                        }

                        else 
                        {
                            GlobalScope.Set(name, ASTType.Type.Card);
                        }

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

                else if (CurrentToken.Type == TokenType.Type)
                {
                    if (type == null)
                    {
                        type = TypeParse();

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

                else if (CurrentToken.Type == TokenType.Faction)
                {
                    if (faction == null)
                    {
                        faction = FactionParse();

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

                else if (CurrentToken.Type == TokenType.Power)
                {
                    if (power == null)
                    {
                        power = PowerParse();

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

                else if (CurrentToken.Type == TokenType.Range)
                {
                    if (range == null)
                    {
                        range = RangeParse();

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

                else if (CurrentToken.Type == TokenType.OnActivation)
                {
                    if (onActivation == null)
                    {
                        onActivation = OnActivationParse();

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

                else 
                { 
                    ErrorNotCorrespondingField(); 
                }
            }

            Eat(TokenType.RightBracket);

            CardNode node = new CardNode(name, type, faction, power, range, onActivation);

            List<AST> listOfParameters = new List<AST> { name, type, faction, power, range, onActivation };
            
            foreach (AST child in listOfParameters)
            {
                if (child == null) 
                {
                    ErrorInNodeCreation(node);

                    break;
                }
            }

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
            Eat(TokenType.Name);

            Eat(TokenType.Colon);
            
            ParamName node = new ParamName(CurrentToken);

            if (node.paramName == "") 
            {
                DebugError("Name must not be an empty string");
            }

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
            Args args = new Args();

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                Var variable = Variable(scope);

                if (scope.IsInScope(variable) || variable.GetType() == typeof(VarComp))
                {
                    DebugError("Invalid declaration of param");
                }

                Eat(TokenType.Colon);

                if (CurrentToken.Type == TokenType.Var_Int || CurrentToken.Type == TokenType.Var_String || CurrentToken.Type == TokenType.Var_Bool)
                {
                    variable.TypeInParams(CurrentToken.Type);

                    scope.Set(variable, variable.type);

                    args.Add(variable);

                    Eat(CurrentToken.Type);

                    if (CurrentToken.Type != TokenType.RightBracket)
                    {
                        Eat(TokenType.Comma);
                    }
                }

                else
                {
                    DebugError($"Invalid Type '{CurrentToken.Lexeme}' found in current context\n Expecting 'String', 'Bool', 'Number'");

                    Eat(CurrentToken.Type);

                    if (CurrentToken.Type == TokenType.Comma) 
                    {
                        Eat(CurrentToken.Type);
                    }
                }
            }

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
            Eat(TokenType.Params);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftBracket);

            Args node = GetParametersInParams(scope);

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
            Eat(TokenType.Action);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftParenthesis);

            Var targets = Variable(outScope);

            targets.type = ASTType.Type.Field;

            if (outScope.IsInScope(targets) || targets.GetType() == typeof(VarComp))
            {
                ErrorUnvalidAssignment(targets);
            }

            else 
            {
                outScope.Set(targets, targets.type);
            }

            Eat(TokenType.Comma);

            Var context = Variable(outScope);

            context.type = ASTType.Type.Context;

            if (outScope.IsInScope(context) || context.GetType() == typeof(VarComp))
            {
                ErrorUnvalidAssignment(context);
            }   

            else 
            {
                outScope.Set(context, context.type);
            }

            Eat(TokenType.RightParenthesis);

            Eat(TokenType.EqualGreater);

            Compound body = CompoundStatement(outScope);

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
            Eat(TokenType.Effect);

            Eat(TokenType.LeftBracket);

            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(GlobalScope);

            ParamName name = null;

            Args parameters = null;

            Action action = null;

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                if (CurrentToken.Type == TokenType.Name)
                {
                    if (name == null)
                    {
                        name = NameParse();

                        if (GlobalScope.IsInScope(name)) 
                        {
                            ErrorAlReadyDefinesMember(name.paramName);
                        }

                        else 
                        {
                            GlobalScope.Set(name, ASTType.Type.Effect);
                        }

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

                else if (CurrentToken.Type == TokenType.Params)
                {
                    if (parameters == null)
                    {
                        parameters = ParamsEffectParse(scope);

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

                else if (CurrentToken.Type == TokenType.Action)
                {
                    if (action == null)
                    {
                        action = ActionParse(scope);

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

                else 
                { 
                    ErrorNotCorrespondingField();
                }
            }

            Eat(TokenType.RightBracket);

            EffectNode node;

            if (parameters == null)
            {
                node = new EffectNode(name, action);
            }
            else
            {
                node = new EffectNode(name, parameters, action, scope);
            }

            if (name == null || action == null)
            {
                ErrorInNodeCreation(node);
            }

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
            List<AST> listOfCardAndEffect = new List<AST>();

            while (CurrentToken.Type != TokenType.EndOfFile)
            {
                if (CurrentToken.Type == TokenType.Card)
                {
                    CardNode node = CardCreation();

                    listOfCardAndEffect.Add(node);
                }

                else if (CurrentToken.Type == TokenType.Effect)
                {
                    EffectNode node = EffectCreation();

                    listOfCardAndEffect.Add(node);
                }

                else 
                {
                    DebugError($"Expecting (card) or (effect) token, found: {CurrentToken.Type.ToString()}");

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
            List<AST> programList = ListOfCardAndEffect();

            Compound program = new Compound();

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
            Compound node = Program();

            if (CurrentToken.Type != TokenType.EndOfFile)
            {
                DebugError("Cannot parse all of text");
            }

            node.Express("");

            return node;
        }

        catch (System.Exception)
        {
            AST node = new NoOp();

            return node;
        }
    }
    public bool IsPossibleUnaryOp(UnaryOperators node)
    {
        return (node.type == node.Expression.type);
    }

    public bool IsPossibleBinOp(BinaryOperators node)
    {
        if (node.Left.type == node.Right.type)
        {
            if (node.type == ASTType.Type.Bool)
            {
                if (node.Left.type == ASTType.Type.Bool && (node.Operator.Type == TokenType.Or || node.Operator.Type == TokenType.And))
                {
                    return true;
                }

                if (node.Operator.Type == TokenType.Equal || node.Operator.Type == TokenType.BangEqual)
                {
                    return true;
                }
                     
                else 
                {
                    return (node.Left.type == ASTType.Type.Int);
                }
            }

            else 
            {
                return (node.Left.type == node.type);
            }
        }

        return false;
    }

    public bool IsValidIndexer(Indexer node)
    {
        return node.index.type == ASTType.Type.Int;
    }

    public Indexer IndexerParse(Scope<ASTType.Type> scope)
    {
        try
        {
            Eat(TokenType.LeftBrace);

            ASTType index = Expression(scope);

            Eat(TokenType.RightBrace);

            Indexer node = new Indexer(index);

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
        for (int i = 0; i < varComp.args.Count; i++)
        {
            if (i == 0)
            {
                if (!InternalIsPossibleVarComp(scope.Get(varComp.value), varComp.args[i])) 
                {
                    return false;
                }
            }

            else if (!InternalIsPossibleVarComp(varComp.args[i - 1].type, varComp.args[i])) 
            {
                return false;
            }
        }

        return true;
    }

    public bool IsFunction(ASTType node)
    {
        return (node.GetType() == typeof(Function));
    }

    public bool InternalIsPossibleVarComp(ASTType.Type fatherType, ASTType var)
    {
        if (fatherType == ASTType.Type.Context)
        {
            if (var.GetType() == typeof(Pointer))
            {
                Pointer p = var as Pointer;

                string s = p.pointer;

                return (s == "Hand" || s == "Graveyard" || s == "Deck" || s == "Melee" || s == "Range" || s == "Siege");
            }

            else if (IsFunction(var)) 
            {
                return (var.type == ASTType.Type.Context || var.type == ASTType.Type.Field);
            }
        }

        if (fatherType == ASTType.Type.Field)
        {
            if (var.GetType() == typeof(Indexer)) 
            {
                return true;
            }

            else if (IsFunction(var)) 
            {
                return (var.type == ASTType.Type.Field || var.type == ASTType.Type.Void || var.type == ASTType.Type.Card);
            }
        }

        if (fatherType == ASTType.Type.Indexer || fatherType == ASTType.Type.Card)
        {
            if (var.GetType() == typeof(Var))
            {
                Var otherVar = var as Var;

                string s = otherVar.value;

                return (s == "Type" || s == "Name" || s == "Faction" || s == "Range" || s == "Power" || s == "Owner");
            }

            else return false;
        }

        if (fatherType == ASTType.Type.Effect)
        {
            var otherValue = var as Var;

            string s = otherValue.value;

            return (s == "Name");
        }

        ErrorInVarCompConstruction(fatherType, var);

        return false;
    }

    public void ErrorInVarCompConstruction(ASTType.Type fatherType, ASTType var)
    {
        if (var.GetType() == typeof(Var))
        {
            Var otherValue = var as Var;

            DebugError($"Invalid VarComp construction: '{otherValue.value}' is not a valid field of type '{fatherType.ToString()}'");
        }

        else DebugError($"Invalid VarComp construction: '{var.ToString()}' is not a valid field of type '{fatherType.ToString()}'");
    }

    public void ErrorInvalidParameterInFunction(string functionName)
    {
        DebugError($"Invalid parameter for Function '{functionName}'");
    }

    public Function FindFunction(Scope<ASTType.Type> outScope)
    {
        try
        {
            Scope<ASTType.Type> scope = new Scope<ASTType.Type>(outScope);

            Eat(TokenType.LeftParenthesis);

            Var variable = Variable(scope);

            if (scope.IsInScope(variable) || variable.GetType() == typeof(VarComp)) 
            {
                ErrorInvalidParameterInFunction("Find");
            }

            else
            {
                variable.type = ASTType.Type.Card;

                scope.Set(variable, variable.type);
            }
            
            Eat(TokenType.RightParenthesis);

            Eat(TokenType.EqualGreater);

            Eat(TokenType.LeftParenthesis);

            ASTType condition = BooleanExpression(scope);

            Eat(TokenType.RightParenthesis);

            Eat(TokenType.RightParenthesis);

            Args predicate = new Args();

            predicate.Add(variable);

            predicate.Add(condition);

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
            Var player = Variable(scope);

            if (!scope.IsInScope(player) || player.type != ASTType.Type.Context) 
            {
                ErrorInvalidParameterInFunction(name);
            }

            Args args = new Args();

            args.Add(player);

            Function node = new Function(name, args);

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
            Function node = new Function(name);

            Eat(TokenType.RightParenthesis);

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
            Var card = Variable(scope);

            if (!scope.IsInScope(card) || card.type != ASTType.Type.Card)
            {
                ErrorInvalidParameterInFunction(name);
            }

            Args args = new Args();

            args.Add(card);
            
            Function node = new Function(name, args);
            
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
        DebugError($"Unvalid Assignment of '{variable.value}'");
    }

    public void ErrorInvalidStatement()
    {
        DebugError("Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement");
    }

    public void ErrorEffectCalling(ParamName name)
    {
        DebugError($"Effect '{name.paramName}' has not been declared");
    }

    public void ErrorEffectCalling(ASTType.Type type, ParamName name)
    {
        Var aux = new Var(new Token(TokenType.Identifier, name.paramName ,0 ,0), type);

        ErrorUnvalidAssignment(aux);
    }

    public bool IsValidSource(Token token)
    {
        string s = token.Lexeme;

        return (s == "board" || s == "hand" || s == "deck" || s == "field" || s == "parent" || s == "otherBoard" || s == "otherHand" || s == "otherDeck" || s == "otherField");
    }

    public void ErrorAlReadyDefinesMember(string name)
    {
        DebugError($"Card Editor already defines a member '{name}'");
    }
}
