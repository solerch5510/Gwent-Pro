using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class NewCard : MonoBehaviour
{
    public string Name;

    public int basedPower;

    public int showPower;

    public bool PlayerID;

    public string zone;

    public string description;

    public string classCard;

    public Effects Activation;

    public int DefaultEffect;

    /*public Card()
    {


    }*/

    public NewCard(bool Id, string CardName, int BasedPower, int Power, string CardDescription, string Zone, string ClassCard, Effects OnActivation, int DFE)
    {
        Id = PlayerID;

        CardName = name;

        BasedPower = basedPower;

        Power = showPower;

        CardDescription = description;

        Zone = zone;

        ClassCard = classCard;

        OnActivation = Activation;

        DFE = DefaultEffect;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
