using Harryanto.CookingGame.Customer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harryanto.CookingGame.Cooking
{
    public class Batter : Cook
    {
        public delegate void BatterDelegate (GameObject gameObject);
        public static event BatterDelegate OnPouringBatter;

        [SerializeField] private Stove[] _stoves;
        private bool _isPouringBatter;

        protected override void StartCooking()
        {
            for (int i = 0; i < _stoves.Length; i++)
            {
                if (!_isPouringBatter)
                {
                    if (!_stoves[i].IsCooking)
                    {
                        _stoves[i].OnPouringBatter(_stoves[i].gameObject);
                        _isPouringBatter = true;
                    }
                }
            }
            _isPouringBatter = false;
        }
    }
}