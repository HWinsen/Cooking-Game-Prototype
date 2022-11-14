using Harryanto.CookingGame.Cooking;
using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Harryanto.CookingGame.Customer
{
    public class CustomerServer : MonoBehaviour
    {
        [SerializeField] private SpriteAtlas[] _orderSpriteAtlas;
        [SerializeField] private PoolingSystem _orderImagePool = new(11);
        [SerializeField] private OrderPrefab _orderSpritePrefab;
        [SerializeField] private List<Sprite> _orderSprites;
        [SerializeField] private int _maxOrderCount = 3;
        [SerializeField] private float[] _customerPatiences;
        private Dictionary<BaseCustomer, List<string>> _customerOrderList = new();

        private void Awake()
        {
            LoadOrderSprites();
            CustomerSpawner.OnCustomerSpawned += CustomerSpawner_OnCustomerSpawned;
            Beverage.CheckOrder += CheckOrder;
            Plate.CheckOrder += CheckOrder;
            BaseCustomer.OnRemoveAllOrder += BaseCustomer_OnRemoveAllOrder;
        }

        private void OnDestroy()
        {
            CustomerSpawner.OnCustomerSpawned -= CustomerSpawner_OnCustomerSpawned;
            Beverage.CheckOrder -= CheckOrder;
            Plate.CheckOrder -= CheckOrder;
            BaseCustomer.OnRemoveAllOrder -= BaseCustomer_OnRemoveAllOrder;
        }

        private void CustomerSpawner_OnCustomerSpawned(BaseCustomer baseCustomer)
        {
            _customerOrderList.Add(baseCustomer, InsertOrder(baseCustomer.OrderPanel));
        }

        private void CheckOrder(string orderName)
        {
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
                            }
                        }
                    }
                }
            }
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
    }
}