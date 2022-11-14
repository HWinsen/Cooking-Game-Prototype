using Harryanto.CookingGame.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderPrefab : PoolObject
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetOrderSprite(Sprite orderSprite)
    {
        _image.sprite = orderSprite;
        gameObject.name = orderSprite.name;
    }
}
