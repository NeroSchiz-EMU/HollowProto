using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject startButton;
    [SerializeField] AudioClip SelectNoise;
    [SerializeField] AudioClip HoverNoise;
    [SerializeField] AudioClip UnClickNoise;
    private DefaultInputActions controls;

    // bool to track if in navigation mode
    bool navigating = false;

    // track mouse position
    Vector2 lastMousePosition = Vector2.zero;

    // Start
    private void Start()
    {
        controls = new DefaultInputActions();
        controls.UI.Navigate.performed += OnNavigate;
        controls.UI.Navigate.Enable();
    }

    private void Update()
    {
        // Exit navigation mode if the mouse moves
        if (Vector2.Distance(Input.mousePosition, lastMousePosition) > 0.1f)
        {
            OnMouseMoved();
        }
        lastMousePosition = Input.mousePosition;
    }

    // Play game
    public void Play()
    {
        SoundFXManager.instance.PlaySoundFXClip(SelectNoise, transform, 1f);
        SceneManager.LoadScene("Sewers");
    }

    // Quit game
    public void Quit()
    {
        SoundFXManager.instance.PlaySoundFXClip(SelectNoise, transform, 1f);
        Application.Quit();
    }

    //button locked and unclickable
    public void Locked(){
        SoundFXManager.instance.PlaySoundFXClip(UnClickNoise, transform, 1f);
    }

    //what button should do when button is hovered over
    public void OnHover(){
        SoundFXManager.instance.PlaySoundFXClip(HoverNoise,transform,1f);
    }
    // On Navigate
    void OnNavigate(InputAction.CallbackContext context)
    {
        if (navigating)
            return;

        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton);
        navigating = true;
    }

    void OnMouseMoved()
    {
        if (!navigating)
            return;

        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
        navigating = false;
    }
}
