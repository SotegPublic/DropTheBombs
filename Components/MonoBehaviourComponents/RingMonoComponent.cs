using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class RingMonoComponent : MonoBehaviour
{
    public TMP_Text ValueText;
    public TMP_Text TypeText;

    public bool IsBad;
    [ShowIf("IsBad")] public GameObject CurrentGO;
    [ShowIf("IsBad")] public GameObject CountGO;
    [ShowIf("IsBad")] public GameObject PowerGO;
}
