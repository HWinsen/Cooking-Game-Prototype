using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Harryanto.CookingGame.Cooking
{
    public class Topping : Cook
    {
        [SerializeField] private Plate[] _plates;
        [SerializeField] private SpriteAtlas _donutAtlas;
        [SerializeField] private string _toppingName;
        private bool _isDonePouringTopping;

        protected override void StartCooking()
        {
            for (int i = 0; i < _plates.Length; i++)
            {
                if (!_isDonePouringTopping && _plates[i].IsOccupied && !_plates[i].IsPouredWithTopping)
                {
                    _plates[i].OnPouredWithTopping(_donutAtlas.GetSprite(_toppingName));
                    _isDonePouringTopping = true;
                }
            }

            _isDonePouringTopping = false;
        }
    }
}