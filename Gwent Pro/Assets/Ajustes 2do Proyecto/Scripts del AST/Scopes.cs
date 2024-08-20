using System;
using System.Collections.Generic;
public class Scope
{
    //Dicionario privado para almacenar pares nombre-valor de simbolos/simbolos
    private Dictionary<string, object> symbols = new Dictionary<string, object>();
    // Referencia al ambito padre
    private Scope parent;

    //Constructor que acepta un ambito padre opcional
    public Scope(Scope parentScope = null)
    {
        parent = parentScope;
    }

    //Metodo para definir un simbolo en el ambito actual 
    public void Define(string name, object value)
    {
        symbols[name] = value;
    }

    // Metodo para buscar un simbolo por su nombre en el ambito actual y todos sus padres 
    public object Lookup(string name)
    {
        var scope = this;

        // Busqueda recursiva del simbolo en el ambito actual y todos sus ambitos padres 
        while (scope != null)
        {
            if (scope.symbols.TryGetValue(name, out var value))
            {
                // Si se encuentra el simbolo, se devuelve su valor
                return value;
            }

            // Se pasa al siguiente ambito padre si no se encontro el simbolo en el actual
            scope = scope.parent;
        }

        // Si despues de buscar en todos los ambitos no se encuentra el simbolo, se lanza una excepcion.
        throw new Exception($"Symbol {name} not found"); 
    }
}