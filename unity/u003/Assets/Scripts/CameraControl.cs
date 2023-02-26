using System.Collections;
using UnityEngine;

public class CameraControl
{
    private Transform transform;

    public CameraControl(Transform transform)
    {
        this.transform = transform;
        GameManager.Instance.LateUpdater.Add(LateUpdate);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        var cubPos = GameManager.Instance.CubeMove.Trans.position;
        transform.position = new Vector3(0, 1, cubPos.z - 4);
    }
}
