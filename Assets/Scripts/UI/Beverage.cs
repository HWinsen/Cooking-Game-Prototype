using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Harryanto.CookingGame.Cooking
{
    public class Beverage : Cook
    {
        public delegate void BeverageDelegate(string orderName);
        public static event BeverageDelegate CheckOrder;

        private PoolingSystem _glassPool;

        [SerializeField] private Transform _glassesParent;
        [SerializeField] private Glass _glassPrefab;
        private Queue<Glass> _glasses = new();
        [SerializeField] private float _cookDuration;
        [SerializeField] private int _maxTotalBeverages;
        [SerializeField] private Image _beverageTimer;
        private float _cookTimer = 0f;
        private float _cookPercentage = 0f;

        protected override void StartCooking()
        {
            if (_glasses.Count >= 1)
            {
                Serve();
            }
        }

        protected override void Start()
        {
            base.Start();
            _glassPool = new(_maxTotalBeverages);
        }

        void Update()
        {
            if (_glasses.Count < _maxTotalBeverages)
            {
                if (_cookTimer < _cookDuration)
                {
                    _cookPercentage = _cookTimer / _cookDuration;
                    _beverageTimer.fillAmount = _cookPercentage;
                    _cookTimer += Time.deltaTime;
                }
                else
                {
                    //IsCooking = false;
                    _cookTimer = 0f;
                    _beverageTimer.fillAmount = _cookTimer;
                    SpawnGlass();
                }
            }
            else
            {
                _cookTimer = 0f;
                _beverageTimer.fillAmount = _cookTimer;
            }
        }

        private void SpawnGlass()
        {
            IPoolObject spawnedGlass = _glassPool.CreateObject(_glassPrefab);
            GameObject spawnedGlassGameObject = spawnedGlass.gameObject;
            spawnedGlassGameObject.transform.SetParent(_glassesParent, false);
            spawnedGlassGameObject.name = spawnedGlassGameObject.GetComponent<Image>().sprite.name;
            _glasses.Enqueue(spawnedGlassGameObject.GetComponent<Glass>());
        }

        private void Serve()
        {
            Glass removedGlass = _glasses.Peek();
            //CheckOrder(removedGlass.GetComponent<Image>().sprite.name);
            CheckOrder(removedGlass.name+"(Clone)");
            removedGlass.StoreToPool();
            _glasses.Dequeue();
        }
    }
}