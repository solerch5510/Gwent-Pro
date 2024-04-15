using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private GameObject dropZone;
    private GameObject startParent;
    private Vector2 startPosition;
    public InteractableObject interactableObject;

    



    
    // Update is called once per frame
    void Update()
    {
        if(isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x , Input.mousePosition.y);

        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;

        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;

        dropZone = null;
    }

    public void StartDrag()
    {

        // Desactivar el GridLayoutGroup al comenzar el arrastre
        FindObjectOfType<DrawCard>().DisableGridLayoutGroup();

        startParent = transform.parent.gameObject;

        startPosition = transform.position;

        isDragging = true;
    }
    
    public void EndDrag()
    {
        isDragging = false;

        if (isOverDropZone && WichZoneIs())
        {
            transform.SetParent(dropZone.transform, false);

            GameManager.passTurn(gameObject, gameObject.GetComponent<CardDisplay>().card.playerID);
        }

        else 
        {
            transform.position = startPosition;

            transform.SetParent(startParent.transform, false); 
        }

        //Reactivar el GridLayoutGroup al terminar arrastre
        FindObjectOfType<DrawCard>().EnableGridLayoutGroup();
    }

    public bool WichZoneIs ()  // Metodo para que la carta solo colisione con su zona correspondiente.
    {
        Zones conditions = dropZone.GetComponent<Zones>();
        
        string k = conditions.ZoneNames;
        
        string l = gameObject.GetComponent<CardDisplay>().Zone;
        
        if (k==l) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
