using UnityEngine;
using TMPro;

public class MyGUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI myDisplay;

    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1f;

    int _frames;
    float _duration, _bestDuration = float.MaxValue, _worstDuration;
    
    // Update is called once per frame
    void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        _frames++;
        _duration += frameDuration;
        if (frameDuration < _bestDuration) {
            _bestDuration = frameDuration;
        }
        if (frameDuration > _worstDuration) {
            _worstDuration = frameDuration;
        }
        if (_duration >= sampleDuration)
        {
            myDisplay.SetText("FPS\n{0:0}\n{1:0}\n{2:0}",
                _frames / _duration,
                1f / _bestDuration, 
                1f / _worstDuration);
            _frames = 0;
            _duration = 0f;
            _bestDuration = float.MaxValue;
            _worstDuration = 0f;
        }
    }
}
