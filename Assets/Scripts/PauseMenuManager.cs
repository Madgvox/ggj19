using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField] private bool isPaused;

    //sounds
    public AudioSource onMouseOver;
    public AudioSource buttonClick;
    public AudioSource openMenu;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            ActivateMenu();
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            ButtonClickSound();
            DeactivateMenu();
        }
        
    }

    public void ActivateMenu()
    {
        openMenu.Play();
        pauseMenuUI.SetActive(true);
        isPaused = true;
    }

    public void DeactivateMenu()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void MainMenuButton(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void OnMouseOver()
    {
        onMouseOver.Play();
    }

    public void ButtonClickSound()
    {
        buttonClick.Play();
    }

}
