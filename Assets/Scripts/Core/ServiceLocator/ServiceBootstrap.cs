using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ServiceLocator
{
    public class ServiceBootstrap : MonoBehaviour
    {
        [Header("Service Config")]
        [SerializeField] private bool _initOnAwake = true;
        [SerializeField] private bool _enableDebugLogging = true;
    }
}