using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Harryanto.CookingGame.LevelSelect
{
    public class LevelManager : MonoBehaviour
    {
        private LevelController _levelController = new();

        private LevelScriptableObject[] _levelScriptableObjects;
        [SerializeField] private Button _levelButtonPrefab;
        [SerializeField] private Transform _levelButtonParent;
        [SerializeField] private GameObject _configurePanel;
        [SerializeField] private TMP_InputField _maximumCustomerToSpawn;
        [SerializeField] private Button _startLevelButton;
        private List<Button> _levelButtonList = new();

        private void Start()
        {
            _levelScriptableObjects = Resources.LoadAll<LevelScriptableObject>("Level");

            for (int i = 0; i < _levelScriptableObjects.Length; i++)
            {
                int j = i;
                Button tempButton = Instantiate(_levelButtonPrefab, _levelButtonParent, false);
                //tempButton.GetComponentInChildren<TMP_Text>().SetText(_levelScriptableObjects[i].name);
                //tempButton.name = _levelScriptableObjects[i].name;
                for (int k = 0; k < _levelScriptableObjects[i].IsLevelClear.Length; k++)
                {
                    if (!_levelScriptableObjects[i].IsLevelClear[k])
                    {
                        tempButton.GetComponentInChildren<TMP_Text>().SetText($"Level {i+1}-{k+1}");
                        tempButton.name = _levelScriptableObjects[i].name + "Button";
                        break;
                    }
                }
                tempButton.onClick.AddListener(() =>
                {
                    OpenLevel(j);
                });
                _levelButtonList.Add(tempButton);
            }
        }

        private void OpenLevel(int index)
        {
            if (_levelScriptableObjects[index].ConfigureMaximumCustomerToSpawn)
            {
                _configurePanel.SetActive(true);
                _startLevelButton.onClick.AddListener(() =>
                {
                    StartLevel(index);
                });
            }
            else
            {
                StartLevel(index);
            }
        }

        private void StartLevel(int index)
        {
            if (_configurePanel.activeInHierarchy)
            {
                _levelScriptableObjects[index].SetMaximumCustomerToSpawn(int.Parse(_maximumCustomerToSpawn.text));
            }
            _levelController.SetLevelScriptableObject(_levelScriptableObjects[index]);
            SceneManager.LoadScene("Gameplay");
        }
    }
}