using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PassScript : MonoBehaviour
{
    public void onClick()
    {
        GameManager.round();
    }
}
