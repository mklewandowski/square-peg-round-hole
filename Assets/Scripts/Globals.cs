using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    // game scroll speed
    public static Vector2 ScrollSpeed = new Vector2(4f, 0);

    // moving direction
    public static Vector2 ScrollDirection = new Vector2(-1f, 0);

    public static int BaseLayer = 8;

    public enum GameState {
        TitleScreen,
        Playing,
        Dead,
        Score,
        ScoreRestart
    }
    public static GameState CurrentGameState = GameState.TitleScreen;

    // keep track of scoring
    public static int BestScore = 0;
    public static int CurrentScore = 0;

    public const string BestScorePlayerPrefsKey = "BestScore";
    public static void SaveToPlayerPrefs(string key, int val)
    {
        PlayerPrefs.SetInt(key, val);
    }
    public static int LoadFromPlayerPrefs(string key)
    {
        int val = PlayerPrefs.GetInt(key, 0);
        return val;
    }
}
