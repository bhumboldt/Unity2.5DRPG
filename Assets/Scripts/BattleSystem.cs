using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BattleSystem : MonoBehaviour
{
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
        
        AttackAction(allEntities[0], allEntities[1]);
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
            // Start the battle
            Debug.Log("Start the battle");
            Debug.Log("We are attacking " + allEntities[currPlayerEntity.Target].Name);
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
