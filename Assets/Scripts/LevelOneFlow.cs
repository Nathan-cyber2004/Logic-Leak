using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Connects the authored LevelOne scene UI to the lesson and battle flow.</summary>
public class LevelOneFlow : MonoBehaviour
{
    enum Phase { Tutorial, Enemy, Boss, Victory, Defeat }

    sealed class QuizQuestion
    {
        public string prompt, code, answer, explanation;
        public string[] wrong;
        public QuizQuestion(string prompt, string code, string answer, string explanation, params string[] wrong)
        {
            this.prompt = prompt;
            this.code = code;
            this.answer = answer;
            this.explanation = explanation;
            this.wrong = wrong;
        }
    }

    [Header("Authored tutorial panels")]
    [SerializeField] WorldData worldData;
    [SerializeField] GameObject[] tutorialPanels = new GameObject[3];
    [SerializeField] TMP_Text[] tutorialText = new TMP_Text[3];
    [SerializeField] TMP_Text[] tutorialCode = new TMP_Text[3];
    [SerializeField] GameObject gamePanel;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject endOfLevelPanel;
    [SerializeField] TMP_Text endMessage, correctAnswersText, wrongAnswersText, endNextText;
    [SerializeField] Button endNextButton;

    [Header("Battle UI")]
    [SerializeField] Slider playerHealthBar;
    [SerializeField] Slider enemyHealthBar;
    [SerializeField] Slider levelTimer;
    [SerializeField] TMP_Text enemyNameText;
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text codeText;
    [SerializeField] Button[] optionButtons = new Button[4];
    [SerializeField] Button hintButton, freezeButton, healButton;
    [SerializeField] TMP_Text hintLabel, freezeLabel, healLabel;

    readonly List<QuizQuestion> questions = new List<QuizQuestion>();
    readonly List<QuizQuestion> bossQuestions = new List<QuizQuestion>();
    Phase phase;
    QuizQuestion currentQuestion;
    string[] currentOptions;
    int currentQuestionIndex = -1;
    int playerHp, opponentHp, opponentMaxHp, correctAnswers, misses, combo;
    int hintsLeft, freezesLeft, healsLeft;
    int levelIndex;
    float questionTime, questionTimeMax;
    bool frozen, answerLocked;

    void Awake()
    {
        BindSceneObjects();
        LoadLevelData();
        BindButtons();
        ShowTutorial(0);
    }

    void Update()
    {
        if ((phase != Phase.Enemy && phase != Phase.Boss) || frozen || answerLocked)
            return;

        questionTime = Mathf.Max(0, questionTime - Time.deltaTime);
        if (levelTimer != null)
            levelTimer.value = questionTimeMax > 0 ? questionTime / questionTimeMax : 0;

        if (questionTime <= 0)
            StartCoroutine(ResolveAnswer(-1));
    }

    void BindSceneObjects()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = FindAnyObjectByType<Canvas>();
        Transform root = canvas != null ? canvas.transform : transform;
        RectTransform canvasRect = root as RectTransform;
        if (canvasRect != null && canvasRect.localScale.sqrMagnitude < 0.0001f)
            canvasRect.localScale = Vector3.one;

        if (tutorialPanels == null || tutorialPanels.Length != 3)
            tutorialPanels = new GameObject[3];
        tutorialPanels[0] = tutorialPanels[0] != null ? tutorialPanels[0] : FindChild(root, "TutorialOnePanel")?.gameObject;
        tutorialPanels[1] = tutorialPanels[1] != null ? tutorialPanels[1] : FindChild(root, "TutorialTwoPanel")?.gameObject;
        tutorialPanels[2] = tutorialPanels[2] != null ? tutorialPanels[2] : FindChild(root, "TutorialThreePanel")?.gameObject;
        tutorialText = new TMP_Text[3];
        tutorialCode = new TMP_Text[3];
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            Transform panel = tutorialPanels[i] != null ? tutorialPanels[i].transform : null;
            tutorialText[i] = FindComponent<TMP_Text>(panel, "TutorialText");
            tutorialCode[i] = FindComponent<TMP_Text>(panel, "CodeDemo");
        }
        gamePanel = gamePanel != null ? gamePanel : FindChild(root, "GamePanel")?.gameObject;
        exitButton = exitButton != null ? exitButton : FindComponent<Button>(root, "ExitButton");
        endOfLevelPanel = endOfLevelPanel != null ? endOfLevelPanel : FindChild(root, "EndofLevel")?.gameObject;
        endMessage = endMessage != null ? endMessage : FindComponent<TMP_Text>(root, "WinorLose", "EndofLevel");
        correctAnswersText = correctAnswersText != null ? correctAnswersText : FindComponent<TMP_Text>(root, "CorrectAnswers", "EndofLevel");
        wrongAnswersText = wrongAnswersText != null ? wrongAnswersText : FindComponent<TMP_Text>(root, "WrongAnswers", "EndofLevel");
        endNextButton = endNextButton != null ? endNextButton : FindComponent<Button>(root, "NextLevel", "EndofLevel");
        endNextText = endNextText != null ? endNextText : FindComponent<TMP_Text>(root, "NextText", "EndofLevel");

        playerHealthBar = playerHealthBar != null ? playerHealthBar : FindComponent<Slider>(root, "HealthBar");
        enemyHealthBar = enemyHealthBar != null ? enemyHealthBar : FindComponent<Slider>(root, "EnemyHealthBar");
        levelTimer = levelTimer != null ? levelTimer : FindComponent<Slider>(root, "LevelTimer");
        questionText = questionText != null ? questionText : FindComponent<TMP_Text>(root, "QuestionText");
        codeText = codeText != null ? codeText : FindComponent<TMP_Text>(root, "CodeDemo", "Question&Answers");
        enemyNameText = enemyNameText != null ? enemyNameText : FindComponent<TMP_Text>(root, "LevelNumber");

        optionButtons = new Button[4];
        string[] names = { "Option1Button", "Option2Button", "Option3Button", "Option4Button" };
        for (int i = 0; i < names.Length; i++)
            optionButtons[i] = FindComponent<Button>(root, names[i]);

        hintButton = hintButton != null ? hintButton : FindComponent<Button>(root, "HintButton");
        freezeButton = freezeButton != null ? freezeButton : FindComponent<Button>(root, "FreezeButton");
        healButton = healButton != null ? healButton : FindComponent<Button>(root, "HealButton");
        hintLabel = hintLabel != null ? hintLabel : FindComponent<TMP_Text>(root, "HintText");
        freezeLabel = freezeLabel != null ? freezeLabel : FindComponent<TMP_Text>(root, "FreezeText");
        healLabel = healLabel != null ? healLabel : FindComponent<TMP_Text>(root, "HealText");

        ValidateReference(gamePanel, "GamePanel");
        ValidateReference(playerHealthBar, "HealthBar slider");
        ValidateReference(enemyHealthBar, "EnemyHealthBar slider");
        ValidateReference(levelTimer, "LevelTimer slider");
        ValidateReference(questionText, "QuestionText");
        ValidateReference(codeText, "CodeDemo under Question&Answers");
        for (int i = 0; i < optionButtons.Length; i++) ValidateReference(optionButtons[i], "Option" + (i + 1) + "Button");
    }

    void BindButtons()
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            GameObject panel = tutorialPanels[i];
            if (panel == null) continue;
            Button next = FindComponent<Button>(panel.transform, "NextButton");
            if (next != null)
            {
                int nextIndex = i + 1;
                next.onClick.AddListener(() => ShowTutorial(nextIndex));
            }

            Button battle = FindComponent<Button>(panel.transform, "BattleButton");
            if (battle != null) battle.onClick.AddListener(StartEnemyBattle);
        }

        if (exitButton != null) exitButton.onClick.AddListener(ExitToMenu);
        if (endNextButton != null) endNextButton.onClick.AddListener(OnEndNext);
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int answerIndex = i;
            if (optionButtons[i] != null) optionButtons[i].onClick.AddListener(() => SubmitAnswer(answerIndex));
        }
        if (hintButton != null) hintButton.onClick.AddListener(UseHint);
        if (freezeButton != null) freezeButton.onClick.AddListener(UseFreeze);
        if (healButton != null) healButton.onClick.AddListener(UseHeal);
    }

    void ShowTutorial(int index)
    {
        if (index < 0 || index >= tutorialPanels.Length || tutorialPanels[index] == null)
        {
            StartEnemyBattle();
            return;
        }

        phase = Phase.Tutorial;
        if (endOfLevelPanel != null) endOfLevelPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        for (int i = 0; i < tutorialPanels.Length; i++)
            if (tutorialPanels[i] != null) tutorialPanels[i].SetActive(i == index);
        if (worldData != null && worldData.lessons != null && index < worldData.lessons.Length)
        {
            LessonPage lesson = worldData.lessons[index];
            if (tutorialText[index] != null && !string.IsNullOrWhiteSpace(lesson.text)) tutorialText[index].text = lesson.text;
            if (tutorialCode[index] != null && !string.IsNullOrWhiteSpace(lesson.code)) tutorialCode[index].text = lesson.code;
        }
    }

    void StartEnemyBattle()
    {
        phase = Phase.Enemy;
        playerHp = worldData != null ? Mathf.Max(1, worldData.playerHP) : 100;
        opponentMaxHp = opponentHp = worldData != null ? Mathf.Max(1, worldData.battleHP) : 100;
        correctAnswers = misses = combo = 0;
        hintsLeft = 2;
        freezesLeft = healsLeft = 1;
        ShowBattleUI();
        NextQuestion();
    }

    void StartBossBattle()
    {
        phase = Phase.Boss;
        opponentMaxHp = opponentHp = worldData != null ? Mathf.Max(1, worldData.bossHP) : 160;
        combo = 0;
        hintsLeft = 2;
        freezesLeft = healsLeft = 1;
        ShowBattleUI();
        NextQuestion();
    }

    void ShowBattleUI()
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
            if (tutorialPanels[i] != null) tutorialPanels[i].SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        if (playerHealthBar != null) playerHealthBar.interactable = false;
        if (enemyHealthBar != null) enemyHealthBar.interactable = false;
        if (levelTimer != null) levelTimer.interactable = false;
        RefreshBars();
        RefreshPowerUpLabels();
    }

    void NextQuestion()
    {
        answerLocked = false;
        frozen = false;
        List<QuizQuestion> pool = phase == Phase.Boss && bossQuestions.Count > 0 ? bossQuestions : questions;
        if (pool.Count == 0)
        {
            Debug.LogError("This WorldData asset has no questions assigned.", this);
            return;
        }
        int next = Random.Range(0, pool.Count);
        if (pool.Count > 1 && next == currentQuestionIndex) next = (next + 1) % pool.Count;
        currentQuestionIndex = next;
        currentQuestion = pool[currentQuestionIndex];
        currentOptions = currentQuestion.wrong.OrderBy(_ => Random.value).Take(3)
            .Append(currentQuestion.answer).OrderBy(_ => Random.value).ToArray();

        questionText.text = currentQuestion.prompt;
        codeText.text = currentQuestion.code;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            Button button = optionButtons[i];
            if (button == null) continue;
            button.gameObject.SetActive(i < currentOptions.Length);
            button.interactable = true;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = currentOptions[i];
        }

        questionTimeMax = phase == Phase.Boss
            ? (worldData != null ? worldData.bossQuestionTime : 10f)
            : (worldData != null ? worldData.questionTime : 15f);
        questionTime = questionTimeMax;
        if (levelTimer != null) levelTimer.value = 1f;
        RefreshPowerUpLabels();
    }

    void SubmitAnswer(int index)
    {
        if (phase != Phase.Enemy && phase != Phase.Boss) return;
        if (answerLocked || currentOptions == null || index < 0 || index >= currentOptions.Length) return;
        StartCoroutine(ResolveAnswer(index));
    }

    IEnumerator ResolveAnswer(int index)
    {
        if (answerLocked) yield break;
        answerLocked = true;
        bool correct = index >= 0 && currentOptions[index] == currentQuestion.answer;
        if (correct)
        {
            correctAnswers++;
            combo++;
            int baseDamage = worldData != null ? worldData.correctDamage : 25;
            int comboDamage = worldData != null ? worldData.comboBonusDamage : 5;
            int damage = baseDamage + Mathf.Min(combo - 1, 3) * comboDamage;
            opponentHp = Mathf.Max(0, opponentHp - damage);
            questionText.text = "Correct! " + currentQuestion.explanation;
            if (phase == Phase.Enemy && opponentHp <= 0)
            {
                RefreshBars();
                yield return new WaitForSeconds(0.7f);
                StartBossBattle();
                yield break;
            }
            if (phase == Phase.Boss && opponentHp <= 0)
            {
                RefreshBars();
                phase = Phase.Victory;
                ShowEndOfLevel(true);
                yield break;
            }
        }
        else
        {
            misses++;
            combo = 0;
            int damageTaken = phase == Phase.Boss
                ? (worldData != null ? worldData.bossWrongDamage : 25)
                : (worldData != null ? worldData.wrongDamage : 15);
            playerHp = Mathf.Max(0, playerHp - damageTaken);
            questionText.text = index < 0
                ? "Time's up! " + currentQuestion.explanation
                : "Answer: " + currentQuestion.answer + ". " + currentQuestion.explanation;
            if (playerHp <= 0)
            {
                RefreshBars();
                phase = Phase.Defeat;
                ShowEndOfLevel(false);
                yield break;
            }
        }

        RefreshBars();
        yield return new WaitForSeconds(0.65f);
        NextQuestion();
    }

    void UseHint()
    {
        if (answerLocked || hintsLeft <= 0 || currentOptions == null) return;
        hintsLeft--;
        int hidden = 0;
        for (int i = 0; i < optionButtons.Length && hidden < 2; i++)
        {
            if (currentOptions[i] == currentQuestion.answer) continue;
            optionButtons[i].gameObject.SetActive(false);
            hidden++;
        }
        RefreshPowerUpLabels();
    }

    void UseFreeze()
    {
        if (answerLocked || freezesLeft <= 0) return;
        freezesLeft--;
        frozen = true;
        RefreshPowerUpLabels();
    }

    void UseHeal()
    {
        int maxPlayerHP = worldData != null ? Mathf.Max(1, worldData.playerHP) : 100;
        if (answerLocked || healsLeft <= 0 || playerHp >= maxPlayerHP) return;
        healsLeft--;
        playerHp = Mathf.Min(maxPlayerHP, playerHp + 30);
        RefreshBars();
        RefreshPowerUpLabels();
    }

    void RefreshBars()
    {
        int maxPlayerHP = worldData != null ? Mathf.Max(1, worldData.playerHP) : 100;
        if (playerHealthBar != null) playerHealthBar.value = Mathf.Clamp01((float)playerHp / maxPlayerHP);
        if (enemyHealthBar != null) enemyHealthBar.value = opponentMaxHp > 0 ? Mathf.Clamp01((float)opponentHp / opponentMaxHp) : 0;
    }

    void RefreshPowerUpLabels()
    {
        if (hintLabel != null) hintLabel.text = "Hint x" + hintsLeft;
        if (freezeLabel != null) freezeLabel.text = frozen ? "Frozen" : "Freeze x" + freezesLeft;
        if (healLabel != null) healLabel.text = "Heal x" + healsLeft;
        if (hintButton != null) hintButton.interactable = hintsLeft > 0 && !answerLocked;
        if (freezeButton != null) freezeButton.interactable = freezesLeft > 0 && !answerLocked;
        int maxPlayerHP = worldData != null ? Mathf.Max(1, worldData.playerHP) : 100;
        if (healButton != null) healButton.interactable = healsLeft > 0 && playerHp < maxPlayerHP && !answerLocked;
    }

    void DisableBattleControls()
    {
        foreach (Button button in optionButtons) if (button != null) button.interactable = false;
        if (hintButton != null) hintButton.interactable = false;
        if (freezeButton != null) freezeButton.interactable = false;
        if (healButton != null) healButton.interactable = false;
        if (levelTimer != null) levelTimer.value = 0;
    }

    void ShowEndOfLevel(bool won)
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
            if (tutorialPanels[i] != null) tutorialPanels[i].SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (endOfLevelPanel != null) endOfLevelPanel.SetActive(true);
        if (endMessage != null) endMessage.text = won ? "You Win!" : "You Lose...";
        if (correctAnswersText != null) correctAnswersText.text = "Correct Answers: " + correctAnswers;
        if (wrongAnswersText != null) wrongAnswersText.text = "Wrong Answers: " + misses;
        if (endNextText != null) endNextText.text = won ? "Next" : "Retry";
        if (endNextButton != null) endNextButton.interactable = true;
        DisableBattleControls();
    }

    void OnEndNext()
    {
        if (phase == Phase.Defeat)
        {
            currentQuestionIndex = -1;
            correctAnswers = misses = combo = 0;
            playerHp = worldData != null ? Mathf.Max(1, worldData.playerHP) : 100;
            ShowTutorial(0);
            return;
        }

        if (phase != Phase.Victory) return;
        string nextScene = worldData != null && !string.IsNullOrWhiteSpace(worldData.nextScene)
            ? worldData.nextScene
            : "MainMenu";
        SceneManager.LoadScene(nextScene);
    }

    void ExitToMenu() => SceneManager.LoadScene("MainMenu");

    void LoadLevelData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (worldData == null) worldData = Resources.Load<WorldData>("Levels/" + sceneName);
        if (worldData == null)
        {
            Debug.LogError("No WorldData asset assigned or found at Resources/Levels/" + sceneName + ".", this);
            return;
        }

        levelIndex = worldData.levelIndex;
        questions.Clear();
        bossQuestions.Clear();
        if (worldData.questions != null)
        {
            foreach (Question q in worldData.questions)
                if (q != null) questions.Add(ToRuntimeQuestion(q));
        }
        if (worldData.bossQuestions != null)
            foreach (Question q in worldData.bossQuestions)
                if (q != null) bossQuestions.Add(ToRuntimeQuestion(q));
    }

    static QuizQuestion ToRuntimeQuestion(Question question)
    {
        return new QuizQuestion(
            question.prompt ?? string.Empty,
            question.code ?? string.Empty,
            question.answer ?? string.Empty,
            question.explanation ?? string.Empty,
            question.wrong != null ? question.wrong.ToArray() : new string[0]);
    }

    static Transform FindChild(Transform root, string objectName)
    {
        if (root == null) return null;
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == objectName) return child;
        return null;
    }

    static T FindComponent<T>(Transform root, string objectName, string requiredAncestor = null) where T : Component
    {
        if (root == null) return null;
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name != objectName) continue;
            Transform parent = child.parent;
            if (requiredAncestor != null)
            {
                while (parent != null && parent.name != requiredAncestor) parent = parent.parent;
                if (parent == null) continue;
            }
            T component = child.GetComponent<T>();
            if (component != null) return component;
        }
        return null;
    }

    static void ValidateReference(Object reference, string label)
    {
        if (reference == null) Debug.LogError("LevelOne is missing " + label + ". Check the existing scene object's name and component.");
    }
}
