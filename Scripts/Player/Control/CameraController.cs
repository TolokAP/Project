
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _camera;
  
    

    void Start()
    {
        
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();

    }


    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.forward);
    }

}
