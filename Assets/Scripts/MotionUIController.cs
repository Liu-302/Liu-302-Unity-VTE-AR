using UnityEngine;
using UnityEngine.UI;

public class MotionUIController : MonoBehaviour
{
    public SimulatedLegMotion motionSimulator;

    public Button ankleFlexionBtn;
    public Button ankleCircumBtn;
    public Button raiseLegBtn;
    public Button dorsiflexionBtn;
    public Button isotonicBtn;

    private void Start()
    {
        ankleFlexionBtn.onClick.AddListener(() => SetMode("Flexion"));
        ankleCircumBtn.onClick.AddListener(() => SetMode("Circumduction"));
        raiseLegBtn.onClick.AddListener(() => SetMode("Raise"));
        dorsiflexionBtn.onClick.AddListener(() => SetMode("Dorsiflex"));
        isotonicBtn.onClick.AddListener(() => SetMode("Isotonic"));
    }

    void SetMode(string mode)
    {
        motionSimulator.SetMotionMode(mode);
    }
}
