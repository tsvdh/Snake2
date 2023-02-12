using System;
using UnityEngine;

namespace Utils
{
public static class Util
{
    public static Vector3Int QuaternionToDirection(Quaternion quaternion)
    {
        // Default direction is to the right
        return quaternion.eulerAngles.z switch
        {
            0 => Vector3Int.right,
            90 => Vector3Int.up,
            180 => Vector3Int.left,
            270 => Vector3Int.down,
            _ => throw new ArgumentException("must be one of the four primary angles", nameof(quaternion))
        };
    }

    public static Quaternion DirectionToQuaternion(Vector3Int direction)
    {
        return direction switch
        {
            var v when v.Equals(Vector3Int.right) => Quaternion.Euler(0, 0, 0),
            var v when v.Equals(Vector3Int.up) => Quaternion.Euler(0, 0, 90),
            var v when v.Equals(Vector3Int.left) => Quaternion.Euler(0, 0, 180),
            var v when v.Equals(Vector3Int.down) => Quaternion.Euler(0, 0, 270),
            _ => throw new ArgumentException("must be one of the four primary directions", nameof(direction))
        };
    }
}
}