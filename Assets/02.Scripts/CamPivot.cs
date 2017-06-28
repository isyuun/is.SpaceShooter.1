using UnityEngine;

public class CamPivot : MonoBehaviour
{
    private Transform pivotTr;
    private Quaternion rotOrigin = Quaternion.identity;

    public float rotSpeed = 100.0f;

    private void Start()
    {
        pivotTr = GetComponent<Transform>();
    }

    private void Update()
    {
        Debug.Log(pivotTr.rotation.eulerAngles.x);
        if (Input.GetMouseButton(2))
        {
            pivotTr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
            if (pivotTr.rotation.eulerAngles.x < 80 || (pivotTr.rotation.eulerAngles.x < 360 && pivotTr.rotation.eulerAngles.x > 300))
            {
                pivotTr.Rotate(Vector3.right * rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));
            }
        }

        if (Input.GetMouseButtonUp(2))
        {
            pivotTr.rotation = rotOrigin;
        }
    }
}