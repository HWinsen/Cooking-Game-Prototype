using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.LevelSelect
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level ScriptableObject")]
    [System.Serializable]
    public class LevelScriptableObject : ScriptableObject
    {
        public int MaximumCustomerToSpawn;

        public bool[] IsLevelClear =
        {
            false,
            false,
            false,
        };
        public bool ConfigureMaximumCustomerToSpawn;

        public void SetMaximumCustomerToSpawn(int customerToSpawn)
        {
            MaximumCustomerToSpawn = customerToSpawn;
        }
    }
}