using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{   
    public GameObject pauseMenu;
    public GameObject inventoryMenu;
    public GameObject BookShelf;
    
    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PauseController.SetPause(false);
        Time.timeScale = 1f;
    }
    public void OnPause()
    {
        if(!pauseMenu.activeSelf && PauseController.IsGamePaused)
        {
           return;
        }
        pauseMenu.SetActive(true);
        PauseController.SetPause(true);
        Time.timeScale = 0f;
    }
    public void OnResume()
    {
        pauseMenu.SetActive(false);
        PauseController.SetPause(false);
        Time.timeScale = 1f;
    }
    public void OnExit()
    {
        Application.Quit();
    }

    public void openInventory()
    {
        if(!inventoryMenu.activeSelf && PauseController.IsGamePaused)
        {
            return;
        }
        inventoryMenu.SetActive(true);
        PauseController.SetPause(true);
    }
    public void closeInventory()
    {
        inventoryMenu.SetActive(false);
        PauseController.SetPause(false);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu.activeSelf == false)
        {
            OnPause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu.activeSelf == true)
        {
            OnResume();
        }
        if (Input.GetKeyDown(KeyCode.Q) && inventoryMenu.activeSelf == false)
        {
            openInventory();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && inventoryMenu.activeSelf == true)
        {
            closeInventory();
        }
        
    }
    
}
