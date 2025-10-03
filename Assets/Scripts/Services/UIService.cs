using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Services
{
    public class UIService : MonoBehaviour, IUIService
    {
        [Header("UI References")] public TextMeshProUGUI notificationText;
        public TextMeshProUGUI scoreText;
        public GameObject loadingScreen;

        private int currentScore = 0;

        private void Start()
        {
            ServiceLocator.Instance.RegisterService<IUIService>(this);
        }

        public void ShowNotification(string message, float duration = 3f)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
                notificationText.gameObject.SetActive(true);
                StartCoroutine(HideNotificationAfterDelay(duration));
            }
        }

        public void UpdateScore(int score)
        {
            currentScore = score;
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore}";
            }
        }

        public void ShowLoadingScreen(bool show)
        {
            if (!loadingScreen != null)
            {
                loadingScreen.SetActive(show);
            }
        }

        private IEnumerator HideNotificationAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (notificationText != null)
            {
                notificationText.gameObject.SetActive(false);
            }
        }
    }
}
