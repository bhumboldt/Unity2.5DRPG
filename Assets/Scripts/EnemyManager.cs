using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private static GameObject instance;
    
    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void GenerateEnemiesByEncounter(Encounter[] encounters, int maxNumEnemies)
    {
        currentEnemies.Clear();

        int numEnemies = Random.Range(1, maxNumEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            Encounter tempEncounter = encounters[Random.Range(0, encounters.Length)];
            int level = Random.Range(tempEncounter.LevelMin, tempEncounter.LevelMax + 1);
            GenerateEnemyByName(tempEncounter.Enemy.EnemyName, level);
        }
    }

    private void GenerateEnemyByName(string name, int level)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            var enemy = allEnemies[i];
            if (enemy.EnemyName == name)
            {
                Enemy newEnemy = new Enemy();
                newEnemy.EnemyName = enemy.EnemyName;
                newEnemy.Level = level;
                float levelModifier = (LEVEL_MODIFIER * newEnemy.Level);
                
                newEnemy.MaxHealth = enemy.BaseHealth + Mathf.RoundToInt(enemy.BaseHealth * levelModifier);
                newEnemy.CurrentHealth = newEnemy.MaxHealth;
                
                newEnemy.Strength = enemy.BaseStrength + Mathf.RoundToInt(enemy.BaseStrength * levelModifier);
                newEnemy.Initiative = enemy.BaseInitiative + Mathf.RoundToInt(enemy.BaseInitiative * levelModifier);
                newEnemy.EnemyVisualPrefab = enemy.EnemyVisualPrefab;
                currentEnemies.Add(newEnemy);
            }
        }
    }
    
    public List<Enemy> GetEnemies()
    {
        return this.currentEnemies;
    }
}

[System.Serializable]
public class Enemy
{
    public string EnemyName;
    public int CurrentHealth;
    public int Level;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public GameObject EnemyVisualPrefab;
}
