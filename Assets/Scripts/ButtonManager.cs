using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    public AudioSource buttonClick;
    public AudioSource mouseOverSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGameButton(string newGameLevel)
    {
        buttonClick.Play();
        SceneManager.LoadScene(newGameLevel);
    }

    public void ExitGameButton()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    public void MouseOverSound()
    {
        mouseOverSound.Play();
    }
}
