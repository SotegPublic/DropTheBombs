using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombLevelMonoComponent : MonoBehaviour
{
    [SerializeField] private Transform lastSpawnTransform;
    [SerializeField] private Transform finishTransform;
    [SerializeField] private GameObject bossPilon;

    public Transform LastSpawnTransform => lastSpawnTransform;
    public Transform FinishTransform => finishTransform;
    public GameObject BossPilon => bossPilon;
}
