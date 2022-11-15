using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.LevelSelect
{
    public class LevelController
    {
        //public static LevelController Instance;

        //private void Awake()
        //{
        //    if (Instance == null)
        //    {
        //        Instance = this;
        //        DontDestroyOnLoad(base.gameObject);
        //    }
        //    else
        //    {
        //        Destroy(base.gameObject);
        //    }
        //}

        private LevelScriptableObject _levelScriptableObject;
        [HideInInspector] public static int CurrentLevelMaximumCustomerToSpawn { private set; get; }

        public void SetLevelScriptableObject(LevelScriptableObject levelScriptableObject)
        {
            _levelScriptableObject = levelScriptableObject;
            CurrentLevelMaximumCustomerToSpawn = _levelScriptableObject.MaximumCustomerToSpawn;
        }
    }
}