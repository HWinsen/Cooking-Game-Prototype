using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Harryanto.CookingGame.Customer
{
    public class NormalCustomer : BaseCustomer
    {
        private void OnEnable()
        {
            SetPatienceTimer();
            RandomSpawnIndex = Random.Range(0, SpawnPoint.Length);
            Ordered = false;
        }

        private void Update()
        {
            CustomerBehavior();
        }
    }
}