using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class pauseScript : MonoBehaviour

{
    [SerializeField] private GameObject pausePanel;
    void Start()
    {
        pausePanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pausePanel.activeInHierarchy)
            {
                PauseGame();
            }
            if (pausePanel.activeInHierarchy)
            {
                ContinueGame();
            }
        }
    }
    public void PauseGame()
    {

        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void ContinueGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

