using UnityEngine;

[CreateAssetMenu(fileName = "GameScriptableObject", menuName = "GameScriptableObject")]
public class GameScriptableObject : ScriptableObject
{
    public float PlayerTurnRate;
    public float PlayerSpeed;
    public float PlayerFireDelayS;
    public float PlayerInvulnerabilityTimeS;
    public float PlayerDriftModifier;
    public int PlayerHealth;
    public float BulletSpeed;
    public int AsteroidsInitCount;
    public int SubAsteroidsCount;
    public float AsteroidMinSpeed;
    public float AsteroidMaxSpeed;
    public float AsteroidSplatterAngle;
    public float EnemyMinSpawnDelayS;
    public float EnemyMaxSpawnDelayS;
    public float EnemyMinFireDelayS;
    public float EnemyMaxFireDelayS;
    public float EnemySpeed;
    public bool isFriendlyFire;
}
