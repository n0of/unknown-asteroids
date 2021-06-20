using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Text output;

    void Update()
    {
        output.enabled = GameScript.instance.currentPlayer != null;
        output.text = $"Health: {GameState.Health}     Score: {GameState.Score}";
    }
}
