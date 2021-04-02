using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    #endregion

    private bool isPushed = false; // pushing trigger (only 1 time per round pushing is available)

    [SerializeField]
    private Player player; // player data

    private int currentGameLevel; // current game level (number)

    // touch utils
    private Vector2 pointerDown;
    private Vector2 pointerUp;

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        // DEMO 
        if (Input.GetKeyDown(KeyCode.C))
            StartCoroutine(ReloadCurrentScene(0f));

        // check for pushing
        if (isPushed) return;

        // check for player's swipe to push the puck
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                pointerDown = touch.position;

            if (touch.phase == TouchPhase.Ended)
            {
                pointerUp = touch.position;
                player.Push(pointerDown, pointerUp);
                isPushed = true;
            }
        }
    }

    // 'loosing' simulation
    public void Defeat() => StartCoroutine(ReloadCurrentScene(.25f));
    // 'winning' simulation
    public void Victory()
    {
        // inform game analytics about level completion
        GAManager.instance.OnLevelComplete(currentGameLevel);

        // play next level (without effects or transitions)
        // DEMO
        if (currentGameLevel == 6)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // start settings
    private void Init()
    {
        // target is 60 FPS
        Application.targetFrameRate = 60;

        // get current game level
        // (возьмем на заметку для ДЕМО, что 1 уровень начинается с buildIndex == 0)
        currentGameLevel = SceneManager.GetActiveScene().buildIndex + 1;
    }
    // reload current scene with delay
    private IEnumerator ReloadCurrentScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
