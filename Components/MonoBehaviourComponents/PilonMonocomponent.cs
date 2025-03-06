using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PilonMonocomponent : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public Canvas Canvas;
    public GameObject HealthPanel;
    public TMP_Text HealthText;
    public Image HealthImage;
    public TMP_Text xText;
    public RectTransform GainedCurrencyRect;
    public TMP_Text GainedCurrencyText;

    public Transform[] TargetTransforms;

    public bool IsBoss;
    [HideIf(nameof(IsBoss))] public GameObject Building;
    [HideIf(nameof(IsBoss))] public GameObject DestroedBuilding;
}
