using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float widthUnit = 6f;
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        // 게임 시작시 해상도가 조절되게
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = widthUnit / _camera.aspect / 2;
    }
}
