using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;
    
    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        GenerateEnemyByName("Slime", 1);
        GenerateEnemyByName("Slime", 1);
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
