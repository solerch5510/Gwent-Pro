using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Clase Abstracta AST

public abstract class AST
{
    // Metodo Abstracto para imprimir la representacion del nodo.
    public abstract void Express (string Height);
}

public abstract class ASTType : AST
{
    public enum Type
    {
        Indexer, Int, String, Bool, Void, Effect, Card, Context, Null , Field
    }
    public Type type = Type.Null;
}
