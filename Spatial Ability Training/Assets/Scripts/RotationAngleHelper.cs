using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotationAngleHelper: MonoBehaviour
{

    public static float CalculateRotationAngle(Vector3 currentRotationEuler, Vector3 desiredRotationEuler)
    {
        // Create a Quaternion from the rotation
        Quaternion currentRotationQuaternion = Quaternion.Euler(currentRotationEuler);

        // Calculate the angle between the current rotation and the neutral rotation
        float angle = Vector3.Angle(currentRotationQuaternion.eulerAngles, desiredRotationEuler);

        // Determine if the rotation is clockwise or counterclockwise
        Vector3 forward = currentRotationQuaternion * Vector3.forward;
        Vector3 up = currentRotationQuaternion * Vector3.up;
        Vector3 right = currentRotationQuaternion * Vector3.right;

        if ((forward.y > 0 && up.z > 0 && right.x > 0) ||
            (forward.y < 0 && up.z < 0 && right.x < 0))
        {
            // Clockwise rotation
            angle = 360 - angle;
        }

        return angle;
    }
    
    public static bool IsRotationWithinLimits(Vector3 eulerAngles, float maxAngle, int offset = 0)
    {
        var x = Mathf.Abs(eulerAngles.x);
        var y = Mathf.Abs(eulerAngles.y);
        var z = Mathf.Abs(eulerAngles.z);
        //Add offset degrees on y, so if they are slightly turned we still get correct result
        return (x <= maxAngle || x >= (360 - maxAngle)) &&
               (y <= maxAngle+ offset || y >= (360 - maxAngle-offset)) &&
               (z <= maxAngle || z >= (360 - maxAngle));
    }
    
    public static Vector3 GetRotationFromDegrees(int angle)
    {
        // Convert degrees to radians
        float radians = angle * Mathf.Deg2Rad;

        // Create a Quaternion from the radian angle
        Quaternion q = Quaternion.Euler(0, radians, 0);

        // Convert the Quaternion to Euler angles
        Vector3 eulerAngles = q.eulerAngles;

        return eulerAngles;
    }
}
