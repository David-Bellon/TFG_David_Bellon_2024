using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(col.gameObject);
    }
}
