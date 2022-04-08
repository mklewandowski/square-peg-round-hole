using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneManager : MonoBehaviour
{
    private Vector2 movement;

    // the player
    [SerializeField]
    GameObject Player;

    // the backgrounds
    [SerializeField]
    GameObject[] Background;
    [SerializeField]
    GameObject[] BackgroundFar;
    [SerializeField]
    GameObject[] BackgroundIntro;

    // shape fields
    [SerializeField]
    GameObject ShapeFieldPrefab;

    // titles and messages
    [SerializeField]
    GameObject HUDHowToPlay;
    [SerializeField]
    GameObject HUDStart;
    [SerializeField]
    GameObject HUDStartContainer;
    [SerializeField]
    GameObject HUDFinalScore;
    [SerializeField]
    GameObject HUDTopScore;
    [SerializeField]
    GameObject HUDScore;
    [SerializeField]
    GameObject HUDScoreContainer;

    float fieldTimer = .5f;
    float fieldTimerMin = 3f;
    float fieldTimerMax = 5f;
	float dyingTimer = 0;
	float scoreTimer = 0;
	float dyingTimerMax = 1.5f;
	float scoreTimerMax = 2f;
    bool dropped = false;

    AudioSource audioSource;
    [SerializeField]
    AudioClip BlipSound;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;

        Globals.BestScore = Globals.LoadFromPlayerPrefs(Globals.BestScorePlayerPrefsKey);

        dyingTimer = dyingTimerMax;
        scoreTimer = scoreTimerMax;

        audioSource = this.GetComponent<AudioSource>();

#if UNITY_IPHONE || UNITY_ANDROID
        HUDStart.GetComponent<TextMeshPro>().text = "Tap to Start";
        HUDHowToPlay.GetComponent<TextMeshPro>().text = "Tap to change shape\nMatch shape when passing moving field\nAct FAST or LOSE!";
#else
        HUDStart.GetComponent<TextMeshPro>().text = "Space to Start";
        HUDHowToPlay.GetComponent<TextMeshPro>().text = "Space to change shape\nMatch shape when passing moving field\nAct FAST or LOSE!";

#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.CurrentGameState == Globals.GameState.TitleScreen)
        {
            UpdateTitleScreenState();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Playing)
        {
            UpdatePlaying();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Dead)
        {
            UpdateDead();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Score)
        {
            UpdateScore();
        }
        else if (Globals.CurrentGameState == Globals.GameState.ScoreRestart)
        {
            UpdateScoreRestart();
        }
    }

    void FixedUpdate()
    {
        if (Globals.CurrentGameState == Globals.GameState.Playing)
        {
            Vector2 backgroundMovement = new Vector2 (Globals.ScrollSpeed.x * Globals.ScrollDirection.x * .45f, 0);
            Vector2 backgroundFarMovement = new Vector2 (Globals.ScrollSpeed.x * Globals.ScrollDirection.x * .3f, 0);
            for (int i = 0; i < Background.Length; i++)
            {
                Background[i].GetComponent<Rigidbody2D>().velocity = backgroundMovement;
            }
            for (int i = 0; i < BackgroundFar.Length; i++)
            {
                BackgroundFar[i].GetComponent<Rigidbody2D>().velocity = backgroundFarMovement;
            }
        }
        else if (Globals.CurrentGameState == Globals.GameState.TitleScreen)
        {
            for (int i = 0; i < BackgroundIntro.Length; i++)
            {
                BackgroundIntro[i].GetComponent<Rigidbody2D>().velocity = new Vector2(-2f, 0);
            }
        }
    }

    void UpdateTitleScreenState()
    {
        if (Input.GetKeyDown ("space") || Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Fire2"))
        {
            audioSource.PlayOneShot(BlipSound, 1f);
            StartGame();
            Globals.CurrentGameState = Globals.GameState.Playing;
        }

        float backgroundMinX = -15f;
        for (int i = 0; i < BackgroundIntro.Length; i++)
        {
            if (BackgroundIntro[i].transform.localPosition.x < backgroundMinX)
            {
                int abutIndex = i == 0 ? BackgroundIntro.Length - 1 : i - 1;
                BackgroundIntro[i].transform.localPosition = new Vector2(
                        BackgroundIntro[abutIndex].transform.localPosition.x + BackgroundIntro[abutIndex].GetComponent<Renderer>().bounds.size.x,
                        BackgroundIntro[i].transform.localPosition.y
                    );
            }
        }
    }

    void UpdatePlaying()
    {
        fieldTimer -= Time.deltaTime;
        if (fieldTimer < 0)
        {
            //make new shape field
            GameObject shapeField = (GameObject)Instantiate(ShapeFieldPrefab, new Vector3(15, 0, 5), Quaternion.identity);
            ShapeField shapeFieldScript = shapeField.GetComponent<ShapeField>();
            shapeFieldScript.InitShapeField();

            fieldTimer = Random.Range(fieldTimerMin, fieldTimerMax);
            fieldTimerMin = Mathf.Max (1.3f, fieldTimerMin -= .25f);
            fieldTimerMax = Mathf.Max (2.3f, fieldTimerMax -= .25f);
            if (Globals.CurrentScore > 1)
            {
                Globals.ScrollSpeed.x = Mathf.Min(11f, Globals.ScrollSpeed.x + .25f);
            }
        }
        HUDScore.GetComponent<TextMeshPro>().text = "Score: " + Globals.CurrentScore.ToString();

        float backgroundMinX = -15f;
        for (int i = 0; i < Background.Length; i++)
        {
            if (Background[i].transform.localPosition.x < backgroundMinX)
            {
                int abutIndex = i == 0 ? Background.Length - 1 : i - 1;
                Background[i].transform.localPosition = new Vector2(
                        Background[abutIndex].transform.localPosition.x + Background[abutIndex].GetComponent<Renderer>().bounds.size.x,
                        Background[i].transform.localPosition.y
                    );
            }
        }
        for (int i = 0; i < BackgroundFar.Length; i++)
        {
            if (BackgroundFar[i].transform.localPosition.x < backgroundMinX)
            {
                int abutIndex = i == 0 ? BackgroundFar.Length - 1 : i - 1;
                BackgroundFar[i].transform.localPosition = new Vector2(
                        BackgroundFar[abutIndex].transform.localPosition.x + BackgroundFar[abutIndex].GetComponent<Renderer>().bounds.size.x,
                        BackgroundFar[i].transform.localPosition.y
                    );
            }
        }
    }

    void UpdateDead()
    {
        if (!dropped)
        {
            Player.GetComponent<Rigidbody2D>().gravityScale = 2;
            Player.GetComponent<Rigidbody2D>().isKinematic = false;
            for (int i = 0; i < Background.Length; i++)
            {
                Background[i].GetComponent<Rigidbody2D>().gravityScale = 2;
                Background[i].GetComponent<Rigidbody2D>().isKinematic = false;
            }
            for (int i = 0; i < BackgroundFar.Length; i++)
            {
                BackgroundFar[i].GetComponent<Rigidbody2D>().gravityScale = 2;
                BackgroundFar[i].GetComponent<Rigidbody2D>().isKinematic = false;
            }
            dropped = true;
        }
        dyingTimer -= Time.deltaTime;
        if (dyingTimer < 0)
        {
            Player.GetComponent<Rigidbody2D>().gravityScale = 0;
            Player.GetComponent<Rigidbody2D>().isKinematic = true;
            Player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            for (int i = 0; i < Background.Length; i++)
            {
                Background[i].GetComponent<Rigidbody2D>().gravityScale = 0;
                Background[i].GetComponent<Rigidbody2D>().isKinematic = true;
                Background[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            for (int i = 0; i < BackgroundFar.Length; i++)
            {
                BackgroundFar[i].GetComponent<Rigidbody2D>().gravityScale = 0;
                BackgroundFar[i].GetComponent<Rigidbody2D>().isKinematic = true;
                BackgroundFar[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
            dyingTimer = dyingTimerMax;
            dropped = false;
            Globals.CurrentGameState = Globals.GameState.Score;

            if (Globals.CurrentScore > Globals.BestScore)
            {
                Globals.BestScore = Globals.CurrentScore;
                Globals.SaveToPlayerPrefs(Globals.BestScorePlayerPrefsKey, Globals.BestScore);
            }

            HUDFinalScore.GetComponent<TextMeshPro>().text = "Your Score: " + Globals.CurrentScore.ToString();
            HUDTopScore.GetComponent<TextMeshPro>().text = "Best Score: " + Globals.BestScore.ToString();

            HUDScoreContainer.SetActive(true);
        }
    }

    void UpdateScore()
    {
        scoreTimer -= Time.deltaTime;
        if (scoreTimer <= 0)
        {
            scoreTimer = scoreTimerMax;
            Globals.CurrentGameState = Globals.GameState.ScoreRestart;
            HUDStartContainer.SetActive(true);
        }
    }

    void UpdateScoreRestart()
    {
        if (Input.GetKeyDown ("space") || Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Fire2"))
        {
            audioSource.PlayOneShot(BlipSound, 1f);
            StartGame();
            Globals.CurrentGameState = Globals.GameState.Playing;
        }
    }

    void StartGame()
    {
        fieldTimer = .5f;
        fieldTimerMin = 3f;
        fieldTimerMax = 5f;
        Globals.ScrollSpeed = new Vector2(4f, 0);
        Globals.CurrentScore = 0;
        HUDScore.GetComponent<TextMeshPro>().text = "Score: " + Globals.CurrentScore.ToString();
        HUDHowToPlay.SetActive(false);
        HUDStartContainer.SetActive(false);
        HUDScore.SetActive(true);
        HUDScoreContainer.SetActive(false);
        ResetPositions();
        for (int i = 0; i < Background.Length; i++)
        {
            Background[i].SetActive(true);
        }
        for (int i = 0; i < BackgroundFar.Length; i++)
        {
            BackgroundFar[i].SetActive(true);
        }
        for (int i = 0; i < BackgroundIntro.Length; i++)
        {
            BackgroundIntro[i].SetActive(false);
        }
        Player.SetActive(true);
    }

	void ResetPositions()
	{
        Player.transform.localPosition = new Vector3 (-4f, 0, 0);
        for (int i = 0; i < Background.Length; i++)
        {
            Background[i].transform.localPosition = new Vector3 (-5f + 10f * i, 0, 0);
        }
        for (int i = 0; i < BackgroundFar.Length; i++)
        {
            BackgroundFar[i].transform.localPosition = new Vector3 (-5f + 10f * i, 0, 0);
        }
	}
}
