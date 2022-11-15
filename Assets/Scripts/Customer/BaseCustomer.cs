using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Harryanto.CookingGame.Customer
{
    public abstract class BaseCustomer : PoolObject
    {
        public delegate void CustomerDelegate(BaseCustomer baseCustomer, Transform destination);
        public static event CustomerDelegate OnFinishedOrdered;

        public delegate void OrderDelegate(BaseCustomer baseCustomer, Transform orderPanel);
        public static event OrderDelegate OnInsertingOrder;
        public static event OrderDelegate OnRemoveAllOrder;

        public float PatienceDuration = 10f;
        public float PatienceTimer { private set; get; }
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
        public GameObject CustomerStatus { protected set; get; }
        public Transform OrderPanel { protected set; get; }
        protected Image PatienceBarInner;

        public void SetDestination(Transform destination)
        {
            Destination = destination;
            DestinationPosition = Destination.position;
        }

        public void SetCustomerStatus(GameObject gameObject)
        {
            CustomerStatus = gameObject;
            if (CustomerStatus != null)
            {
                OrderPanel = CustomerStatus.transform.GetChild(0);
                PatienceBarInner = CustomerStatus.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            }
        }

        protected void SetPatienceTimer()
        {
            PatienceTimer = PatienceDuration;
        }

        protected virtual void CustomerBehavior()
        {
            if (!Ordered && transform.position != DestinationPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, DestinationPosition, MoveSpeed * Time.deltaTime);
            }
            else
            {
                Order();
            }
        }

        protected virtual void Order()
        {
            if (!Ordered)
            {
                OnInsertingOrder(this, OrderPanel);
            }
            Ordered = true;
            CustomerStatus.SetActive(true);
            PatienceBarInner.fillAmount = PatienceTimer / PatienceDuration;
            PatienceTimer -= Time.deltaTime;
            if (PatienceTimer <= 0f)
            {
                PatienceBarInner.fillAmount = 0f;
                OnRemoveAllOrder(this, OrderPanel);
                CustomerStatus.SetActive(false);
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

        public void BoostPatience()
        {
            PatienceTimer = PatienceDuration;
        }

        public void ZeroPatience()
        {
            PatienceTimer = 0f;
        }
    }
}