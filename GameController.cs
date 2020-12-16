using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PenguinRun
{
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum PlayerState
    {
        Alive,
        Dead
    }

    public class GameController : MonoBehaviour
    {
        private GameDifficulty m_CurrentDifficulty;
        public GameDifficulty gameDifficulty
        {
            get { return m_CurrentDifficulty; }
            set
            {
                m_CurrentDifficulty = value;
                m_MainCharacter.SetWalkSpeed(m_CurrentDifficulty);
                m_HazardsManager.SetHazardCount(m_CurrentDifficulty);
                SetGameElementsSpeed();
                SetTimeRange();
            }
        }

        private PlayerState m_PlayerState;
        public PlayerState playerState
        {
            get { return m_PlayerState; }
            set
            {
                m_PlayerState = value;
                if (value == PlayerState.Dead)
                {
                    EndGame();
                }
            }
        } 

        public static GameController Instance;

        private CharacterController m_MainCharacter;
        private EnvironmentManager m_EnvironmentManager;
        private PlayerInput m_PlayerActionController;
        private GUIManager m_GuiManager;
        private HazardsManager m_HazardsManager;
        private PathManager m_PathManager;

        //----------------------------------------------------------------
        //Time and Score
        private int m_Score = 0;
        private float m_TimeRange = 0;
        //----------------------------------------------------------------
        //Score thresholds to change difficult
        private const int MEDIUM_THRESHOLD = 100;
        private const int HARD_THRESHOLD = 200;

        private const float EASY_SPEED = 0.04f;
        private const float MEDIUM_SPEED = 0.15f;
        private const float HARD_SPEED = 0.3f;

        void Awake()
        {
            Instance = this;

#if UNITY_ANDROID
            Screen.orientation = ScreenOrientation.Landscape;
#endif

            Vector3 bottomRightScreenCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0));

            m_MainCharacter = this.transform.Find("Penguin").gameObject.AddComponent<CharacterController>();
            if (m_MainCharacter != null)
            {
                InitialiseControls();
            }
            else
            {
                Debug.LogError("MainCharacterNull");
            }

            var environmentTransform = this.transform.Find("Environment");

            m_EnvironmentManager = environmentTransform.gameObject.AddComponent<EnvironmentManager>();
            if (m_EnvironmentManager == null)
                Debug.LogError("EnvironmentManagerNull");
            m_EnvironmentManager.Initialise(bottomRightScreenCorner.x);

            Vector3 penguinPos = m_MainCharacter.gameObject.transform.position;
            float penguinWidth = m_MainCharacter.gameObject.GetComponent<BoxCollider2D>().size.x;
            m_HazardsManager = environmentTransform.gameObject.AddComponent<HazardsManager>();
            m_HazardsManager.Initialise(penguinPos, penguinWidth, bottomRightScreenCorner.x);

            m_PathManager = environmentTransform.gameObject.AddComponent<PathManager>();
            m_PathManager.Initialise(bottomRightScreenCorner.x);

            m_GuiManager = this.transform.Find("GUI").gameObject.AddComponent<GUIManager>();
            m_GuiManager.Initialise();

            gameDifficulty = GameDifficulty.Easy;
            playerState = PlayerState.Alive;



            StartCoroutine(Timer());
        }

        void Update()
        {
            if (m_CurrentDifficulty != GameDifficulty.Hard)
                CheckScore();
        }

        private void OnEnable()
        {
            m_PlayerActionController.Enable();
        }

        private void InitialiseControls()
        {
            m_PlayerActionController = new PlayerInput();
            m_PlayerActionController.Action.Jump.performed += ctx => m_MainCharacter.Jump(true);
            m_PlayerActionController.Action.Jump.canceled += ctx => m_MainCharacter.Jump(false);

        }

        //Timer to set the player score
        IEnumerator Timer()
        {
            while (m_PlayerState == PlayerState.Alive)
            {
                m_Score++;
                m_GuiManager.SetScore(m_Score.ToString());
                yield return new WaitForSeconds(m_TimeRange);
            }
        }

        private void SetTimeRange()
        {
            switch (m_CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    m_TimeRange = 1f; 
                    break;
                case GameDifficulty.Medium:
                    m_TimeRange = 0.7f;
                    break;
                case GameDifficulty.Hard:
                    m_TimeRange = 0.3f;
                    break;
            }
        }

        private void SetGameElementsSpeed()
        {
            switch (m_CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    m_EnvironmentManager.backgroundSpeed = EASY_SPEED;
                    m_HazardsManager.hazardsSpeed = EASY_SPEED;
                    m_PathManager.pathSpeed = EASY_SPEED;
                    break;
                case GameDifficulty.Medium:
                    m_EnvironmentManager.backgroundSpeed = MEDIUM_SPEED;
                    m_HazardsManager.hazardsSpeed = MEDIUM_SPEED;
                    m_PathManager.pathSpeed = MEDIUM_SPEED;

                    break;
                case GameDifficulty.Hard:
                    m_EnvironmentManager.backgroundSpeed = HARD_SPEED;
                    m_HazardsManager.hazardsSpeed = HARD_SPEED;
                    m_PathManager.pathSpeed = HARD_SPEED;
                    break;
            }
        }

        //Check the actual score to increase difficulty level
        private void CheckScore()
        {
            if (m_CurrentDifficulty == GameDifficulty.Easy)
            {
                if (m_Score > MEDIUM_THRESHOLD)
                    m_CurrentDifficulty = GameDifficulty.Medium;
            }
            else if (m_CurrentDifficulty == GameDifficulty.Medium)
            {
                if (m_Score > HARD_THRESHOLD)
                    m_CurrentDifficulty = GameDifficulty.Hard;
            }
        }
    
        private void EndGame()
        {
            //Stop the game
            Time.timeScale = 0;

            //Show end game screen
            m_GuiManager.ShowEndGameScreen();
        }

        public void Quit()
        {

        }

        public void Restart()
        {
            
        }

    }
}

