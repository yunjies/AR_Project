using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    ///matrix4x4 to transform

    //public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    //{
    //    Vector3 translate;
    //    translate.x = matrix.m03;
    //    translate.z = matrix.m13;
    //    translate.y = matrix.m23;
    //    return translate;
    //}

    ///// <summary>
    ///// Extract rotation quaternion from transform matrix.
    ///// </summary>
    ///// <param name="matrix">Transform matrix. This parameter is passed by reference
    ///// to improve performance; no changes will be made to it.</param>
    ///// <returns>
    ///// Quaternion representation of rotation transform.
    ///// </returns>
    //public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    //{
    //    Vector3 forward;
    //    forward.x = matrix.m02;
    //    forward.z = matrix.m12;
    //    forward.y = matrix.m22;

    //    Vector3 upwards;
    //    upwards.x = matrix.m01;
    //    upwards.z = matrix.m11;
    //    upwards.y = matrix.m21;

    //    return Quaternion.LookRotation(forward, upwards);
    //}

    ///// <summary>
    ///// Extract scale from transform matrix.
    ///// </summary>
    ///// <param name="matrix">Transform matrix. This parameter is passed by reference
    ///// to improve performance; no changes will be made to it.</param>
    ///// <returns>
    ///// Scale vector.
    ///// </returns>
    //public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    //{
    //    Vector3 scale;
    //    scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
    //    scale.z = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
    //    scale.y = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
    //    return scale;
    //}

    ///// <summary>
    ///// Set transform component from TRS matrix.
    ///// </summary>
    ///// <param name="transform">Transform component.</param>
    ///// <param name="matrix">Transform matrix. This parameter is passed by reference
    ///// to improve performance; no changes will be made to it.</param>
    //public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
    //{
    //    transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
    //    transform.localRotation = ExtractRotationFromMatrix(ref matrix);
    //    transform.localScale = ExtractScaleFromMatrix(ref matrix);
    //    //transform.Rotate(0, 0, 45);
    //}



    /// <summary>
    /// Extract translation from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Translation offset.
    /// </returns>
    public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    {
        return matrix.GetColumn(3);
    }

    /// <summary>
    /// Extract rotation quaternion from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Quaternion representation of rotation transform.
    /// </returns>
    public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    {
        return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
    }

    /// <summary>
    /// Extract scale from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Scale vector.
    /// </returns>
    public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    {

        return new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude);
    }

    /// <summary>
    /// Extract position, rotation and scale from TRS matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <param name="localPosition">Output position.</param>
    /// <param name="localRotation">Output rotation.</param>
    /// <param name="localScale">Output scale.</param>
    public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale)
    {
        localPosition = ExtractTranslationFromMatrix(ref matrix);
        localRotation = ExtractRotationFromMatrix(ref matrix);
        localScale = ExtractScaleFromMatrix(ref matrix);
    }

    /// <summary>
    /// Set transform component from TRS matrix.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
    {
        //transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        //transform.localScale = ExtractScaleFromMatrix(ref matrix);
    }

}
