using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfCollider : MonoBehaviour
{
  public Collider2D col;
  public string tagTarget = "Player";
  public List<Collider2D> detectedObjs = new List<Collider2D>();
  public GameObject interactButton;
  public GameObject BookshelfTab;
  public bool interactive = false;
  

  void Start(){
    col.GetComponent<Collider2D>();
    BookshelfTab.SetActive(false);
  }
  void OnTriggerEnter2D(Collider2D collider){
      if(collider.gameObject.tag == tagTarget){    
        detectedObjs.Add(collider);
        interactButton.SetActive(true);
        interactive = true;
      }
   }
  void OnTriggerExit2D(Collider2D collider){
      if(collider.gameObject.tag == tagTarget){
        detectedObjs.Remove(collider);
        interactButton.SetActive(false);
        interactive = false;
      }
   }
  void Update()
   {
      if (Input.GetKeyDown(KeyCode.E) && interactive == true && BookshelfTab.activeSelf == false)
        {
            OpenbookshelfTab();
        }
        else if (Input.GetKeyDown(KeyCode.E) && BookshelfTab.activeSelf == true)
        {
            ClosebookshelfTab();
        }
   }
  public void OpenbookshelfTab()
  { if(!BookshelfTab.activeSelf && PauseController.IsGamePaused)
    {
      return;
    }
    BookshelfTab.SetActive(true);
    PauseController.SetPause(true);
  }
  public void ClosebookshelfTab()
  {
    BookshelfTab.SetActive(false);
    PauseController.SetPause(false);
  }
   
      
   

}
