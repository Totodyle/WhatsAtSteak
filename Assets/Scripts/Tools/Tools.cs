using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static void DrawGizmosCube(Vector3 p_position, Vector3 p_scale, Quaternion p_rotation)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(p_position, p_rotation, p_scale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;

        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = oldGizmosMatrix;
    }

    public static void DebugLog(string p_text)
    {
        #if DEBUG
        Debug.Log(p_text);
        #endif
    }

    public static void DebugLogError(string p_text)
    {
        #if DEBUG
        Debug.LogError(p_text);
        #endif
    }
}
