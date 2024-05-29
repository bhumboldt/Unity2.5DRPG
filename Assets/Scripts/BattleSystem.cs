using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
    
    [Header("Entities")]
    [SerializeField] private List<BattleEntities> allEntities = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerEntities = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyEntities = new List<BattleEntities>();
    
    private PartyManager partyManager;
    private EnemyManager enemyManager;
    
    // Start is called before the first frame update
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        
        CreatePartyEntities();
        CreateEnemyEntities();
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
}

[System.Serializable]
public class BattleEntities
{
    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int Strength;
    public int Initiative;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;

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
}
