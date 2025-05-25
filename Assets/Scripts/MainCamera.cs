using UnityEngine;

public class MainCamera : MonoBehaviour {
    void Start()
    {
        CenterCameraOnXAxis(LevelGrid.Instance.GetCenterGridPosition().x);
    }

    void CenterCameraOnXAxis(float xAxis)
    {
        Vector3 newPosition = transform.position;
        newPosition.x = xAxis;
        transform.position = newPosition;
    }
}
