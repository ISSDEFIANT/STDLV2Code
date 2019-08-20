using System;
using UnityEngine;

/// <summary> Расширение класса Vector3 дополнительными матетматическми функциями. </summary>
public static class VectorExtensions
{
    /// <summary> Проецирует вектор на ось. </summary>
    /// <param name="vector"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static float ProjectOn(this Vector3 vector, Vector3 axis)
    {
        return Mathf.Cos(Vector3.Angle(vector, axis) * Mathf.Deg2Rad) * vector.magnitude / axis.magnitude;
    }

    /// <summary> Обрезает вектор, до определённой длины. </summary>
    /// <param name="vector"></param>
    /// <param name="maxMagnitude"> Длина до которой нужно обрезать. </param>
    /// <returns> Обрезанный вектор. </returns>
    public static Vector3 Clip(this Vector3 vector, float maxMagnitude = 1.0f)
    {
        if (vector.magnitude > maxMagnitude)
        {
            return vector.normalized * maxMagnitude;
        }

        return vector;
    }

    /// <summary> Проверяет равенство двух векторов с учетом неточности значений типа float. </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool FloatEquals(this Vector3 vector, Vector3 other)
    {
        return Math.Abs(vector.x - other.x) < float.Epsilon
               && Math.Abs(vector.y - other.y) < float.Epsilon
               && Math.Abs(vector.z - other.z) < float.Epsilon;
    }
}