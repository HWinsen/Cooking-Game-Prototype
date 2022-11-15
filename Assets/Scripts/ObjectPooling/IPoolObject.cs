using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.ObjectPooling
{
    public interface IPoolObject
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }
        PoolingSystem Pooling { get; }

        void Initial(PoolingSystem poolSystem);
        
        void StoreToPool();
    }
}