using UnityEngine;
using GameAnalyticsSDK;

public class GAManager : MonoBehaviour
{
    #region Singleton

    public static GAManager instance;
    private void Awake()
    {
        instance = this;
    }

    #endregion

    private void Start()
    {
        GameAnalytics.Initialize();
    }

    public void OnLevelComplete(int level)
    {
        GameAnalytics.NewDesignEvent("LevelCompleted", level);
    }
}
