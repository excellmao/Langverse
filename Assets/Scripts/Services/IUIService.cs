using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IUIService
    {
        void ShowNotification(string message, float duration = 3f);
        void UpdateScore(int score);
        void ShowLoadingScreen(bool show);
    }
}