using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CongratulationsButton : MonoBehaviour
{
    public GameObject readyToPlayImage; // Cartel que da inicio a la escena.
    public Button readyToPlayButton; // Boton que inicia la escena.


    // Start is called before the first frame update
    void Start()
    {
        
        
    }
    public void EnableInteractionsWithOtherObjects()
    {
        GameManager.endTurn1 = false;

        GameManager.endTurn2 = false;

        //Oculta la imagen del cartel
        readyToPlayImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
