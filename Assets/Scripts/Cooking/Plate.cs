using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Harryanto.CookingGame.Cooking
{
    public class Plate : Cook
    {
        public delegate bool PlateDelegate(string orderName);
        public static event PlateDelegate CheckOrder;

        //private Button _plateButton;
        [SerializeField] private Image _plateImage;
        [SerializeField] private SpriteAtlas _donutSpriteAtlas;
        public bool IsOccupied { private set; get; }
        public bool IsPouredWithTopping { private set; get; }

        //protected override void Start()
        //{
        //    base.Start();
        //    _plateImage = GetComponent<Image>();
        //    //_plateButton = GetComponent<Button>();
        //}

        protected override void StartCooking()
        {
            if (_plateImage.sprite != null)
            {
                bool isOrderTrue = CheckOrder(_plateImage.sprite.name);

                if (isOrderTrue)
                {
                    SetPlateOccupied(false);
                    SetPouredWithTopping(false);
                    _plateImage.sprite = null;
                }
            }
        }

        public void AddTopping(string toppingName)
        {
            _plateImage.sprite = _donutSpriteAtlas.GetSprite(toppingName);
        }

        private void SetPlateOccupied(bool state)
        {
            IsOccupied = state;
        }

        private void SetPouredWithTopping(bool state)
        {
            IsPouredWithTopping = state;
        }

        public void OnPouredWithTopping(Sprite sprite)
        {
            _plateImage.sprite = sprite;
            SetPouredWithTopping(true);
        }

        public void OnDoneCooking(Sprite sprite)
        {
            _plateImage.sprite = sprite;
            SetPlateOccupied(true);
        }
    }
}