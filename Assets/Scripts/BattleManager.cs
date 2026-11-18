using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [SerializeField] Image playerBar, monsterBar, timerBar;
    [SerializeField] TMP_Text tagText, promptText, codeText, feedbackText, comboText;
    [SerializeField] Button[] optionButtons;
    [SerializeField] GameObject nextButton;

    WorldData world; bool boss;
    int monsterHP, monsterMax, playerHP, combo, miss, correct;
    int hints = 2, freezes = 1, heals = 1;
    Question q; string[] opts;
    float time, timeMax; bool locked, frozen;
    Func<Question> lastGen;

    void Start()
    {
        world = GameSession.CurrentWorld;
        boss = GameSession.IsBoss;
        if (world == null)
        {
            Debug.LogError("No world is selected. Call GameSession.StartWorld before loading the battle scene.", this);
            enabled = false;
            return;
        }
        monsterMax = monsterHP = boss ? world.bossHP : world.battleHP;
        if (monsterMax <= 0) monsterMax = monsterHP = boss ? 120 : 100;
        playerHP = 100;
        NextQuestion();
    }

    void NextQuestion()
    {
        locked = false; frozen = false; feedbackText.text = ""; nextButton.SetActive(false);

        // Boss fights mix in review questions from earlier worlds.
        int w = GameSession.WorldIndex;
        var pool = WorldGenerators.All[w];
        if (boss && w > 0 && UnityEngine.Random.value < 0.4f)
            pool = WorldGenerators.All[UnityEngine.Random.Range(0, w)];

        Func<Question> gen;
        do { gen = pool[UnityEngine.Random.Range(0, pool.Length)]; }
        while (gen == lastGen && pool.Length > 1);
        lastGen = gen;

        q = gen();
        opts = q.wrong.OrderBy(_ => UnityEngine.Random.value).Take(3)
                .Append(q.answer).OrderBy(_ => UnityEngine.Random.value).ToArray();

        tagText.text = q.tag; promptText.text = q.prompt; codeText.text = q.code;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            bool used = i < opts.Length;
            optionButtons[i].gameObject.SetActive(used);
            optionButtons[i].interactable = true;
            if (used) optionButtons[i].GetComponentInChildren<TMP_Text>().text = opts[i];
            optionButtons[i].image.color = Color.white;
        }
        timeMax = time = boss ? 10f : 15f;
        RefreshUI();
    }

    void Update()
    {
        if (locked || frozen || !GameSession.Save.timer) return;
        time -= Time.deltaTime;
        timerBar.fillAmount = time / timeMax;
        if (time <= 0) Answer(-1);   // timeout counts as wrong
    }

    // Hook each option button's OnClick to Answer(0..3).
    public void Answer(int i)
    {
        if (locked || i < -1 || i >= opts.Length) return;
        locked = true;
        bool ok = i >= 0 && opts[i] == q.answer;

        if (ok)
        {
            correct++; combo++;
            int dmg = 20 + Mathf.Min(combo - 1, 3) * 5;
            monsterHP -= dmg;
            feedbackText.text = $"Hit! -{dmg}" + (combo >= 3 ? $"  COMBO x{combo}!" : "");
            RefreshUI();
            StartCoroutine(After(monsterHP <= 0 ? 0.7f : 0.85f, monsterHP <= 0 ? Win : NextQuestion));
        }
        else
        {
            miss++; combo = 0;
            playerHP -= boss ? 25 : 20;
            feedbackText.text = $"Answer: {q.answer}\n{q.explanation}";
            nextButton.SetActive(true);   // Next button calls OnNext()
            RefreshUI();
        }
    }

    public void OnNext() { if (playerHP <= 0) Lose(); else NextQuestion(); }

    // Spells (hook up to the three spell buttons)
    public void Hint()
    {
        if (locked || hints <= 0) return; hints--;
        int hidden = 0;
        for (int i = 0; i < opts.Length && hidden < 2; i++)
            if (opts[i] != q.answer) { optionButtons[i].gameObject.SetActive(false); hidden++; }
    }
    public void Freeze() { if (!locked && freezes > 0) { freezes--; frozen = true; } }
    public void Heal()   { if (!locked && heals > 0)   { heals--; playerHP = Mathf.Min(100, playerHP + 30); RefreshUI(); } }

    void RefreshUI()
    {
        playerBar.fillAmount  = Mathf.Max(0, playerHP) / 100f;
        monsterBar.fillAmount = Mathf.Max(0, monsterHP) / (float)monsterMax;
        comboText.text = combo > 1 ? $"x{combo}" : "";
    }

    IEnumerator After(float s, Action a) { yield return new WaitForSeconds(s); a(); }

    void Win()
    {
        if (!boss) { GameSession.IsBoss = true; GameSession.AddResults(correct, miss); /* show Boss intro, then reload */ return; }
        GameSession.AddResults(correct, miss);
        GameSession.CompleteWorld();   // stars, XP, save, then show result + ad
    }
    void Lose() { /* show crash screen: Retry / Review lesson / Map */ }
}
