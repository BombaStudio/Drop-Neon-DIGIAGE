using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Text scoreText;

    public int Score { get; private set; } = 0;
    public bool IsGameOver { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    public void Start()
    {
        AddScore(0);
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;

        Score += amount;
        Debug.Log($"Score: {Score}");
        if (scoreText != null) scoreText.text = $"Score: {Score}";
    }

    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        Debug.Log($"GAME OVER! Final Score: {Score}");

        // Optional: Reload scene after a delay or show UI
        Invoke(nameof(RestartGame), 2.0f);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
