using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Canvas menuCanvas;

    private void Start()
    {
        
        menuCanvas.enabled = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        
        menuCanvas.enabled = false;
        
        
        Time.timeScale = 1f;
        
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
