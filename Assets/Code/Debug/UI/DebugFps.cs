using TMPro;
using UnityEngine;

namespace Code.Debug.UI
{
    public class DebugFps : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fpsLabel;
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
        void Update()
        {
            _fpsLabel.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
        }
    }
}