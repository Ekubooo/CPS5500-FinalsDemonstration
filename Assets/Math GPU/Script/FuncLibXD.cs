using UnityEngine;
using static UnityEngine.Mathf;
public static class FuncLibXD
{
    public enum CSFuncName{Wave, MultiWave ,Ripple, Sphere, Tours, ToursA, ToursTT, SphereTT}
    public delegate Vector3 Func(float x, float z, float t);
    public enum FuncName{Wave, MultiWave, CrossWave, Ripple, Sphere, Tours, Tours2, ToursR}

    static Func[] _funcs = {Wave, MultiWave, CrossWave, Ripple, Sphere, Tours,  Tours2, ToursR};

    public static Func GetFunc(FuncName name)
    {
        return _funcs[(int)name];
    }
    
    public static FuncName GetNextFuncName(FuncName name)
    {
        return (int)name < _funcs.Length - 1 ? name + 1 : 0;
    }
    
    public static FuncName GetRandomFunctionNameOtherThan (FuncName name) 
    {
        var choice = (FuncName)Random.Range(1, _funcs.Length);
        return choice == name ? 0 : choice;
    }
    
    // not for object template, though useing static
    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }
    
    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y *= 2f / 3f;
        p.z = v;
        return p;
    }

    public static Vector3 CrossWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {   // uv ball
        // float r = 0.5f + 0.5f * Sin(PI * t);         // dynamic sphere
        // float r = 0.9f + 0.1f * Sin(8f * PI * u);    // virtical strap
        // float r = 0.9f + 0.1f * Sin(8f * PI * v);    // horizon strap
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * 0.5f * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Tours(float u, float v, float t)
    {
        float r = 1f;
        float s = 0.5f + r * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    public static Vector3 Tours2(float u, float v, float t)
    {
        float r1 = 0.75f;
        float r2 = 0.25f;
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    public static Vector3 ToursR(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Morph
        (float u, float v, float t, Func form, Func to, float progress)
    {
        // return Vector3.Lerp(form(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
        return Vector3.LerpUnclamped(form(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
    }
    
}

