// including file for different math-description surfaces
float3 Wave(float u, float v, float t)
{
    float3 p;
    p.x = u;
    p.y = sin(PI * (u + v + t));
    p.z = v;
    return p;
}
float3 MultiWave (float u, float v, float t)
{
    float3 p;
    p.x = u;
    p.y = sin(PI * (u + 0.5 * t));
    p.y += 0.5 * sin(2.0 * PI * (v + t));
    p.y += sin(PI * (u + v + 0.25 * t));
    p.y *= 1.0 / 2.5f;
    p.z = v;
    return p;
}

float3 Ripple (float u, float v, float t)
{
    float d = sqrt(u * u + v * v);
    float3 p;
    p.x = u;
    p.y = sin(PI * (4.0 * d - t));
    p.y /= 1.0 + 10.0 * d;
    p.z = v;
    return p;
}

// float3 Sphere (float u, float v, float t)
// {
//     // float r = 0.9 + 0.1 * sin(PI * (6.0 * u + 4.0 * v + t));
//     // float r = 0.9 + 0.1 * sin(PI * (12.0 * u + 8.0 * v + t));
//     // float r = 0.9 + 0.1 * sin(PI * (24.0 * u + 16.0 * v + t));
//     float r = 0.9 + 0.1 * sin(PI * ((6.0 * _Progress) * u + (4.0 * _Progress) * v + t));
//     float s = r * cos(0.5 * PI * v);
//     float3 p;
//     p.x = s * sin(PI * u);
//     p.y = r * sin(0.5 * PI * v);
//     p.z = s * cos(PI * u);
//     return p;
// }
//
// float3 Tours (float u, float v, float t)
// {
//     // float r1 = 0.7 + 0.1 * sin(PI * (6.0 * u + 0.5 * t));
//     // float r2 = 0.15 + 0.05 * sin(PI * (8.0 * u + 4.0 * v + 2.0 * t));
//     
//     // float r1 = 0.7 + 0.1 * sin(PI * (8.0 * u + 0.5 * t));
//     // float r2 = 0.15 + 0.05 * sin(PI * (16.0 * u + 8.0 * v + 3.0 * t));
//     
//     // float r1 = 0.7 + 0.1 * sin(PI * (10.0 * u + 0.5 * t));
//     // float r2 = 0.15 + 0.05 * sin(PI * (32.0 * u + 16.0 * v + 4.0 * t));
//     
//     float r1 = 0.7 + 0.1 * sin(PI * ((6.0 + _Progress * 2.0) * u + 0.5 * t));
//     float r2 = 0.15 + 0.05 * sin(PI * ((8.0 * _Progress) * u + ((4.0 * _Progress)) * v + (2.0 + _Progress) * t));
//     float s = r2 * cos(PI * v) + r1;
//     float3 p;
//     p.x = s * sin(PI * u);
//     p.y = r2 * sin(PI * v);
//     p.z = s * cos(PI * u);
//     return p;
// }


float3 ToursA(float u, float v, float t)
{
    float r1 = 0.75f;
    float r2 = 0.25f;
    float s = r1 + r2 * cos(PI * v);
    float3 p;
    p.x = s * sin(PI * u);
    p.y = r2 * sin(PI * v);
    p.z = s * cos(PI * u);
    return p;
}


