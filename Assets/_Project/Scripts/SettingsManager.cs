using UnityEngine;

namespace TestProject
{
    /// <summary>
    /// Application Settings Class. 
    /// Base class that varies by device or initializes Unity settings
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SetSettings();
        }

        private void SetSettings()
        {
            SetCameraResponsive();
        }

        /// <summary>
        /// Camera Orthographic setting width and height are 
        /// adjusted equally for different devices
        /// </summary>
        private void SetCameraResponsive()
        {
            float horizontalResolution = 1920;
            float currentAspect = (float)Screen.width / (float)Screen.height;
            Camera.main.orthographicSize = horizontalResolution / currentAspect / 340;
        }
    }
}
