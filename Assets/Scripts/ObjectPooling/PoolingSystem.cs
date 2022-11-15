using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.ObjectPooling
{
    public class PoolingSystem
    {
        public int AmountToPool;
        List<IPoolObject> _storedList = new List<IPoolObject>();
        int _spawned = 0;

        public PoolingSystem(int AmountToPool = 10)
        {
            this.AmountToPool = AmountToPool;
        }

        public IPoolObject CreateObject(IPoolObject objectPrefab, Vector3 spawnPosition, Transform parent = null)
        {
            IPoolObject outObject;
            if (_spawned < AmountToPool)
            {
                outObject = MonoBehaviour.Instantiate(objectPrefab.gameObject).
                GetComponent<IPoolObject>();
                outObject.Initial(this);
                _spawned++;
            }
            else
            {
                outObject = _storedList[Random.Range(0, _storedList.Count)];
                _storedList.Remove(outObject);
            }
            outObject.transform.position = spawnPosition;
            outObject.transform.parent = parent;

            outObject.gameObject.SetActive(true);

            return outObject;
        }

        // Overload with Rotation
        public IPoolObject CreateObject(IPoolObject objectPrefab, Vector3 spawnPosition, Quaternion spawnRotation, Transform parent = null)
        {
            IPoolObject outObject;
            if (_spawned < AmountToPool)
            {
                outObject = MonoBehaviour.Instantiate(objectPrefab.gameObject).
                GetComponent<IPoolObject>();
                outObject.Initial(this);
                _spawned++;
            }
            else
            {
                outObject = _storedList[Random.Range(0, _storedList.Count)];
                _storedList.Remove(outObject);
            }
            outObject.transform.position = spawnPosition;
            outObject.transform.rotation = spawnRotation;
            outObject.transform.parent = parent;


            outObject.gameObject.SetActive(true);

            return outObject;
        }

        // Overload IPoolObjectOnly
        public IPoolObject CreateObject(IPoolObject objectPrefab)
        {
            IPoolObject outObject;
            if (_spawned < AmountToPool)
            {
                outObject = MonoBehaviour.Instantiate(objectPrefab.gameObject).
                GetComponent<IPoolObject>();
                outObject.Initial(this);
                _spawned++;
            }
            else
            {
                outObject = _storedList[Random.Range(0, _storedList.Count)];
                _storedList.Remove(outObject);
            }

            outObject.gameObject.SetActive(true);

            return outObject;
        }

        public void Store(IPoolObject poolObject)
        {
            _storedList.Add((IPoolObject)poolObject);
        }
    }
}