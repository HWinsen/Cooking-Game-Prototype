using Harryanto.CookingGame.Cooking;
using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

namespace Harryanto.CookingGame.Customer
{
    public class CustomerServer : MonoBehaviour
    {
        public delegate void GameStateDelegate(bool gameState);
        public static event GameStateDelegate UpdateGameWinState;

        [SerializeField] private SpriteAtlas[] _orderSpriteAtlas;
        [SerializeField] private PoolingSystem _orderImagePool = new(12);
        [SerializeField] private OrderPrefab _orderSpritePrefab;
        [SerializeField] private List<Sprite> _orderSprites;
        [SerializeField] private int _maxOrderCount = 3;
        private Dictionary<BaseCustomer, List<string>> _customerOrderList = new();
        private int _totalCustomerServed = 0;
        [SerializeField] private TMP_Text _customerServed;

        private void Awake()
        {
            CustomerSpawner.OnCustomerSpawned += CustomerSpawner_OnCustomerSpawned;
            BaseCustomer.OnInsertingOrder += BaseCustomer_OnInsertingOrder;
            BaseCustomer.OnRemoveAllOrder += BaseCustomer_OnRemoveAllOrder;
            Beverage.CheckOrder += CheckOrder;
            Plate.CheckOrder += CheckOrder;
            CustomerSpawner.OnStopSpawning += CheckTotalCustomerServed;
            GameManager.RestartCustomerServer += RestartCustomerServer;
            LoadOrderSprites();
        }

        private void OnDestroy()
        {
            CustomerSpawner.OnCustomerSpawned -= CustomerSpawner_OnCustomerSpawned;
            BaseCustomer.OnInsertingOrder -= BaseCustomer_OnInsertingOrder;
            BaseCustomer.OnRemoveAllOrder -= BaseCustomer_OnRemoveAllOrder;
            Beverage.CheckOrder -= CheckOrder;
            Plate.CheckOrder -= CheckOrder;
            CustomerSpawner.OnStopSpawning -= CheckTotalCustomerServed;
            GameManager.RestartCustomerServer -= RestartCustomerServer;
        }

        private void Start()
        {
            _customerServed.SetText($"Customer Served: {_totalCustomerServed:F0}");
        }

        private void CustomerSpawner_OnCustomerSpawned(BaseCustomer baseCustomer)
        {
            //_customerOrderList.Add(baseCustomer, InsertOrder(baseCustomer.OrderPanel));
            _customerOrderList.Add(baseCustomer, new());
        }

        private void BaseCustomer_OnInsertingOrder(BaseCustomer baseCustomer, Transform orderPanel)
        {
            _customerOrderList[baseCustomer] = InsertOrder(orderPanel);
        }

        private bool CheckOrder(string orderName)
        {
            bool isOrderTrue = false;

            if (_customerOrderList.Count >= 1)
            {
                //get customers with match order
                List<BaseCustomer> orderingCustomer = new();

                foreach (KeyValuePair<BaseCustomer, List<string>> customerOrder in _customerOrderList)
                {
                    for (int i = 0; i < customerOrder.Value.Count; i++)
                    {
                        if (customerOrder.Value[i] == orderName)
                        {
                            orderingCustomer.Add(customerOrder.Key);
                        }
                    }
                }

                if (orderingCustomer.Count >= 1)
                {
                    //get all customer patience
                    List<float> customerPatienceTimer = new();
                    for (int i = 0; i < orderingCustomer.Count; i++)
                    {
                        customerPatienceTimer.Add(orderingCustomer[i].PatienceTimer);
                    }

                    // get lowest patience customer
                    BaseCustomer lowestPatienceCustomer = null;
                    float lowestPatienceValue = 0;

                    for (int i = 0; i < customerPatienceTimer.Count; i++)
                    {
                        if (i == 0)
                        {
                            lowestPatienceValue = customerPatienceTimer[i];
                            lowestPatienceCustomer = orderingCustomer[i];
                        }
                        else
                        {
                            if (customerPatienceTimer[i] < lowestPatienceValue)
                            {
                                lowestPatienceValue = customerPatienceTimer[i];
                                lowestPatienceCustomer = orderingCustomer[i];
                            }
                        }

                        if (i == customerPatienceTimer.Count - 1)
                        {
                            // delete order from dictionary
                            _customerOrderList[lowestPatienceCustomer].Remove(orderName);

                            // delete order from customer order panel
                            OnRemoveOrder(lowestPatienceCustomer.OrderPanel, orderName);

                            if (_customerOrderList[lowestPatienceCustomer].Count >= 1)
                            {
                                lowestPatienceCustomer.BoostPatience();
                            }
                            else
                            {
                                lowestPatienceCustomer.ZeroPatience();
                                _totalCustomerServed++;
                                _customerServed.SetText($"Customer Served: {_totalCustomerServed:F0}");
                            }
                        }
                    }

                    isOrderTrue = true;
                }
            }

            return isOrderTrue;
        }

        private void LoadOrderSprites()
        {
            for (int i = 0; i < _orderSpriteAtlas.Length; i++)
            {
                Sprite[] sprites = new Sprite[_orderSpriteAtlas[i].spriteCount];
                _orderSpriteAtlas[i].GetSprites(sprites);
                for (int j = 0; j < sprites.Length; j++)
                {
                    _orderSprites.Add(sprites[j]);
                }
            }
        }

        private List<string> InsertOrder(Transform orderPanel)
        {
            int randomOrderCount = Random.Range(1, _maxOrderCount + 1);
            List<string> orderNames = new(randomOrderCount);
            for (int i = 0; i < randomOrderCount; i++)
            {
                int randomIndex = Random.Range(0, _orderSprites.Count);
                IPoolObject spawnedOrder = _orderImagePool.CreateObject(_orderSpritePrefab);
                GameObject spawnedOrderGameObject = spawnedOrder.gameObject;
                spawnedOrderGameObject.GetComponent<OrderPrefab>().SetOrderSprite(_orderSprites[randomIndex]);
                spawnedOrderGameObject.transform.SetParent(orderPanel, false);
                orderNames.Add(spawnedOrderGameObject.name);
            }
            return orderNames;
        }

        private void OnRemoveOrder(Transform orderPanel, string orderName)
        {
            Transform orderPanelChild = orderPanel.Find(orderName);
            orderPanelChild.GetComponent<OrderPrefab>().StoreToPool();
            orderPanelChild.SetParent(null);
        }

        private void BaseCustomer_OnRemoveAllOrder(BaseCustomer baseCustomer, Transform orderPanel)
        {
            if (orderPanel.childCount >= 1)
            {
                for (int i = 0; i < orderPanel.childCount; i++)
                {
                    Transform orderPanelChild = orderPanel.GetChild(i);
                    orderPanelChild.GetComponent<OrderPrefab>().StoreToPool();
                    orderPanelChild.SetParent(null);
                }
            }

            _customerOrderList.Remove(baseCustomer);
        }

        private void CheckTotalCustomerServed(int totalCustomerToSpawn)
        {
            if (_totalCustomerServed >= totalCustomerToSpawn)
            {
                UpdateGameWinState(true);
            }
            else
            {
                UpdateGameWinState(false);
            }
        }

        private void RestartCustomerServer()
        {
            _totalCustomerServed = 0;
            _customerServed.SetText($"Customer Served: {_totalCustomerServed:F0}");

            List<BaseCustomer> customerTempList = new(_customerOrderList.Keys);
            foreach (BaseCustomer customer in customerTempList)
            {
                BaseCustomer_OnRemoveAllOrder(customer, customer.OrderPanel);
            }
        }
    }
}