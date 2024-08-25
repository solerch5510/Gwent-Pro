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

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;

        TokensList = lexer.tokenList;

        NumberOfToken = 0;

        CurrentError = "";

        CurrentToken = lexer.tokenList[0];
    }

    public void DebugError()
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
public AST Factor()
    {
        try
        {
            Token token = CurrentToken;
            if (token.Type == TokenType.Plus)
            {
                Eat(TokenType.Plus);

                UnaryOperators node = new UnaryOperators(token, Factor());

                return node;
            }

            if (token.Type == TokenType.Minus)
            {
                Eat(TokenType.Minus);

                UnaryOperators node = new UnaryOperators(token, Factor());

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

                AST result = Expression();

                Eat(TokenType.RightParenthesis);

                return result;
            }

            if (CurrentToken.Type == TokenType.Identifier)
            {
                return Variable();
            }

            if (CurrentToken.Type == TokenType.Function)
            {
                return FunctionStatement(CurrentToken.Lexeme);
            }

            DebugError();

            return new NoOp();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Term()
    {
        try
        {
            AST node = Factor();

            Token token = CurrentToken;

            if (token.Type == TokenType.Multiply || token.Type == TokenType.Divide || token.Type == TokenType.Mod)
            {
                Eat(token.Type);

                node = new BinaryOperators(node, token, Expression());
            }

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Expression()
    {
        try
        {
            AST node = Term();

            Token token = CurrentToken;
            if (token.Type == TokenType.Plus || token.Type == TokenType.Minus)
            {
                Eat(token.Type);

                node = new BinaryOperators(node, token, Expression());
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST BooleanFactor()
    {
        try
        {
            Token token = CurrentToken;

            if (token.Type == TokenType.Not)
            {
                Eat(TokenType.Not);

                Eat(TokenType.LeftParenthesis);

                UnaryOperators unaryOp = new UnaryOperators(token, BooleanExpression());

                Eat(TokenType.RightParenthesis);

                return unaryOp;
            }

            if (token.Type == TokenType.LeftParenthesis)
            {
                Eat(TokenType.LeftParenthesis);

                AST result = BooleanExpression();

                Eat(TokenType.RightParenthesis);

                return result;
            }

            AST left = Expression();

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

            else if (token.Type == TokenType.RightParenthesis) 
            {
                return left;
            }
            
            else DebugError();

            AST right = Expression();

            BinaryOperators node = new BinaryOperators(left, token, right);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public AST BooleanTerm()
    {
        try
        {
            AST node = BooleanFactor();

            Token token = CurrentToken;

            if (token.Type == TokenType.Or)
            {
                Eat(TokenType.Or);

                return new BinaryOperators(node, token, BooleanExpression());
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST BooleanExpression()
    {
        try
        {
            AST node = BooleanTerm();

            Token token = CurrentToken;

            if (token.Type == TokenType.And)
            {
                Eat(TokenType.And);

                node = new BinaryOperators(node, token, BooleanExpression());
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Var Variable()
    {
        try
        {
            Var node = new Var(CurrentToken);

            Eat(TokenType.Identifier);

            if (CurrentToken.Type == TokenType.Dot)
            {
                VarComp varComp = new VarComp(node.token);

                while (CurrentToken.Type == TokenType.Dot && CurrentToken.Type != TokenType.EndOfFile)
                {
                    Eat(TokenType.Dot);

                    if (CurrentToken.Type == TokenType.Function)
                    {
                        Function function = FunctionStatement(CurrentToken.Lexeme);

                        varComp.args.Add(function);
                    }

                    else
                    {
                        Token token = CurrentToken;

                        if (token.Type == TokenType.Type)
                        {
                            Eat(token.Type);

                            Type cardType = new Type(token);

                            varComp.args.Add(cardType);
                        }

                        else if (token.Type == TokenType.Name)
                        {
                            Eat(token.Type);

                            ParamName cardName = new ParamName(token);

                            varComp.args.Add(cardName);
                        }

                        else if (token.Type == TokenType.Faction)
                        {
                            Eat(token.Type);

                            Faction cardFaction = new Faction(token);

                            varComp.args.Add(cardFaction);
                        }
                        else if (token.Type == TokenType.Power)
                        {
                            Eat(token.Type);
                            ShowPower cardShowPower = new ShowPower();
                            
                            varComp.args.Add(cardShowPower);
                        }

                        else if (token.Type == TokenType.Range)
                        {
                            Eat(token.Type);

                            Range cardRange = new Range(token);

                            varComp.args.Add(cardRange);
                        }

                        else if (token.Type == TokenType.Pointer)
                        {
                            Eat(token.Type);

                            Var value = new Var(token);

                            value.value = "pointer ->" + value.value;

                            varComp.args.Add(value);
                        }

                        else
                        {
                            DebugError();

                            break;
                        }
                    }
                }

                node = varComp;
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Assign AssignmentStatement(Var variable)
    {
        try
        {
            Var left = variable;

            Token token = CurrentToken;

            Eat(TokenType.Assign);

            AST right = Expression();

            Assign node = new Assign(left, token, right);

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Function FunctionStatement(string name)
    {
        try
        {
            Args args = new Args();

            Eat(TokenType.Function);

            Eat(TokenType.LeftParenthesis);

            while (CurrentToken.Type != TokenType.RightParenthesis && CurrentToken.Type != TokenType.EndOfFile)
            {
                AST currentArg = Expression();

                args.Add(currentArg);

                if (CurrentToken.Type != TokenType.RightParenthesis)
                {
                    Eat(TokenType.Comma);
                }
            }

            Eat(TokenType.RightParenthesis);

            Function node = new Function(name, args);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public ForLoop FLStatement()
    {
        try
        {
            Eat(TokenType.For);

            Var target = Variable();

            Eat(TokenType.In);

            Var targets = Variable();

            Compound body = CompoundStatement();

            ForLoop node = new ForLoop(target, targets, body);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public WhileLoop WLStatement()
    {
        try
        {
            Eat(TokenType.While);

            Eat(TokenType.LeftParenthesis);

            AST condition = BooleanExpression();

            Eat(TokenType.RightParenthesis);

            Compound body = CompoundStatement();

            WhileLoop node = new WhileLoop(condition, body);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public IfNode IfNodeStatement()
    {
        try
        {
            Eat(TokenType.If);

            Eat(TokenType.LeftParenthesis);

            AST condition = BooleanExpression();

            Eat(TokenType.RightParenthesis);

            Compound body = CompoundStatement();

            IfNode node = new IfNode(condition, body);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Statement()
    {
        try
        {
            if (CurrentToken.Type == TokenType.While)
            {
                return WLStatement();
            }

            if (CurrentToken.Type == TokenType.If)
            {
                return IfNodeStatement();
            }

            if (CurrentToken.Type == TokenType.For)
            {
                return FLStatement();
            }

            if (CurrentToken.Type == TokenType.Function)
            {
                Function node = FunctionStatement(CurrentToken.Lexeme);

                return node;
            }

            if (CurrentToken.Type == TokenType.Identifier)
            {
                Var variable = Variable();

                if (variable.GetType() == typeof(VarComp) && CurrentToken.Type == TokenType.SemiColon)
                {
                    VarComp varComp = variable as VarComp;

                    if (varComp.args[varComp.args.Count - 1].GetType() == typeof(Function))
                    {
                        Function f = varComp.args[varComp.args.Count - 1] as Function;

                        if (f.type != Var.Type.VOID) DebugError();
                    }
                    else DebugError();

                    return variable as VarComp;
                }

                else if (CurrentToken.Type == TokenType.Assign)
                {
                    Assign node = AssignmentStatement(variable);

                    return node;
                }

                return new NoOp();
            }

            DebugError();

            return new NoOp();
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public List<AST> StatementList()
    {
        try
        {
            List<AST> results = new List<AST>();

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                AST node = CompoundStatement();

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

    public Compound CompoundStatement()
    {
        try
        {
            Eat(TokenType.LeftBracket);

            List<AST> nodes = StatementList();

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

    public Type TypeParse()
    {
        try
        {
            Eat(TokenType.Type);

            Eat(TokenType.Colon);

            Type node = new Type(CurrentToken);

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

                        if (CurrentToken.Type != TokenType.RightBracket) 
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else DebugError();
                }

                else if (CurrentToken.Type == TokenType.Identifier)
                {
                    Var variable = Variable();

                    Token token = CurrentToken;

                    Eat(TokenType.Colon);

                    AST value = Expression();

                    Assign param = new Assign(variable, token, value);

                    parameters.Add(param);

                    if (CurrentToken.Type != TokenType.RightBracket) Eat(TokenType.Comma);
                }

                else
                { 
                    DebugError();
                    
                    Eat(CurrentToken.Type); 
                }
            }

            Eat(TokenType.RightBracket);

            if (name == null) 
            {
                DebugError();
            }

            EffectOnActivation node;

            if (parameters.args.Count == 0)
            {
                node = new EffectOnActivation(name);
            }
                
            else 
            {
                node = new EffectOnActivation(name, parameters);
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

            Var unit = Variable();

            unit.type = Var.Type.CARD;

            Eat(TokenType.RightParenthesis);

            Eat(TokenType.EqualGreater);

            Eat(TokenType.LeftParenthesis);

            AST condition = BooleanExpression();

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
                        DebugError();
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

                    else DebugError();
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

                    else DebugError();
                }

                else 
                { 
                    DebugError(); 
                    
                    Eat(CurrentToken.Type); 
                }
            }

            Eat(TokenType.RightBracket);

            if (source == null || predicate == null) 
            {
                DebugError();
            }

            Selector node;

            if (single == null) 
            {
                node = new Selector(source, predicate);
            }

            else 
            {
                node = new Selector(source, single, predicate);
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

            Type type = null;

            Selector selector = null;

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                if (CurrentToken.Type == TokenType.Type)
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
                        DebugError();
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
                        DebugError();
                    }
                }

                else 
                {
                    DebugError();
                }
            }

            Eat(TokenType.RightBracket);

            if (type == null) 
            {
                DebugError();
            }

            PostAction node;

            if (selector == null) 
            {
                node = new PostAction(type);
            }

            else 
            {
                node = new PostAction(type, selector);
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
                        DebugError();
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
                        DebugError();
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
                        DebugError();
                    }
                }

                else 
                {
                    DebugError();
                }
            }

            Eat(TokenType.RightBracket);


            if (effectOnActivation == null || selector == null) 
            {
                DebugError();
            }

            OnActivationElement node;

            if (postAction == null) node = new OnActivationElement(effectOnActivation, selector);

            else node = new OnActivationElement(effectOnActivation, selector, postAction);

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
                    DebugError();
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

            Type type = null;

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

                        if (CurrentToken.Type != TokenType.RightBracket) Eat(TokenType.Comma);
                    }
                    
                    else 
                    {
                        DebugError();
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
                        DebugError();
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
                        DebugError();
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
                        DebugError();
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
                        DebugError();
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
                        DebugError();
                    }
                }

                else 
                { 
                    DebugError();  
                    
                    Eat(CurrentToken.Type); 
                }
            }

            Eat(TokenType.RightBracket);

            List<AST> listOfParameters = new List<AST> { name, type, faction, power, range, onActivation };
            
            foreach (AST child in listOfParameters)
            {
                if (child == null) 
                {
                    DebugError();
                }
            }

            CardNode node = new CardNode(name, type, faction, power, range, onActivation);

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

            Eat(TokenType.StringLiteral);

            return node;

        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Args GetParametersInParams()
    {
        try
        {
            Args args = new Args();

            while (CurrentToken.Type != TokenType.RightBracket && CurrentToken.Type != TokenType.EndOfFile)
            {
                Var variable = Variable();

                Eat(TokenType.Colon);

                if (CurrentToken.Type == TokenType.Var_Int || CurrentToken.Type == TokenType.Var_String || CurrentToken.Type == TokenType.Var_Bool)
                {
                    variable.TypeInParams(CurrentToken.Type);

                    args.Add(variable);

                    Eat(CurrentToken.Type);

                    if (CurrentToken.Type != TokenType.RightBracket)
                    {
                        Eat(TokenType.Comma);
                    }
                }

                else
                {
                    DebugError();

                    return args;
                }
            }

            return args;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Args ParamsEffectParse()
    {
        try
        {
            Eat(TokenType.Params);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftBracket);

            Args node = GetParametersInParams();

            Eat(TokenType.RightBracket);

            return node;
        }

        catch (System.Exception)
        {
            throw;
        }
    }

    public Action ActionParse()
    {
        try
        {
            Eat(TokenType.Action);

            Eat(TokenType.Colon);

            Eat(TokenType.LeftParenthesis);

            Var targets = Variable();

            targets.type = Var.Type.TARGETS;

            Eat(TokenType.Comma);

            Var context = Variable();

            context.type = Var.Type.CONTEXT;

            Eat(TokenType.RightParenthesis);

            Eat(TokenType.EqualGreater);

            Compound body = CompoundStatement();

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

                        if (CurrentToken.Type != TokenType.RightBracket)
                        {
                            Eat(TokenType.Comma);
                        }    
                    }

                    else 
                    {
                        DebugError();
                    }
                }

                else if (CurrentToken.Type == TokenType.Params)
                {
                    if (parameters == null)
                    {
                        parameters = ParamsEffectParse();

                        if (CurrentToken.Type != TokenType.RightBracket)
                        {
                            Eat(TokenType.Comma);
                        }
                    }

                    else 
                    {
                        DebugError();
                    }
                }

                else if (CurrentToken.Type == TokenType.Action)
                {
                    if (action == null)
                    {
                        action = ActionParse();

                        if (CurrentToken.Type != TokenType.RightBracket)
                        {
                            Eat(TokenType.Comma);
                        }                            
                    }
                    
                    else 
                    {
                        DebugError();
                    }
                }

                else 
                { 
                    DebugError(); 
                    
                    Eat(CurrentToken.Type); 
                }
            }

            Eat(TokenType.RightBracket);

            if (name == null || action == null)
            {
                DebugError();
            }

            EffectNode node;

            if (parameters == null)
            {
                node = new EffectNode(name, action);
            }
            else
            {
                node = new EffectNode(name, parameters, action);
            }

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
                    DebugError();

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
                DebugError();
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
}
