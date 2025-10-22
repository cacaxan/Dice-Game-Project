using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Entities/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string ID;                     // Identificador único
    public string characterName;          // Nombre visible
    [TextArea] public string description; // Breve descripción

    [Header("Stats")]
    public int maxHealth = 20;            // Vida máxima
    public int baseDefense = 0;           // Defensa inicial
    public DiceData dice;                 // Dados que usa el personaje
    public int maxRerolls = 2;            // Máximo de rerolls por turno
    public int dicePerTurn = 3;           // Número de dados que tira por turno
    public RarityType rarity = RarityType.Common; // Rareza o nivel del personaje

    [Header("Visuals")]
    public Sprite characterSprite;        // Imagen o icono del personaje
    public GameObject prefab;             // Prefab de juego para instanciar en batalla

    [Header("Rewards / Roguelike Extras")]
    public int goldReward = 0;            // Oro que da al derrotarlo
    public int expReward = 0;             // Experiencia que da al derrotarlo
    public List<AbilityData> specialAbilities; // Habilidades especiales
    public List<EffectParams> startingBuffs;   // Buffs que inicia la batalla con ellos
}

// Extra enums y structs
public enum RarityType { Common, Rare, Epic, Legendary }

[System.Serializable]
public class AbilityData
{
    public string abilityName;
    [TextArea] public string description;
    public DiceFaceType type; // Tipo de habilidad: Attack, Buff, etc.
    public int powerValue;
    public EffectParams effectParams;
}
