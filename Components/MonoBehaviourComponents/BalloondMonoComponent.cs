using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalloondMonoComponent : MonoBehaviour
{
    [SerializeField] private Image healthFiller;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void ChangeFillAmount(float amount)
    {
        if (canvasGroup.alpha == 0f)
        {
            canvasGroup.alpha = 1f;
        }

        healthFiller.fillAmount = amount;
    }

    public void ResetBaloon() 
    {
        canvasGroup.alpha = 0f;
        healthFiller.fillAmount = 1;
    }
}
