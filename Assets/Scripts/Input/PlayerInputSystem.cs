using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputSystem : MonoBehaviour //获取玩家输入的封装层。对外提供获取玩家输入的属性
    {
        private InputController inputController;

        public Vector2 movement => inputController.PlayerInput.Movement.ReadValue<Vector2>();
        public Vector2 cameraLook => inputController.PlayerInput.CameraLook.ReadValue<Vector2>();
        public bool atk => inputController.PlayerInput.Atk.triggered;
        public bool roll => inputController.PlayerInput.Roll.triggered;
        public bool jump => inputController.PlayerInput.Jump.triggered;
        public bool run => inputController.PlayerInput.Run.phase == InputActionPhase.Performed;

        private void Awake()
        {
            if (inputController == null) inputController = new InputController();
        }
        private void OnEnable()
        {
            inputController.Enable();
        }
        private void OnDisable()
        {
            inputController.Disable();
        }
    }
}
