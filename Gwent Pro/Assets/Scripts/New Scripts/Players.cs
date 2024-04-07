using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Players : MonoBehaviour
{
    public class Player
    {
        public List<GameObject> Cards = new List<GameObject>();

        public void AddCard(GameObject card)
        {
            Cards.Add(card);
        }
    }

    public Player Player1 = new Player();

    public Player Player2 = new Player();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
