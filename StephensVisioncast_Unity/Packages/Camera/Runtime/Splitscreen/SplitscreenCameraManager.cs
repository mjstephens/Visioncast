using System.Collections.Generic;
using UnityEngine;

namespace Stephens.Camera
{
    [DefaultExecutionOrder(-1)]
    public class SplitscreenCameraManager : MonoBehaviour
    {
        #region VARIABLES
        
        [Header("References")]
        [SerializeField] private GameObject _prefabGameCameraSplitBackground;
        
        [Header("Config")] 
        [SerializeField] private DataConfigSplitscreenCameraLayout _dataSplitscreen;

        public static SplitscreenCameraManager Instance;
        private static readonly Dictionary<int, GameCamera> _cams = new(4);
        private GameObject _splitBackgroundCam;

        #endregion VARIABLES


        #region SINGLETON

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        #endregion SINGLETON


        #region REGISTER

        /// <summary>
        /// GameCamera instances will automatically register themselves, splitting the screen if necessary
        /// </summary>
        public int RegisterCamera(GameCamera cam)
        {
            // Add the camera to the cameras list
            int thisIndex = GetNextAvailableCameraIndex();
            _cams.Add(thisIndex, cam);
            
            // Update the splitscreen layout to reflect new player count
            ArrangeSplitscreenLayout();

            // Create background camera if we have more than one player 
            if (thisIndex > 0 && !_splitBackgroundCam)
            {
                _splitBackgroundCam = Instantiate(_prefabGameCameraSplitBackground);
                _splitBackgroundCam.transform.name = "[CAMERA_BACKGROUND]";
            }
            
            return thisIndex;
        }

        internal void UnregisterCamera(int cameraIndex)
        {
            
        }

        #endregion REGISTER


        #region SPLITSCREEN

        private void ArrangeSplitscreenLayout()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            switch (GetValidCameraCount())
            {
                case 1:
                    Vector2 maxPanel = GetMaxPanelSizeForSplit(screenSize);
                    Vector2 relativePanelSize = new Vector2(maxPanel.x / screenSize.x, maxPanel.y / screenSize.y);
                    _cams[0].SetCameraRect(new Rect(0, 0, relativePanelSize.x, relativePanelSize.y));
                    break;
                case 2:
                    // We need to determine the dimensions of the split. Defaults to largest screen within aspect range
                    Vector2 maxHorizontalPanel = GetMaxPanelSizeForSplit(new Vector2(screenSize.x, screenSize.y / 2));
                    Vector2 maxVerticalPanel = GetMaxPanelSizeForSplit(new Vector2(screenSize.x / 2, screenSize.y));
                    switch (_dataSplitscreen.TwoCamDivisionType)
                    {
                        case Splitscreen2CameraDivisionType.Automatic:

                            // We maximize for panel area
                            if (maxHorizontalPanel.sqrMagnitude > maxVerticalPanel.sqrMagnitude)
                            {
                                Apply2SplitPanels(maxHorizontalPanel, screenSize, true);
                            }
                            else
                            {
                                Apply2SplitPanels(maxVerticalPanel, screenSize, false);
                            }
                        
                            break;
                    
                        case Splitscreen2CameraDivisionType.ForceHorizontal:
                            Apply2SplitPanels(maxHorizontalPanel, screenSize, true);
                            break;
                    
                        case Splitscreen2CameraDivisionType.ForceVertical:
                            Apply2SplitPanels(maxVerticalPanel, screenSize, false);
                            break;
                    }
                    break;
                
                case 3:
                    Vector2 max3Panel = GetMaxPanelSizeForSplit(new Vector2(screenSize.x / 2, screenSize.y / 2));
                    Apply3SplitPanels(max3Panel, screenSize, _dataSplitscreen.ThreeCamArrangementType); 
                    break;
                
                case 4:
                    Vector2 max4Panel = GetMaxPanelSizeForSplit(new Vector2(screenSize.x / 2, screenSize.y / 2));
                    Apply4SplitPanels(max4Panel, screenSize);
                    break;
                    
                    default: Debug.LogError("Invalid number of splitscreen panels specified!"); break;
            }
        }

        private void Apply2SplitPanels(Vector2 panelSize, Vector2 screenSize, bool horizontalSplit)
        {
            Vector2 relativePanelSize = new Vector2(panelSize.x / screenSize.x, panelSize.y / screenSize.y);
            Vector2 layoutAdjustment;
            if (horizontalSplit)
            {
                 layoutAdjustment = new Vector2((1 - relativePanelSize.x) / 2, 0);
                 _cams[0].SetCameraRect(new Rect()
                 {
                     x = layoutAdjustment.x, y = 0.5f, width = relativePanelSize.x, height = relativePanelSize.y 
                 });
                 _cams[1].SetCameraRect(new Rect()
                 {
                     x = layoutAdjustment.x, y = 0f, width = relativePanelSize.x, height = relativePanelSize.y 
                 });
            }
            else
            {
                layoutAdjustment = new Vector2(0, (1 - relativePanelSize.y) / 2);
                _cams[0].SetCameraRect(new Rect()
                {
                    x = 0, y = layoutAdjustment.y, width = relativePanelSize.x, height = relativePanelSize.y 
                });
                _cams[1].SetCameraRect(new Rect()
                {
                    x = 0.5f, y = layoutAdjustment.y, width = relativePanelSize.x, height = relativePanelSize.y 
                });
            }
        }

        private void Apply3SplitPanels(Vector2 panelSize, Vector2 screenSize, Splitscreen3CameraArrangementType type)
        {
            Vector2 relativePanelSize = new Vector2(panelSize.x / screenSize.x, panelSize.y / screenSize.y);
            Vector2 singlePanelAdjustment = new Vector2((1 - relativePanelSize.x) / 2, 0);
            Vector2 multiPanelAdjustment = new Vector2((1 - (relativePanelSize.x * 2)) / 3, 0);
            Rect r1 = default, r2 = default, r3 = default;
            switch (type)
            {
                case Splitscreen3CameraArrangementType.CenteredBottom:
                    r1 = new Rect
                    {
                        x = multiPanelAdjustment.x, 
                        y = 0.5f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    r2 = new Rect
                    {
                        x = relativePanelSize.x + (multiPanelAdjustment.x * 2), 
                        y = 0.5f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    r3 = new Rect
                    {
                        x = singlePanelAdjustment.x, 
                        y = 0f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    break;
                case Splitscreen3CameraArrangementType.CenteredTop:
                    r1 = new Rect
                    {
                        x = singlePanelAdjustment.x, 
                        y = 0.5f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    r2 = new Rect
                    {
                        x = multiPanelAdjustment.x, 
                        y = 0f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    r3 = new Rect
                    {
                        x = relativePanelSize.x + (multiPanelAdjustment.x * 2), 
                        y = 0f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    break;
                case Splitscreen3CameraArrangementType.NotCentered:
                    r1 = new Rect
                    {
                        x = multiPanelAdjustment.x, 
                        y = 0.5f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    r2 = new Rect
                    {
                        x = relativePanelSize.x + (multiPanelAdjustment.x * 2), 
                        y = 0.5f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    r3 = new Rect
                    {
                        x = multiPanelAdjustment.x, 
                        y = 0f, 
                        width = relativePanelSize.x, 
                        height = relativePanelSize.y
                    };
                    break;
            }

            _cams[0].SetCameraRect(r1);
            _cams[1].SetCameraRect(r2);
            _cams[2].SetCameraRect(r3);
        }

        private void Apply4SplitPanels(Vector2 panelSize, Vector2 screenSize)
        {
            Vector2 relativePanelSize = new Vector2(panelSize.x / screenSize.x, panelSize.y / screenSize.y);
            Vector2 adjustment = new Vector2((1 - (relativePanelSize.x * 2)) / 3, 0);
            _cams[0].SetCameraRect(new Rect
            {
                x = adjustment.x, 
                y = 0.5f, 
                width = relativePanelSize.x, 
                height = relativePanelSize.y
            });
            _cams[1].SetCameraRect(new Rect { 
                x = relativePanelSize.x + (adjustment.x * 2), 
                y = 0.5f, 
                width = relativePanelSize.x, 
                height = relativePanelSize.y });
            _cams[2].SetCameraRect(new Rect
            {
                x = adjustment.x, 
                y = 0f, 
                width = relativePanelSize.x, 
                height = relativePanelSize.y
            });
            _cams[3].SetCameraRect(new Rect { 
                x = relativePanelSize.x + (adjustment.x * 2), 
                y = 0f, 
                width = relativePanelSize.x, 
                height = relativePanelSize.y });
        }

        private Vector2 GetMaxPanelSizeForSplit(Vector2 baseSize)
        {
            Vector2 maxSize = baseSize;
            
            // If whole panel fits within target aspect, we're good
            if (TargetInRange(_dataSplitscreen.MinAspect, _dataSplitscreen.MaxAspect, maxSize.x / maxSize.y))
            {
                return maxSize;
            }
            
            // We need to trim the panel to fit
            if (baseSize.x > baseSize.y)
            {
                // Height is max factor
                maxSize = new Vector2(maxSize.y * _dataSplitscreen.MaxAspect, maxSize.y);
            }
            else
            {
                // Width is max factor
                maxSize = new Vector2(maxSize.x, maxSize.x / _dataSplitscreen.MinAspect);
            }

            return maxSize;
        }

        private static bool TargetInRange(float min, float max, float target)
        {
            return target >= min && target <= max;
        }

        private int GetValidCameraCount()
        {
            int count = 0;
            if (_cams.ContainsKey(0)) count++;
            if (_cams.ContainsKey(1)) count++;
            if (_cams.ContainsKey(2)) count++;
            if (_cams.ContainsKey(3)) count++;

            return count;
        }

        #endregion SPLITSCREEN


        #region UTILITY

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _cams.Clear();
            Instance = null;
        }

        private static int GetNextAvailableCameraIndex()
        {
            return _cams.Count;
        }

        #endregion UTILITY
    }
}