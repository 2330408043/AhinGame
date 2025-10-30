using Input;
using UnityEngine;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform lookAtTarget;
        private PlayerInputSystem playerInput;
        private Transform playerCamera;

        [SerializeField, Header("鼠标灵敏度"), Range(0.1f, 1.0f)] public float mouseSpeed;
        [SerializeField, Header("相机旋转平滑度"), Range(0.1f, 1.0f)] public float rotationSmoothTime;
        
        [SerializeField, Header("相机碰撞")] private Vector2 distanceMinMax = new Vector2(0.01f, 2f);
        [SerializeField] private float collisionLerpTime; //碰撞移动平滑度
        
        [SerializeField, Header("锁敌")] private bool isLockOn;
        [SerializeField] private Transform currentTarget;
        private Vector3 rotationSmoothVelocity;
        private Vector3 currentRotation; //相机锚点欧拉角
        private float cameraDistance; //相机相对锚点的偏移距离
        private float yaw; //偏航角
        private float pitch; //俯仰角
        private Vector2 pitchMinMax = new Vector2(-75f, 70f); //最大俯仰角
        
        public LayerMask collisionLayer;

        private void Awake()
        {
            playerCamera = UnityEngine.Camera.main.transform;
            playerInput = lookAtTarget.root.GetComponent<PlayerInputSystem>();
        }
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        private void Update()
        {
            GetCameraControllerInput();
        }
        private void LateUpdate()
        {
            ControlCamera();
            CameraCollision();
        }
        
        //获取摄像机控制输入
        private void GetCameraControllerInput()
        {
            if (isLockOn) return;
            yaw += playerInput.cameraLook.x * mouseSpeed;
            pitch -= playerInput.cameraLook.y * mouseSpeed;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        }
        //控制摄像机控制器（相机锚点）的旋转和位置
        private void ControlCamera()
        {
            if (!isLockOn)
            {
                //SmoothDamp的插值更加平滑 内在已经处理了 Time.deltaTime
                currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity,
                    rotationSmoothTime);
                transform.eulerAngles = currentRotation;
            }
            transform.position = lookAtTarget.position; //相机锚点始终在角色身上
        }
        //摄像机碰撞检测 （实际设置摄像机相对于相机锚点的偏移）
        private void CameraCollision()
        {
            Vector3 worldCamPosition = transform.TransformPoint(Vector3.back * distanceMinMax.y); //期望位置下相机的世界坐标
            //期望位置和角色位置之间有障碍物遮挡，更新摄像机偏移距离；否则保持期望距离
            if (Physics.Linecast(transform.position, worldCamPosition, out RaycastHit hit, collisionLayer))
                cameraDistance = Mathf.Clamp(hit.distance * .9f, distanceMinMax.x, distanceMinMax.y); 
            else cameraDistance = distanceMinMax.y;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,
                Vector3.back * (cameraDistance - 0.1f), collisionLerpTime * Time.deltaTime);
        }

        public void SetLookAtTarget(Transform target)
        {
            lookAtTarget = target;
        }
    }
}
