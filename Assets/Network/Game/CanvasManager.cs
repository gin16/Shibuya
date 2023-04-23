using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manage GUI, singleton.
/// </summary>
public class CanvasManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI phaseText;
    [SerializeField] GameObject optionCanvas;
    [SerializeField] RectTransform optionWindow;
    [SerializeField] TMPro.TMP_InputField nameInput;
    public string UserName { get { return nameInput.text; } }
    [SerializeField] Button[] gameOptionButtons;
    [SerializeField] GameObject readyCanvas;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] TMPro.TextMeshProUGUI scoreText;
    [SerializeField] TMPro.TextMeshProUGUI nextText;
    [SerializeField] TMPro.TextMeshProUGUI remainingText;
    [SerializeField] TMPro.TextMeshProUGUI timeText;
    [SerializeField] TMPro.TextMeshProUGUI resultNameText;
    [SerializeField] TMPro.TextMeshProUGUI resultScoreText;

    void Awake()
    {
        optionCanvas.SetActive(true);
        readyCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        phaseText.text = "Loading";

        foreach (Button button in gameOptionButtons) {
            button.interactable = false;
        }
    }

    public void SetActive(GamePhase phase) {
        optionCanvas.SetActive(phase == GamePhase.Top);
        readyCanvas.SetActive(phase == GamePhase.Ready);
        gameCanvas.SetActive(phase == GamePhase.Game);
        phaseText.text = "" + phase;

        foreach (Button button in gameOptionButtons) {
            button.interactable = (phase == GamePhase.Top);
        }
    }

    /// <summary>
    /// Called by Game when Phase == Game.
    /// </summary>
    public void UpdateGameText(int score, int next, int remaining, int time) {
        scoreText.text = "" + score;
        nextText.text = "" + next;
        remainingText.text = "" + remaining;
        timeText.text = $"{time}s";
    }

    void Update()
    {
        if (Game.Main == null) return;
        if (Game.Main.Phase == GamePhase.Game || Game.Main.Phase == GamePhase.Top) {
            if (Input.GetKeyDown(KeyCode.O)) {
                optionCanvas.SetActive(!optionCanvas.activeSelf);
                if (optionCanvas.activeSelf) {
                    // to manipulate options
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            // Close the option window if clicked outside the window
            if (optionCanvas.activeSelf && Input.GetMouseButtonDown(0) && !optionWindow.ContainsMouseCursor()) {
                optionCanvas.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Called by Game when Game finished.
    /// Show Result of Game, name and score in descending order
    /// </summary>
    public void ShowResult(List<Player> players) {
        players.Sort((a, b) => (a.Score == b.Score ? 0 : a.Score < b.Score ? -1 : 1));
        string nameText = "";
        string scoreText = "";
        foreach (Player player in players) {
            nameText += $"{Colored(player.PlayerName, player.PlayerId)}:\n";
            scoreText += $"{player.Score}\n";
        }
        resultNameText.text = nameText;
        resultScoreText.text = scoreText;
    }

    public string Colored(string str, int id) {
        string colorCode = "#" + ColorUtility.ToHtmlStringRGBA(Parameter.GetColor(id));
        return $"<color={colorCode}>{str}</color>";
    }
}
