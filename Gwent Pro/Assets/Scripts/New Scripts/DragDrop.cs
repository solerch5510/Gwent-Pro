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
        }

        else 
        {
            transform.position = startPosition;

            transform.SetParent(startParent.transform, false); 
        }
    }

    public bool WichZoneIs ()
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
