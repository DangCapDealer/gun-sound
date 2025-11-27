using UnityEngine;

public static class VectorExtensions
{
    // Tạo vector nhanh
    public static Vector3 Create(float value) => new Vector3(value, value, value);
    public static Vector2 Create2D(float x = 0, float y = 0) => new Vector2(x, y);
    public static Vector3 Create3D(float x = 0, float y = 0, float z = 0) => new Vector3(x, y, z);

    // Extension cho Vector2/3: Set, Add, Clamp, Remap, Magnitude
    public static Vector2 WithX(this Vector2 v, float x) { v.x = x; return v; }
    public static Vector2 WithY(this Vector2 v, float y) { v.y = y; return v; }
    public static Vector2 AddY(this Vector2 v, float y) { v.y += y; return v; }
    public static Vector2 WithXY(this Vector2 v, float x, float y) { v.x = x; v.y = y; return v; }
    public static Vector2 To2D(this Vector3 v) => new Vector2(v.x, v.y);
    public static Vector3 To3D(this Vector2 v, float z = 0) => new Vector3(v.x, v.y, z);
    public static bool IsZero(this Vector3 v) => v == Vector3.zero;
    public static bool IsZero(this Vector2 v) => v == Vector2.zero;
    public static bool IsNearlyEqual(this Vector3 v, Vector3 other, float epsilon = 0.001f) => (v - other).sqrMagnitude < epsilon * epsilon;
    public static float DistanceTo(this Vector3 v, Vector3 other) => Vector3.Distance(v, other);
    public static float DistanceTo(this Vector2 v, Vector2 other) => Vector2.Distance(v, other);
    public static Vector3 ClampMagnitude(this Vector3 v, float max) => Vector3.ClampMagnitude(v, max);
    public static Vector2 ClampMagnitude(this Vector2 v, float max) => Vector2.ClampMagnitude(v, max);
    public static Vector3 WithMagnitude(this Vector3 v, float mag) => v.normalized * mag;
    public static Vector2 WithMagnitude(this Vector2 v, float mag) => v.normalized * mag;
    public static Vector3 Remap(this Vector3 v, float from1, float to1, float from2, float to2)
        => new Vector3(
            Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, v.x)),
            Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, v.y)),
            Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, v.z))
        );
    public static Vector2 Remap(this Vector2 v, float from1, float to1, float from2, float to2)
        => new Vector2(
            Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, v.x)),
            Mathf.Lerp(from2, to2, Mathf.InverseLerp(from1, to1, v.y))
        );

    // Random tiện dụng
    public static float RandomWithin(this Vector2 v) => Random.Range(Mathf.Min(v.x, v.y), Mathf.Max(v.x, v.y));
    public static int RandomWithin(this Vector2Int v) => Random.Range(Mathf.Min(v.x, v.y), Mathf.Max(v.x, v.y));
    public static Vector3 RandomInsideUnitCircleXZ(float radius = 1f)
    {
        var v = Random.insideUnitCircle * radius;
        return new Vector3(v.x, 0, v.y);
    }

    // Project, Rotate
    public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 planeNormal)
        => Vector3.ProjectOnPlane(v, planeNormal);
    public static Vector3 RotateAroundY(this Vector3 v, float angle)
        => Quaternion.Euler(0, angle, 0) * v;

    // Clamp giá trị trong Vector2 như min/max
    public static float Clamp(this Vector2 v, float val) => Mathf.Clamp(val, Mathf.Min(v.x, v.y), Mathf.Max(v.x, v.y));
    public static float Lerp(this Vector2Int v, float t) => Mathf.Lerp(v.x, v.y, t);

    // Nearest point trên vector
    public static Vector2 GetNearestPointToVector(this Vector2 vec, Vector2 point, out float distance, out bool endNearest)
    {
        if (Vector2.Angle(vec, point) >= 90)
        {
            distance = point.magnitude;
            endNearest = true;
            return Vector2.zero;
        }
        if (Vector2.Angle((point - vec), -vec) >= 90)
        {
            distance = (point - vec).magnitude;
            endNearest = true;
            return vec;
        }
        endNearest = false;
        distance = Mathf.Sin(Vector2.Angle(vec, point) * Mathf.Deg2Rad) * point.magnitude;
        return point.magnitude * Mathf.Cos(Vector2.Angle(vec, point) * Mathf.Deg2Rad) * vec.normalized;
    }

    // Box/Circle distance
    public static float BoxDistance(Vector2 a, Vector2 b)
        => Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    public static float CircleDistance(Vector2 a, Vector2 b)
        => Vector2.Distance(a, b);

    // Quaternion extensions
    public static Quaternion WithX(this Quaternion q, float x)
    {
        var e = q.eulerAngles; e.x = x; return Quaternion.Euler(e);
    }
    public static Quaternion WithY(this Quaternion q, float y)
    {
        var e = q.eulerAngles; e.y = y; return Quaternion.Euler(e);
    }
    public static Quaternion WithZ(this Quaternion q, float z)
    {
        var e = q.eulerAngles; e.z = z; return Quaternion.Euler(e);
    }
    public static Quaternion Add(this Quaternion q, Vector3 offset)
    {
        var e = q.eulerAngles;
        e.x += offset.x; e.y += offset.y; e.z += offset.z;
        return Quaternion.Euler(e);
    }
    public static Quaternion AddX(this Quaternion q, float x)
    {
        var e = q.eulerAngles; e.x += x; return Quaternion.Euler(e);
    }
    public static Quaternion AddY(this Quaternion q, float y)
    {
        var e = q.eulerAngles; e.y += y; return Quaternion.Euler(e);
    }
    public static Quaternion AddZ(this Quaternion q, float z)
    {
        var e = q.eulerAngles; e.z += z; return Quaternion.Euler(e);
    }
}

public static class PrimitiveExtensions
{
    public static int FloorTo(this int value, int digit)
    {
        var pow = (int)Mathf.Pow(10, digit);
        return (value / pow) * pow;
    }
    // Clamp tiện cho int
    public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);
}

public static class ColorExtensions
{
    public static Color WithAlpha(this Color color, float a)
    {
        color.a = a;
        return color;
    }
    public static string ToHex(this Color color)
        => ColorUtility.ToHtmlStringRGBA(color);
}