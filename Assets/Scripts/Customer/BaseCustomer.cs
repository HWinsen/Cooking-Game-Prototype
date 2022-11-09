using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.Customer
{
    public abstract class BaseCustomer : PoolObject
    {
        public delegate void CustomerDelegate(BaseCustomer baseCustomer, Transform destination);
        public static event CustomerDelegate OnFinishedOrdered;

        public float Patience = 10f;
        [SerializeField] protected float GoldMultiplier;
        [SerializeField] protected float MoveSpeed = 2f;
        protected Transform Destination;
        protected Vector3 DestinationPosition;
        protected Vector3[] SpawnPoint =
        {
            new Vector3(-12, 0, 0),
            new Vector3(12, 0, 0),
        };
        protected int RandomSpawnIndex;
        protected bool Ordered = false;

        public void SetDestination(Transform destination)
        {
            Destination = destination;
            DestinationPosition = Destination.position;
        }

        protected virtual void CustomerBehavior()
        {
            if (Ordered == false && transform.position != DestinationPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, DestinationPosition, MoveSpeed * Time.deltaTime);
            }
            else
            {
                Ordered = true;
                Patience -= Time.deltaTime;
                if (Patience <= 0f)
                {
                    if (transform.position != SpawnPoint[RandomSpawnIndex])
                    {
                        transform.position = Vector2.MoveTowards(transform.position, SpawnPoint[RandomSpawnIndex], MoveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        StoreToPool();
                        OnFinishedOrdered(this, Destination);
                    }
                }
            }
        }
    }
}