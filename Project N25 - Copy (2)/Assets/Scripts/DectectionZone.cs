using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DectectionZone : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider2D col;
    public string tagTarget = "Table";
    public List<Collider2D> detectedObjs = new List<Collider2D>();
    void Start()
    {
        col.GetComponent<Collider2D>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == tagTarget)
        {
            detectedObjs.Add(col);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == tagTarget)
        {
            detectedObjs.Remove(col);
        }
    }
}
