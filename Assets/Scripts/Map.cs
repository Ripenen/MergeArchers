using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
     [SerializeField] private Archer _archer;
     [SerializeField] private MeshRenderer[] _levels;
     [SerializeField] private float _cameraMaxX, _cameraMaxZ;
     [SerializeField] private Button _close;

     public Transform CameraPosition;
     public Transform Center;

     private Vector2 _previousViewportPosition;
     private Camera _camera;
     private MergeView _mergeView;
     private InGameView _inGameView;
     private Transform _backPos;

     public void Setup(Camera camera, int level, MergeView mergeView, InGameView inGameView, Transform backPos)
     {
          _backPos = backPos;
          _camera = camera;
          _mergeView = mergeView;
          _inGameView = inGameView;

          _mergeView.Disable();
          _inGameView.SetActiveKeys(false);

          level -= 1;
          
          camera.transform.SetPositionAndRotation(CameraPosition.position, CameraPosition.rotation);

          HighlightCompletedLevels(level);
          
          while (level > _levels.Length)
          {
               level -= _levels.Length - 30;
          }
          
          _archer.Idle();
          _archer.transform.position = _levels[level - 1].transform.GetChild(0).position;
          _archer.transform.LookAt(_levels[level].transform.GetChild(0).position);

          var dir = _archer.transform.position - camera.transform.position;

          camera.transform.position += new Vector3(dir.x, 0, dir.z) * 0.75f;
          
          _close.onClick.AddListener(Close);
     }

     private void Close()
     {
          _mergeView.Enable();
          _inGameView.SetActiveKeys(true);
          
          Destroy(gameObject);
          _camera.transform.SetPositionAndRotation(_backPos.position, _backPos.rotation);
     }

     private void Update()
     {
          if (Input.GetMouseButtonDown(0))
          {
               _previousViewportPosition = new Vector2(Input.mousePosition.x / Screen.width,
                    Input.mousePosition.y / Screen.height);
          }

          if (Input.GetMouseButton(0))
          {
               var viewport = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

               var delta = _previousViewportPosition - viewport;

               var cameraPosition = CameraPosition.position;

               _previousViewportPosition = viewport;

               var angles = _camera.transform.rotation.eulerAngles;

               var rot = Quaternion.Euler(0, angles.y, angles.z);
               _camera.transform.rotation = rot;
               
               _camera.transform.Translate(new Vector3(delta.x * ((float)Screen.width / Screen.height), 0, delta.y * ((float)Screen.height / Screen.width)) * 10, Space.Self);
               
               var x = Mathf.Clamp(_camera.transform.position.x, Center.position.x - _cameraMaxX, Center.position.x + _cameraMaxX);
               var z = Mathf.Clamp(_camera.transform.position.z, Center.position.z - _cameraMaxZ, Center.position.z + _cameraMaxZ);

               _camera.transform.position = new Vector3(x, cameraPosition.y, z);
               
               rot = Quaternion.Euler(angles.x, angles.y, angles.z);
               _camera.transform.rotation = rot;
          }
     }

     private void HighlightCompletedLevels(int completed)
     {
          var materialColor = new Color(1, 0.5f, 0);

          completed = Mathf.Clamp(completed, 1, _levels.Length + 1);
          
          for (int i = 0; i < completed - 1; i++)
          {
               _levels[i].material.color = materialColor;
          }
     }
}