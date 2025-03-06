using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

public class PlaneMonocomponent : MonoBehaviour, IChangableColorView
{
    [SerializeField] private MeshRenderer meshRenderer;
    
    [SerializeField] private GameObject shark;
    [SerializeField] private GameObject dragon;
    [SerializeField] private GameObject capitan;
    [SerializeField] private GameObject buzz;
    [SerializeField] private GameObject venom;

    public GameObject Shark => shark;
    public GameObject Dragon => dragon;
    public GameObject Capitan => capitan;
    public GameObject Buzz => buzz;
    public GameObject Venom => venom;

    public void ChangeColor(int paintingID, Texture2D texture)
    {
        meshRenderer.sharedMaterial.SetTexture("_BaseMap", texture);

        switch (paintingID)
        {
            case PlanePaintIdentifierMap.Capitan:
                shark.SetActive(false);
                dragon.SetActive(false);
                capitan.SetActive(true);
                buzz.SetActive(false);
                venom.SetActive(false);
                break;
            case PlanePaintIdentifierMap.Buzz:
                shark.SetActive(false);
                dragon.SetActive(false);
                capitan.SetActive(false);
                buzz.SetActive(true);
                venom.SetActive(false);
                break;
            case PlanePaintIdentifierMap.Dragon:
                shark.SetActive(false);
                dragon.SetActive(true);
                capitan.SetActive(false);
                buzz.SetActive(false);
                venom.SetActive(false);
                break;
            case PlanePaintIdentifierMap.Shark:
                shark.SetActive(true);
                dragon.SetActive(false);
                capitan.SetActive(false);
                buzz.SetActive(false);
                venom.SetActive(false);
                break;
            case PlanePaintIdentifierMap.Venom:
                shark.SetActive(false);
                dragon.SetActive(false);
                capitan.SetActive(false);
                buzz.SetActive(false);
                venom.SetActive(true);
                break;
            default:
                ShutdownRareSkins();
                break;
        }
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;

        ShutdownRareSkins();
    }

    private void ShutdownRareSkins()
    {
        shark.SetActive(false);
        dragon.SetActive(false);
        capitan.SetActive(false);
        buzz.SetActive(false);
        venom.SetActive(false);
    }

    public void SetMaterialWith3DTexture(int paintingID, Material material)
    {
        meshRenderer.material = material;

        ShutdownRareSkins();

        switch (paintingID)
        {
            case PlanePaintIdentifierMap.Capitan:                
                capitan.SetActive(true);               
                break;
            case PlanePaintIdentifierMap.Buzz:               
                buzz.SetActive(true);
                break;
            case PlanePaintIdentifierMap.Dragon:               
                dragon.SetActive(true);                
                break;
            case PlanePaintIdentifierMap.Shark:
                shark.SetActive(true);               
                break;
            case PlanePaintIdentifierMap.Venom:               
                venom.SetActive(true);
                break;           
        }
    }
}
