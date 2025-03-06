using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, "Bombs Pool")]
    public sealed class BombsPoolComponent : BaseComponent, IWorldSingleComponent
    {
        private Stack<GameObject> bombs = new Stack<GameObject>(81);

        public void AddBomb(GameObject view)
        {
            bombs.Push(view);
        }

        public GameObject GetBomb()
        {
            if (bombs.Count == 1)
            {
                return MonoBehaviour.Instantiate(bombs.Peek());
            }

            return bombs.Pop();
        }

        public void ReturnBomb(Actor bomb)
        {
            bomb.gameObject.layer = LayerMask.NameToLayer("Hiden");
            bomb.transform.parent = null;
            bomb.transform.position = Vector3.zero;
            bomb.transform.rotation = Quaternion.identity;
            var id = bomb.Entity.GetComponent<BombTagComponent>().BombID;
            bombs.Push(bomb.gameObject);
            bomb.Dispose();
        }
    }
}