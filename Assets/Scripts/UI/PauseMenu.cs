using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    [SerializeField] AudioClip SelectNoise;
    [SerializeField] AudioClip HoverNoise;

    [SerializeField] GameObject resumeButton;

    public bool isPaused;

    // track mouse position
    Vector2 lastMousePosition = Vector2.zero;

    private DefaultInputActions controls;

    private bool navigating = false;

    void Start()
    {
        pauseMenu.SetActive(false);

        controls = new DefaultInputActions();
        controls.UI.Navigate.performed += OnNavigate;
        controls.UI.Navigate.Enable();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // Exit navigation mode if the mouse moves
        if (isPaused && Vector2.Distance(Input.mousePosition, lastMousePosition) > 0.1f)
        {
            OnMouseMoved();
        }
        lastMousePosition = Input.mousePosition;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(SelectNoise, transform, 1f);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(SelectNoise, transform, 1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
    }
    
    public void QuitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(SelectNoise, transform, 1f);
        Application.Quit();
    }

    public void OnHover(){
        SoundFXManager.instance.PlaySoundFXClip(HoverNoise,transform,1f);
    }

    void OnMouseMoved()
    {
        if (!navigating)
            return;

        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
        navigating = false;
    }

    // On Navigate
    void OnNavigate(InputAction.CallbackContext context)
    {
        if (navigating || !isPaused)
            return;

        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);
        navigating = true;
    }
}



