using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugFps : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fps;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        _fps.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }
}