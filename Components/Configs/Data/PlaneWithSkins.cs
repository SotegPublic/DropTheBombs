using Components;
using HECSFramework.Serialize;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[JsonObject]
public partial class PlaneWithSkins
{
    [JsonProperty(nameof(PlaneID))]
    public int PlaneID;

    [JsonProperty(nameof(Skins))]
    public List<PlaneSkinWithInfo> Skins;

    [JsonProperty(nameof(isPlaneUnlocked))]
    private bool isPlaneUnlocked;
    [JsonProperty(nameof(isPlaneOwned))]
    private bool isPlaneOwned;
    [JsonProperty(nameof(isPlaneInstalled))]
    private bool isPlaneInstalled;
    [JsonProperty(nameof(isPlaneNew))]
    private bool isPlaneNew;

    public bool IsUnlocked => isPlaneUnlocked;
    public bool IsOwned => isPlaneOwned;
    public bool IsInstalled => isPlaneInstalled;
    public bool IsNew => isPlaneNew;

    public PlaneWithSkins() { }

    public PlaneWithSkins(int planeID, List<PlaneTextureConfig> skinsConfigs)
    {
        PlaneID = planeID;

        isPlaneUnlocked = false;
        isPlaneOwned = false;
        isPlaneInstalled = false;
        isPlaneNew = false;

        if(skinsConfigs != null)
        {
            Skins = new List<PlaneSkinWithInfo>(skinsConfigs.Count);

            foreach (var skinConfig in skinsConfigs)
            {
                var skinWithinfo = new PlaneSkinWithInfo(skinConfig.TextureID, planeID, false, false, skinConfig.ItemType);
                Skins.Add(skinWithinfo);
            }
        }
        else
        {
            Skins = new List<PlaneSkinWithInfo>(1)
            {
                new PlaneSkinWithInfo(PlanePaintIdentifierMap.Base, planeID, true, true, ItemsTypes.None)
            };
        }
    }

    public void SetPlaneUnlocked() 
    {
        isPlaneUnlocked = true;
        isPlaneNew = true;
    }

    public void SetPlaneOwned()
    {
        isPlaneOwned = true;
    }

    public void SetPlaneInstalled(bool status)
    {
        isPlaneInstalled = status;
    }

    public void SetPlaneIsSelected()
    {
        isPlaneNew = false;
    }

    public void SetPlaneIsNew(bool status)
    {
        isPlaneNew = status;
    }
}
