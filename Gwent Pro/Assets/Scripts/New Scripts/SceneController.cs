using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneController : MonoBehaviour
{
    public GameObject readyToPlayImage; // Cartel que da inicio a la escena.
    public Button readyToPlayButton; // Boton que inicia la escena.


    // Start is called before the first frame update
    void Start()
    {
        //Activa la imagen del cartel al inicio

        readyToPlayImage.SetActive(true);

        //Desactiva la interaccion de los objetos
        DisableInteractionWithOtherObjects();

        // Configura el boton para que al hacer clic, active la interaccion con los objetos.
        readyToPlayButton.onClick.AddListener(EnableInteractionsWithOtherObjects);
        
    }

    void DisableInteractionWithOtherObjects()
    {
        //Desactiva la interaccion con los objetos

        foreach (var interactableObject in FindObjectsOfType<InteractableObject>())
        {
            interactableObject.isInteractable = false;
        }
    }

    void EnableInteractionsWithOtherObjects()
    {
        // Activa la interaccion con los objetos
        foreach (var interactableObject in FindObjectsOfType<InteractableObject>())
        {
            interactableObject.isInteractable = true;
        }

        //Oculta la imagen del cartel
        readyToPlayImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
