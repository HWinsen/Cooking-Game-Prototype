using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Harryanto.CookingGame.Cooking
{
    public class Stove : Cook
    {
        [SerializeField] private Plate[] _plates;
        [SerializeField] private Image _stoveImage;
        [SerializeField] private Image _stoveTimerImage;
        [SerializeField] private SpriteAtlas _stoveAtlas;
        [SerializeField] private SpriteAtlas _donutAtlas;
        [SerializeField] private SpriteAtlas _timerAtlas;
        [SerializeField] private float _cookDuration;
        private float _cookTimer = 0f;
        private float _cookPercentage = 0f;
        private bool _isDone = false;
        private bool _isBurnt = false;

        protected override void StartCooking()
        {
            if (_isDone)
            {
                bool isDonePlating = false;
                // trigger change image on plate
                for (int i = 0; i < _plates.Length; i++)
                {
                    if (!isDonePlating && !_plates[i].IsOccupied)
                    {
                        _plates[i].OnDoneCooking(_stoveImage.sprite);
                        isDonePlating = true;

                        _stoveImage.sprite = _stoveAtlas.GetSprite("stove-2814568_640");
                        _stoveTimerImage.sprite = _timerAtlas.GetSprite("target_back 1");
                        _cookTimer = 0f;
                        _stoveTimerImage.fillAmount = _cookTimer;
                        _isDone = false;
                        IsCooking = false;
                    }
                }
            }
            else if (_isBurnt)
            {
                _isBurnt = false;
                IsCooking = false;
                _stoveImage.sprite = _stoveAtlas.GetSprite("stove-2814568_640");
            }
        }

        protected override void Start()
        {
            base.Start();
            Batter.OnPouringBatter += OnPouringBatter;
        }

        private void OnDestroy()
        {
            Batter.OnPouringBatter -= OnPouringBatter;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsCooking && !_isBurnt)
            {
                if (_cookTimer < _cookDuration)
                {
                    Cooking();
                }
                else
                {
                    OverCooking();
                }
            }
        }

        private void OverCooking()
        {
            _cookTimer = 0f;
            _stoveTimerImage.fillAmount = _cookTimer;
            if (!_isDone)
            {
                _isDone = true;
                _stoveTimerImage.sprite = _timerAtlas.GetSprite("target_red2 1");
            }
            else
            {
                _stoveTimerImage.sprite = _timerAtlas.GetSprite("target_back 1");
                _isBurnt = true;
                _isDone = false;
                _stoveImage.sprite = _stoveAtlas.GetSprite("donut-145306_640");
            }
        }

        private void Cooking()
        {
            _cookPercentage = _cookTimer / _cookDuration;
            _stoveTimerImage.fillAmount = _cookPercentage;
            _cookTimer += Time.deltaTime;
        }

        public void OnPouringBatter(GameObject gameObject)
        {
            IsCooking = true;
            _stoveImage.sprite = _donutAtlas.GetSprite("chocolate-2024546_640");
        }
    }
}