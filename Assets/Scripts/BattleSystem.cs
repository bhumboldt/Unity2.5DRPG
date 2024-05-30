using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private enum BattleState
    {
        Start,
        Selection,
        Battle,
        Won,
        Lost,
        Run
    }

    [Header("Battle State")]
    [SerializeField] private BattleState state;
    
    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
    
    [Header("Entities")]
    [SerializeField] private List<BattleEntities> allEntities = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerEntities = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyEntities = new List<BattleEntities>();

    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI battleText;
    [SerializeField] private GameObject bottomTextPopup;
    [SerializeField] private TextMeshProUGUI bottomText;
    
    private const string ACTION_MESSAGE = "'s Action:";
    private const int TURN_DURATION = 2;
    private const string WIN_MESSAGE = "Your party won the battle!";
    private const string LOSE_MESSAGE = "Your party has been defeated!";
    private const string OVERWORLD_SCENE = "OverworldScene";
    
    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        
        CreatePartyEntities();
        CreateEnemyEntities();

        ShowBattleMenu();
    }

    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);
        state = BattleState.Battle;
        bottomTextPopup.SetActive(true);

        for (int i = 0; i < allEntities.Count; i++)
        {
            if (state == BattleState.Battle)
            {
                switch (allEntities[i].BattleAction)
                {
                    case BattleEntities.Action.Attack:
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case BattleEntities.Action.Run:
                        break;
                    default:
                        break;
                }
            }
        }

        if (state == BattleState.Battle)
        {
            bottomTextPopup.SetActive(false);
            currentPlayer = 0;
            ShowBattleMenu();
        }

        yield return null;
    }

    private IEnumerator AttackRoutine(int idx)
    {
        if (allEntities[idx].IsPlayer)
        {
            BattleEntities currAttacker = allEntities[idx];
            if (allEntities[currAttacker.Target].IsPlayer == true || currAttacker.Target >= allEntities.Count)
            {
                currAttacker.SetTarget(GetRandomEnemy());
            }
            BattleEntities currTarget = allEntities[currAttacker.Target];
            
            AttackAction(currAttacker, currTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if (currTarget.CurrentHealth <= 0)
            {
                bottomText.text = string.Format("{0} has defeated {1}!", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                enemyEntities.Remove(currTarget);
                allEntities.Remove(currTarget);
            }
            
            if (enemyEntities.Count <= 0)
            {
                state = BattleState.Won;
                bottomText.text = WIN_MESSAGE;
                yield return new WaitForSeconds(TURN_DURATION);
                SceneManager.LoadScene(OVERWORLD_SCENE);
            }
        }
        else
        {
            BattleEntities currAttacker = allEntities[idx];
            currAttacker.SetTarget(GetRandomPartyMember());
            BattleEntities currTarget = allEntities[currAttacker.Target];
            
            AttackAction(currAttacker, currTarget);
            yield return new WaitForSeconds(TURN_DURATION);
            
            if (currTarget.CurrentHealth <= 0)
            {
                bottomText.text = string.Format("{0} has defeated {1}!", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                playerEntities.Remove(currTarget);
                allEntities.Remove(currTarget);
            }
            
            if (playerEntities.Count <= 0)
            {
                state = BattleState.Lost;
                bottomText.text = LOSE_MESSAGE;
                yield return new WaitForSeconds(TURN_DURATION);
                Debug.Log("Return to overworld scene or a game over scene");
            }
        }
    }

    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>();
        for (int i = 0; i < allEntities.Count; i++)
        {
            if (allEntities[i].IsPlayer)
            {
                partyMembers.Add(i);
            }
        }
        
        return partyMembers[Random.Range(0, partyMembers.Count)];
    }

    private int GetRandomEnemy()
    {
        List<int> enemies = new List<int>();
        for (int i = 0; i < allEntities.Count; i++)
        {
            if (!allEntities[i].IsPlayer)
            {
                enemies.Add(i);
            }
        }
        return enemies[Random.Range(0, enemies.Count)];
    }

    private void CreatePartyEntities()
    {
        List<PartyMember> partyMembers = partyManager.GetCurrentParty();
        
        for (int i = 0; i < partyMembers.Count; i++)
        {
            var member = partyMembers[i];
            BattleEntities newEntity = new BattleEntities();
            newEntity.SetEntityValues(member.MemberName, member.MaxHealth, member.CurrentHealth, member.Strength, member.Initiative, member.Level, true);
            
            BattleVisuals tempBattleVisuals = Instantiate(member.MemberBattleVisualPrefab, partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();
            
            tempBattleVisuals.SetStartingValues(member.CurrentHealth, member.MaxHealth, member.Level);
            newEntity.BattleVisuals = tempBattleVisuals;
            
            playerEntities.Add(newEntity);
            allEntities.Add(newEntity);
        }
    }
    
    private void CreateEnemyEntities()
    {
        List<Enemy> enemies = enemyManager.GetEnemies();
        
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            BattleEntities newEntity = new BattleEntities();
            newEntity.SetEntityValues(enemy.EnemyName, enemy.MaxHealth, enemy.CurrentHealth, enemy.Strength, enemy.Initiative, enemy.Level, false);
            
            BattleVisuals tempBattleVisuals = Instantiate(enemy.EnemyVisualPrefab, enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();
            
            tempBattleVisuals.SetStartingValues(enemy.CurrentHealth, enemy.MaxHealth, enemy.Level);
            newEntity.BattleVisuals = tempBattleVisuals;
            
            enemyEntities.Add(newEntity);
            allEntities.Add(newEntity);
        }
    }

    public void ShowBattleMenu()
    {
        battleText.text = playerEntities[currentPlayer].Name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }

    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        foreach (var button in enemySelectionButtons)
        {
            button.SetActive(false);
        }
        
        for (int i = 0; i < enemyEntities.Count; i++)
        {
            enemySelectionButtons[i].SetActive(true);
            enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyEntities[i].Name;
        }
    }
    
    public void SelectEnemy(int enemyIndex)
    {
        BattleEntities currPlayerEntity = playerEntities[currentPlayer];
        currPlayerEntity.BattleAction = BattleEntities.Action.Attack;
        currPlayerEntity.SetTarget(allEntities.IndexOf(enemyEntities[enemyIndex]));

        currentPlayer++;

        if (currentPlayer >= playerEntities.Count)
        {
            StartCoroutine(BattleRoutine());
        }
        else
        {
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }

    private void AttackAction(BattleEntities currentAttacker, BattleEntities currentTarget)
    {
        int damage = currentAttacker.Strength;
        currentAttacker.BattleVisuals.PlayAttackAnimation();
        currentTarget.CurrentHealth -= damage;
        currentTarget.BattleVisuals.PlayHitAnimation();
        currentTarget.UpdateUI();
        
        bottomText.text = string.Format("{0} attacked {1} for {2} damage!", currentAttacker.Name, currentTarget.Name, damage);
        bottomTextPopup.SetActive(true);
        
        SaveHealth();
    }

    private void SaveHealth()
    {
        for (int i = 0; i < playerEntities.Count; i++)
        {
            partyManager.SaveHealth(i, playerEntities[i].CurrentHealth);
        }
    }
}

[System.Serializable]
public class BattleEntities
{
    public enum Action
    {
        Attack,
        Run
    }
    public Action BattleAction;
    
    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int Strength;
    public int Initiative;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;
    public int Target;

    public void SetEntityValues(string name, int maxHealth, int currentHealth, int strength, int initiative, int level, bool isPlayer)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Strength = strength;
        Initiative = initiative;
        Level = level;
        IsPlayer = isPlayer;
    }

    public void SetTarget(int target)
    {
        Target = target;
    }

    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurrentHealth);
    }
}
