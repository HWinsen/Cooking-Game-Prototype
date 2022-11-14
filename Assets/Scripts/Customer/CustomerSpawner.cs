using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.Customer
{
    public class CustomerSpawner : MonoBehaviour
    {
        public delegate void CustomerSpawnerDelegate(BaseCustomer baseCustomer);
        public static event CustomerSpawnerDelegate OnCustomerSpawned;

        private PoolingSystem _customerPool = new(10);
        [SerializeField] private BaseCustomer[] _customerPrefabList;
        [SerializeField] private int _totalCustomerToSpawn;
        [SerializeField] private float _spawnDuration;
        private Vector3[] _spawnPoint =
        {
            new Vector3(-12, 0, 0),
            new Vector3(12, 0, 0),
        };
        [SerializeField] private Transform[] _stallSlotList;
        [SerializeField] private GameObject[] _customerStatus;
        private bool[] _stallSlotListAlloted = new bool[4];
        private List<BaseCustomer> _customerList = new();
        private float _spawnTimer = 0f;

        private void OnEnable()
        {
            BaseCustomer.OnFinishedOrdered += OnFinishedOrdered;
        }

        private void OnDisable()
        {
            BaseCustomer.OnFinishedOrdered -= OnFinishedOrdered;
        }

        private void Start()
        {
            _customerList.Capacity = 4;
            SpawnCustomer();
        }

        private void FixedUpdate()
        {
            if (_totalCustomerToSpawn > 0 && _customerList.Count < _customerList.Capacity)
            {
                if (_spawnTimer <= _spawnDuration)
                {
                    _spawnTimer += Time.fixedDeltaTime;
                }
                else
                {
                    SpawnCustomer();
                    _spawnTimer = 0f;
                }
            }
        }

        private void SpawnCustomer()
        {
            int randomPrefab = Random.Range(0, _customerPrefabList.Length);
            int randomPoint = Random.Range(0, _spawnPoint.Length);

            IPoolObject spawnedCustomer = _customerPool.CreateObject(_customerPrefabList[randomPrefab], _spawnPoint[randomPoint]);

            List<int> randomTransformIndex = new();

            for (int i = 0; i < _stallSlotListAlloted.Length; i++)
            {
                if (!_stallSlotListAlloted[i])
                {
                    randomTransformIndex.Add(i);
                }
            }

            int randomTransform = randomTransformIndex[Random.Range(0, randomTransformIndex.Count)];

            BaseCustomer baseSpawnedCustomer = spawnedCustomer.gameObject.GetComponent<BaseCustomer>();
            _customerList.Add(baseSpawnedCustomer);
            baseSpawnedCustomer.SetDestination(_stallSlotList[randomTransform]);
            baseSpawnedCustomer.SetCustomerStatus(_customerStatus[randomTransform]);
            if (OnCustomerSpawned != null)
            {
                OnCustomerSpawned(baseSpawnedCustomer);
            }
            _stallSlotListAlloted[randomTransform] = true;
            _totalCustomerToSpawn--;
        }

        private void OnFinishedOrdered(BaseCustomer baseFinishedCustomer, Transform destination)
        {
            int index = System.Array.IndexOf(_stallSlotList, destination);
            _stallSlotListAlloted[index] = false;
            _customerList.Remove(baseFinishedCustomer);
        }
    }
}