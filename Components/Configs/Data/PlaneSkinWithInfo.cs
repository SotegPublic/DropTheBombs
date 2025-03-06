using HECSFramework.Serialize;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[JsonObject]
public partial class PlaneSkinWithInfo
{
    [JsonProperty(nameof(SkinID))]
    public int SkinID;
    [JsonProperty(nameof(PlaneID))]
    public int PlaneID;
    [JsonProperty(nameof(isSkinUnlocked))]
    private bool isSkinUnlocked;
    [JsonProperty(nameof(isSkinOwned))]
    private bool isSkinOwned;
    [JsonProperty(nameof(isSkinInstalled))]
    private bool isSkinInstalled;
    [JsonProperty(nameof(isSkinNew))]
    private bool isSkinNew;
    [JsonProperty(nameof(skinTextureType))]
    private ItemsTypes skinTextureType;

    public bool IsUnlocked => isSkinUnlocked;
    public bool IsOwned => isSkinOwned;
    public bool IsInstalled => isSkinInstalled;
    public bool IsNew => isSkinNew;
    public ItemsTypes TextureType => skinTextureType;

    public PlaneSkinWithInfo()
    {

    }


    public PlaneSkinWithInfo(int skinID, int planeID, bool isUnlocked, bool isOwned, ItemsTypes textureType)
    {
        SkinID = skinID;
        PlaneID = planeID;
        isSkinUnlocked = isUnlocked;
        isSkinOwned = isOwned;
        skinTextureType = textureType;
    }

    public void SetSkinIsUnlocked()
    {
        isSkinUnlocked = true;
        isSkinNew = true;
    }

    public void SetSkinIsOwned()
    {
        isSkinOwned = true;
    }

    public void SetSkinIsInstalled(bool status)
    {
        isSkinInstalled = status;
    }

    public void SetSkinIsSelected()
    {
        isSkinNew = false;
    }
}
