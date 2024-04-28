using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager
{
    public static void ClearScores()
    {
        PlayerPrefs.DeleteKey(c_highScoresKey);
    }

    public static bool IsHighScore(uint score)
    {
        var scores = GetScores();

        // If there is space for this score in the list, it's automatically a high score.
        if (scores.Count < c_maxScores)
        {
            return true;
        }

        // Loop through the items see if this score is better than any of the existing scores.
        for (int i = 0; i < scores.Count; i++)
        {
            if (score > scores[i].score)
            {
                return true;
            }
        }

        return false;
    }

    public static void AddScore(string playerName, uint score)
    {
        bool scoreAdded = false;

        var scores = GetScores();

        // Loop through the items and find the right place to insert the new score.
        for (int i = 0; i < scores.Count; i++)
        {
            if (score > scores[i].score)
            {
                var newScore = new PlayerScore(playerName, score);

                scores.Insert(i, newScore);

                scoreAdded = true;

                break;
            }
        }

        // If we didn't add an item and we're not at our max Score count, it means
        // there's space to add it at the end. So we can just add the item.
        if (!scoreAdded && scores.Count < c_maxScores)
        {
            var newScore = new PlayerScore(playerName, score);

            scores.Add(newScore);

            scoreAdded = true;
        }

        if (scoreAdded)
        {
            // Make sure we only keep the top N scores by shaving off any
            // extra items in the list. 
            for (int i = scores.Count - 1; i >= c_maxScores; i--)
            {
                scores.RemoveAt(i);
            }

            // Now save the updated score list.
            SaveScores(scores);
        }
    }

    public static List<PlayerScore> GetScores()
    {
        string scoresJson = PlayerPrefs.GetString(c_highScoresKey);

        if (!string.IsNullOrEmpty(scoresJson))
        {
            return JsonConvert.DeserializeObject<List<PlayerScore>>(scoresJson);
        }
        else
        {
            return new List<PlayerScore>();
        }
    }

    private static void SaveScores(List<PlayerScore> scores)
    {
        string scoresJson = JsonConvert.SerializeObject(scores);

        PlayerPrefs.SetString(c_highScoresKey, scoresJson);
    }

    internal static uint GetPendingHighScore()
    {
        return pendingHighScore;
    }

    internal static void SetPendingHighScore(uint score)
    {
        pendingHighScore = score;
    }

    internal static void ClearPendingHighScore()
    {
        pendingHighScore = 0;
    }

    internal static string GetScoreFormattedString(uint score)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(score);

        return $"{timeSpan.Minutes.ToString("d2")}:{timeSpan.Seconds.ToString("d2")}:{timeSpan.Milliseconds.ToString("d3")}";
    }

    private const int c_maxScores = 5;
    private const string c_highScoresKey = "HighScores";

    private static uint pendingHighScore = 0;
}

public struct PlayerScore
{
    public string name;
    public uint score;

    public PlayerScore(string name, uint score)
    {
        this.name = name;
        this.score = score;
    }
}