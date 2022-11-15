using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Harryanto.CookingGame.Cooking
{
    public abstract class Cook : MonoBehaviour
    {
        [SerializeField] protected Button[] _buttons;
        public bool IsCooking { protected set; get; } = false;

        protected virtual void Initialize()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].onClick.RemoveAllListeners();
                _buttons[i].onClick.AddListener(StartCooking);
            }
        }

        protected abstract void StartCooking();

        protected virtual void Start()
        {
            Initialize();
        }
    }
}