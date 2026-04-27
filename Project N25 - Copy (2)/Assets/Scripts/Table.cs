using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider2D col;
    public string tagTarget = "NPC";
    public List<Collider2D> detectedObjs = new List<Collider2D>();
    public GameObject inform;
    void Start()
    {
        col.GetComponent<Collider2D>();
        inform.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.root.CompareTag(tagTarget))
        {
            detectedObjs.Add(col);
            inform.SetActive(true);
            //Debug.Log("Enter");
        }
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.transform.root.CompareTag(tagTarget))
        {
            detectedObjs.Remove(col);
            inform.SetActive(false);
            Destroy(gameObject);
            //Debug.Log("Exit");
        }
    }
    
}
