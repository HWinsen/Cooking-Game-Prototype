using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.LevelSelect
{
    public class LevelController:MonoBehaviour
    {
        public static LevelController Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(base.gameObject);
            }
            else
            {
                Destroy(base.gameObject);
            }
        }

        public LevelScriptableObject Level { private set; get; }
        [HideInInspector] public int CurrentLevelMaximumCustomerToSpawn { private set; get; }

        public void SetLevelScriptableObject(LevelScriptableObject levelScriptableObject)
        {
            Level = levelScriptableObject;
            CurrentLevelMaximumCustomerToSpawn = Level.MaximumCustomerToSpawn;
        }

        public void SetLevelClear()
        {
            for (int i = 0; i < Level.IsLevelClear.Length; i++)
            {
                if (!Level.IsLevelClear[i])
                {
                    Level.IsLevelClear[i] = true;
                    break;
                }
            }
            SaveDataController.Instance.Save();
        }
    }
}