using UnityEngine;

namespace Stephens.Camera
{
    [CreateAssetMenu(
        fileName = "DAT_SplitscreenLayout", 
        menuName = "GG/Camera/Splitscreen Layout")]
    public class DataConfigSplitscreenCameraLayout : ScriptableObject
    {
        [Header("Aspect Ratio")]
        [SerializeField] internal float MaxAspect = 3.6f;
        [SerializeField] internal float MinAspect = 1.25f;

        [Header("Preferences")]
        [SerializeField] internal Splitscreen2CameraDivisionType TwoCamDivisionType = Splitscreen2CameraDivisionType.Automatic;
        [SerializeField] internal Splitscreen3CameraArrangementType ThreeCamArrangementType =
            Splitscreen3CameraArrangementType.CenteredBottom;
    }
}