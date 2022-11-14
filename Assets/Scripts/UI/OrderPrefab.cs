using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderPrefab : PoolObject
{
    [SerializeField] private Image _image;

    public void SetOrderSprite(Sprite orderSprite)
    {
        _image.sprite = orderSprite;
        gameObject.name = orderSprite.name;
    }
}
