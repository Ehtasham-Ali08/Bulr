using TMPro;
using UnityEngine;

public class PositionUI : MonoBehaviour
{
    public TextMeshProUGUI positionText;

    [Header("Camera")]
    public Camera targetCamera;

    public void SetPosition(int position)
    {
        if (position == 1)
            positionText.text = "1st";
        else if (position == 2)
            positionText.text = "2nd";
        else if (position == 3)
            positionText.text = "3rd";
        else
            positionText.text = position + "th";
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        Vector3 direction = transform.position - targetCamera.transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}