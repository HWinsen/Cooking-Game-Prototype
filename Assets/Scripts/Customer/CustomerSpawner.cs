using Harryanto.CookingGame.LevelSelect;
using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Harryanto.CookingGame.Customer
{
    public class CustomerSpawner : MonoBehaviour
    {
        public delegate void CustomerSpawnerDelegate(BaseCustomer baseCustomer);
        public static event CustomerSpawnerDelegate OnCustomerSpawned;

        public delegate void WinningConditionDelegate(int totalCustomerToSpawn);
        public static event WinningConditionDelegate OnStopSpawning;

        private PoolingSystem _customerPool = new(10);
        [SerializeField] private BaseCustomer[] _customerPrefabList;
        public int TotalCustomerToSpawn;
        private int _customerNeedToBeServed;
        [SerializeField] private TMP_Text _customerLeft;
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
        private bool _isSpawning = true;

        private void OnEnable()
        {
            BaseCustomer.OnFinishedOrdered += OnFinishedOrdered;
            GameManager.SetCustomerSpawnerState += SetSpawnerState;
            GameManager.AddExtraCustomerToSpawn += AddExtraCustomerToSpawn;
            GameManager.RestartCustomerSpawner += RestartCustomerSpawner;
            TotalCustomerToSpawn = LevelController.CurrentLevelMaximumCustomerToSpawn;
            _customerNeedToBeServed = TotalCustomerToSpawn;
            _customerLeft.SetText($"Customer Left: {TotalCustomerToSpawn:F0}");
        }

        private void OnDisable()
        {
            BaseCustomer.OnFinishedOrdered -= OnFinishedOrdered;
            GameManager.SetCustomerSpawnerState -= SetSpawnerState;
            GameManager.AddExtraCustomerToSpawn -= AddExtraCustomerToSpawn;
            GameManager.RestartCustomerSpawner -= RestartCustomerSpawner;
        }

        private void Start()
        {
            _customerList.Capacity = 4;
            SpawnCustomer();
        }

        private void FixedUpdate()
        {
            if (_isSpawning)
            {
                if (TotalCustomerToSpawn > 0 && _customerList.Count < _customerList.Capacity)
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
            _stallSlotListAlloted[randomTransform] = true;
            TotalCustomerToSpawn--;
            _customerLeft.SetText($"Customer Left: {TotalCustomerToSpawn:F0}");
            OnCustomerSpawned(baseSpawnedCustomer);
        }

        private void OnFinishedOrdered(BaseCustomer baseFinishedCustomer, Transform destination)
        {
            int index = System.Array.IndexOf(_stallSlotList, destination);
            _stallSlotListAlloted[index] = false;
            _customerList.Remove(baseFinishedCustomer);
        }

        private void SetSpawnerState(bool spawnerState)
        {
            _isSpawning = spawnerState;

            if (!_isSpawning)
            {
                OnStopSpawning(_customerNeedToBeServed);
            }
        }

        private void AddExtraCustomerToSpawn(int amountToSpawn)
        {
            TotalCustomerToSpawn = amountToSpawn;
            _customerNeedToBeServed = TotalCustomerToSpawn;
            _customerLeft.SetText($"Customer Left: {TotalCustomerToSpawn:F0}");
        }

        private void RestartCustomerSpawner()
        {
            _spawnTimer = 0f;

            for (int i = 0; i < _customerList.Count; i++)
            {
                _customerList[i].CustomerStatus.SetActive(false);
                _customerList[i].StoreToPool();
            }

            for (int i = 0; i < _stallSlotListAlloted.Length; i++)
            {
                _stallSlotListAlloted[i] = false;
            }
        }
    }
}