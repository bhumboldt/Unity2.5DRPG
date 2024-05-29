using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private PartyManager partyManager;
    private EnemyManager enemyManager;
    
    [SerializeField] private List<BattleEntities> allEntities = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerEntities = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyEntities = new List<BattleEntities>();
    
    // Start is called before the first frame update
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
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
