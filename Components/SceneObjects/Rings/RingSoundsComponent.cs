using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Sound, Doc.Rings, "here we hold rings sounds")]
    public sealed class RingSoundsComponent : BaseComponent
    {
        [SerializeField] private AudioClip positive;
        [SerializeField] private AudioClip negative;
        [SerializeField] private AudioClip hit;

        public AudioClip Positive => positive;
        public AudioClip Negative => negative;
        public AudioClip Hit => hit;
    }
}