using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.ObjectPooling
{
    public abstract class PoolObject : MonoBehaviour, IPoolObject
    {
        public PoolingSystem Pooling { private set; get; }
        void IPoolObject.Initial(PoolingSystem poolSystem)
        {
            Pooling = poolSystem;
        }

        public virtual void StoreToPool()
        {
            Pooling.Store(this);
            gameObject.SetActive(false);
        }
    }
}