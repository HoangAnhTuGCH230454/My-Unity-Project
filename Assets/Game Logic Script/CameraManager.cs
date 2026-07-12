using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] virtualCameras;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer currentTransposer;

    [Header("Y Damping Setting for player Jump and Fall")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    public float playerFallSpeedTheshold = -10;
    private float originalYDamp;
    public bool isLerpingYDamp;
    public bool hasLerpingYDamp;
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        foreach (var cam in virtualCameras)
        {
            if (cam == null)
                continue;

            if (cam.enabled)
            {
                currentCamera = cam;
                currentTransposer =
                    cam.GetCinemachineComponent<CinemachineFramingTransposer>();
                break;
            }
        }

        if (currentCamera == null)
        {
            Debug.LogError("No enabled virtual camera found.");
            return;
        }

        if (currentTransposer == null)
        {
            Debug.LogError(currentCamera.name + " has no FramingTransposer.");
            return;
        }

        originalYDamp = currentTransposer.m_YDamping;
    }

    private void Start()
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].Follow = PlayerController.Instance.transform;
        }
    }

    public void SwapCamera(CinemachineVirtualCamera _camera)
    {
        currentCamera.enabled = false;
        currentCamera = _camera;
        currentCamera.enabled = true;
    }

    public IEnumerator LerpYDamping(bool isPlayerFalling)
    {
        isLerpingYDamp = true;
        float _startYDamp = currentTransposer.m_YDamping;
        float _endYDamp = 0;
        if (!isPlayerFalling)
        {
            _endYDamp = panAmount;
            hasLerpingYDamp = true;
        }
        else
        {
            _endYDamp = originalYDamp;
        }
        float _timer = 0;
        while (_timer < panTime)
        {
            _timer += Time.deltaTime;
            float _lerpedPanAmount = Mathf.Lerp(_startYDamp, _endYDamp, (_timer / panTime));
            currentTransposer.m_YDamping = Mathf.Lerp(_startYDamp, _endYDamp, _timer / panTime);
            yield return null;
        }
        isLerpingYDamp = false;
    }
}
