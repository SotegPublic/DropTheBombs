using HECSFramework.Unity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = nameof(LevelConfig), menuName = "CustomConfigs/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private AssetReference levelPrefReference;
    
    [Space]
    [SerializeField] private float intervalBetweenRingsHorizontal = 90;
    [SerializeField] private float intervalBetweenRingsVertical = 50;

    [Space]
    [SerializeField] private List<SpawnPointConfig> flySpawnPoints = new List<SpawnPointConfig>();
    [Space]
    [SerializeField] private List<SpawnPointConfig> fallSpawnPoints = new List<SpawnPointConfig>();

    public AssetReference LevelPrefReference => levelPrefReference;
    public float IntervalBetweenRingsOrTrapsHorizontal => intervalBetweenRingsHorizontal;
    public float IntervalBetweenRingsOrTrapsVertical => intervalBetweenRingsVertical;
    public List<SpawnPointConfig> FlySpawnPoints => flySpawnPoints;
    public List<SpawnPointConfig> FallSpawnPoints => fallSpawnPoints;
}


