using System;
using System.Collections.Generic;
using Stephens.Tick;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Stephens.Input
{
    /// <summary>
    /// Specialized input listener that translates mouse or gamepad inputs to an on-screen pointer.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UISimulatedPointer : MonoBehaviour, IInputPointer, ITickable
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private RectTransform _pointerOverlay;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private Image _ringOuter;
        [SerializeField] private Image _iconInner;

        [Header("Values")] 
        [SerializeField] private float _pointerSpeedMouseKB;
        [SerializeField] private float _pointerSpeedGamepad;
        
        public TickGroup TickGroup => TickGroup.InputTransmission;
        public Vector3 Position => _pointer.position;
        public bool IsOverUI => _hoveredObjs.Count > 0;
        InputPointerType IInputPointer.Type => InputPointerType.Simulated;
        public RectTransform Rect => _pointer;
        public Vector2 Delta => _dataInput.Delta;
        public Camera Camera => _uiCamera;
        public List<GameObject> HoveredObjects => _hoveredObjs;

        private string _controlScheme;
        private PointerEventData _eventData;
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();
        private List<GameObject> _hoveredObjs = new List<GameObject>();
        private readonly List<GameObject> _selectedObjs = new List<GameObject>();
        private PlayerInput _input;
        private Guid _deltaActionID;
        private Guid _scrollActionID;
        private DataInputValuesPointer _dataInput;
        private DataPointerType _type;
        private readonly List<IInputReceiver<DataInputValuesPointer>> _receivers = new();
        private Vector2 _prevEventPos;
        private EventSystem _eventSystem;

        #endregion VARIABLES


        #region INITIALIZATION

        public void Init(PlayerInput input, DataPointerType type)
        {
            _eventSystem = GetComponentInParent<EventSystem>();
            _eventData = new PointerEventData(_eventSystem);
            
            input.onActionTriggered += OnActionTriggered;
            _deltaActionID = input.actions["Delta"].id;
            _scrollActionID = input.actions["Scroll"].id;
            _input = input;
            
            _type = type;
            InitForType();
        }

        private void InitForType()
        {
            switch (_type)
            {
                case DataPointerType.Simulated:
                    _pointerOverlay.gameObject.SetActive(true); 
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case DataPointerType.System:
                    _pointerOverlay.gameObject.SetActive(false);
                    if (_eventSystem.currentInputModule as InputSystemUIInputModule != null)
                    {
                        (_eventSystem.currentInputModule as InputSystemUIInputModule).AssignDefaultActions();
                    }
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
                case DataPointerType.None:
                    _pointerOverlay.gameObject.SetActive(false); 
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
            }
        }

        private void OnEnable()
        {
            TickRouter.Register(this);
        }

        private void OnDisable()
        {
            TickRouter.Unregister(this);
            if (_input)
            {
                _input.onActionTriggered -= OnActionTriggered;
            }
        }

        #endregion INITIALIZATION


        #region API

        public void SetPointerVisible(bool visible)
        {
            if (!visible)
            {
                _type = DataPointerType.None;
                _pointerOverlay.gameObject.SetActive(false); 
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                _type = DataPointerType.Simulated;
                _pointerOverlay.gameObject.SetActive(true); 
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void SetInnerImage(Sprite sprite)
        {
            if (!sprite)
            {
                _iconInner.enabled = false;
                return;
            }

            _iconInner.sprite = sprite;
            _iconInner.enabled = true;
        }

        public void SetOuterRingColor(Color c)
        {
            _ringOuter.color = c;
        }

        #endregion API


        #region INPUT

        public void UpdatePointer(float deltaTime)
        {
            if (!gameObject.activeSelf || _type != DataPointerType.Simulated)
                return;
            
            // Move pointer
            Vector2 delta = _input.actions.FindAction(_deltaActionID).ReadValue<Vector2>();
            _dataInput.Scroll = _input.actions.FindAction(_scrollActionID).ReadValue<Vector2>();
            float speed = _controlScheme == "Gamepad" ? _pointerSpeedGamepad * deltaTime : _pointerSpeedMouseKB * deltaTime;
            Vector2 increase = speed * delta;
            _dataInput.Delta = increase;
            _pointer.anchoredPosition = GatePointerPosition(_pointer.anchoredPosition + increase);
            _dataInput.Position = _pointer.position;
            _eventData.delta = increase;

            // Move virtual mouse
            _eventData.position = _uiCamera.WorldToScreenPoint(_pointer.position);
            _raycastResults = new List<RaycastResult>();
            _eventSystem.RaycastAll(_eventData, _raycastResults);
            
            // Update hovered objects
            List<GameObject> newHovered = new List<GameObject>();
            foreach (RaycastResult obj in _raycastResults)
            {
                newHovered.Add(obj.gameObject);
                _eventData.pointerCurrentRaycast = obj;
            }

            ResolveHoveredObjects(newHovered);

            if (_prevEventPos != _eventData.position)
            {
                foreach (GameObject obj in _hoveredObjs)
                {
                    Move(obj);
                }
            }
            _prevEventPos = _eventData.position;
        }

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (_type != DataPointerType.Simulated)
                return;
            
            switch (context.action.name)
            {
                case "Select":
                    OnActionSelect(context.action);
                    break;
                case "SelectAlternate":
                    OnActionSelectAlternate(context.action);
                    break;
                case "SelectTertiary":
                    OnActionSelectTertiary(context.action);
                    break;
            }
        }

        private void OnActionSelect(InputAction action)
        {
            _eventData.button = PointerEventData.InputButton.Left;
            if (action.phase == InputActionPhase.Started)
            {
                _dataInput.SelectStarted = true;
                _dataInput.SelectIsPressed = true;
                
                foreach (RaycastResult obj in _raycastResults)
                {
                    _eventData.pointerPressRaycast = obj;
                }
                
                ResolveActionPointerDown();
            }
            else if (action.phase == InputActionPhase.Canceled)
            {
                _dataInput.SelectReleased = true;
                _dataInput.SelectIsPressed = false;
                ResolveActionPointerUp();
            }
        }

        private void OnActionSelectAlternate(InputAction action)
        {
            _eventData.button = PointerEventData.InputButton.Right;
            if (action.phase == InputActionPhase.Started)
            {
                _dataInput.SelectAlternateStarted = true;
                _dataInput.SelectAlternateIsPressed = true;
                ResolveActionPointerDown();
            }
            else if (action.phase == InputActionPhase.Canceled)
            {
                _dataInput.SelectAlternateReleased = true;
                _dataInput.SelectAlternateIsPressed = false;
                ResolveActionPointerUp();
            }
        }

        private void OnActionSelectTertiary(InputAction action)
        {
            _eventData.button = PointerEventData.InputButton.Middle;
            if (action.phase == InputActionPhase.Started)
            {
                ResolveActionPointerDown();
            }
            else if (action.phase == InputActionPhase.Canceled)
            {
                ResolveActionPointerUp();
            }
        }

        public void OnControlsChanged(string newControls)
        {
            _controlScheme = newControls;
        }

        #endregion INPUT


        #region EVENTS

        private void ResolveHoveredObjects(List<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                if (!_hoveredObjs.Contains(obj))
                {
                    Enter(obj);
                }
            }

            foreach (GameObject obj in _hoveredObjs)
            {
                if (!objs.Contains(obj))
                {
                    Exit(obj);
                }
            }

            _hoveredObjs = objs;
        }

        private void ResolveActionPointerDown()
        {
            foreach (GameObject obj in _hoveredObjs) 
            {
                PointerDown(obj);
                _selectedObjs.Add(obj);
            }
        }

        private void ResolveActionPointerUp()
        {
            foreach (GameObject obj in _selectedObjs)
            {
                PointerUp(obj);
            }

            _selectedObjs.Clear();
        }

        private void Enter(GameObject obj)
        {
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.initializePotentialDrag);
        }

        private void Exit(GameObject obj)
        {
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.pointerExitHandler);
        }

        private void PointerDown(GameObject obj)
        {
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.beginDragHandler);
        }

        private void PointerUp(GameObject obj)
        {
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.deselectHandler);
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.endDragHandler);
        }

        private void Move(GameObject obj)
        {
            ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.pointerMoveHandler);
            if (_dataInput.SelectIsPressed)
            {
                ExecuteEvents.Execute(obj, _eventData, ExecuteEvents.dragHandler);
            }
        }

        #endregion EVENTS


        #region TRANSMIT

        void ITickable.Tick(float delta)
        {
            foreach (IInputReceiver<DataInputValuesPointer> receiver in _receivers)
            {
                receiver.ReceiveInput(_dataInput, delta);
            }
            
            // Reset input
            _dataInput.SelectStarted = false;
            _dataInput.SelectReleased = false;
            _dataInput.SelectAlternateStarted = false;
            _dataInput.SelectAlternateReleased = false;
        }

        public void RegisterReceiver(IInputReceiver<DataInputValuesPointer> receiver)
        {
            _receivers.Add(receiver);
        }
        
        public void UnregisterReceiver(IInputReceiver<DataInputValuesPointer> receiver)
        {
            _receivers.Remove(receiver);
        }

        #endregion TRANSMIT


        #region UTILITY

        public void SnapPointerToPosition(Vector2 pos)
        {
            _pointer.anchoredPosition = pos;
            _dataInput.Position = _pointer.position;
            
            _eventData.position = _uiCamera.WorldToScreenPoint(_pointer.position);
            _prevEventPos = _eventData.position;
        }

        private Vector2 GatePointerPosition(Vector2 inPosition)
        {
            Vector2 adjustment = Vector2.zero;
            if (inPosition.x < _pointerOverlay.rect.xMin)
                adjustment.x = _pointerOverlay.rect.xMin - inPosition.x;
            else if (inPosition.x > _pointerOverlay.rect.xMax)
                adjustment.x = _pointerOverlay.rect.xMax - inPosition.x;
            
            if (inPosition.y < _pointerOverlay.rect.yMin)
                adjustment.y = _pointerOverlay.rect.yMin - inPosition.y;
            else if (inPosition.y > _pointerOverlay.rect.yMax)
                adjustment.y = _pointerOverlay.rect.yMax - inPosition.y;
            
            return inPosition + adjustment;
        }

        #endregion UTILITY
    }
}