using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject
{
    public string EnemyName;
    public int BaseHealth;
    public int BaseStrength;
    public int BaseInitiative;
    public GameObject EnemyVisualPrefab; // what will be displayed in battle
}