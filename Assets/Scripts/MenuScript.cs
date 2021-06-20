using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public GameObject resumeButton;
    public Text toggleButton;

    private Button _resumeButton;

    void Awake()
    {
        _resumeButton = resumeButton.GetComponent<Button>();
    }

    void Update()
    {
        _resumeButton.interactable = GameScript.instance.currentPlayer?.gameObject.activeSelf ?? false;
        var toggleState = GameState.IsKeyboardOnly ? "Клавиатура" : "Клавиатура + мышь";
        toggleButton.text = $"Управление: {toggleState}";
    }

    private void OnEnable()
    {
        GameState.IsPaused = true;
    }

    public void OnResume()
    {
        if (!_resumeButton.interactable)
            return;

        CloseMenu(false);
    }

    public void OnNewGame()
    {
        CloseMenu(true);
    }

    public void OnToggleIsKeyboardOnly()
    {
        GameState.IsKeyboardOnly = !GameState.IsKeyboardOnly;
    }

    public void OnExit()
    {
        Application.Quit();
    }

    private void CloseMenu(bool asNew)
    {
        GameState.IsPaused = false;
        gameObject.SetActive(false);
        if (asNew)
            GameScript.instance.StartGame();
    }
}
