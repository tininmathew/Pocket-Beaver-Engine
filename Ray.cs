using Engine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public struct Ray
{
    public Vector3 Origin;
    public Vector3 Direction;
}
public static class Raycaster
{
    public static Ray GetRayFromScreen(float mouseX, float mouseY, Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 cameraPosition)
    {
        float x = (2.0f * mouseX) / Constants.ScreenSize.X - 1.0f;
        float y = 1.0f - (2.0f * mouseY) / Constants.ScreenSize.Y; 

        Vector4 rayClip = new Vector4(x, y, -1.0f, 1.0f);

        Matrix4 invProjection = Matrix4.Invert(projectionMatrix);
        Vector4 rayEye = Vector4.TransformRow(rayClip, invProjection);
        rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

        Matrix4 invView = Matrix4.Invert(viewMatrix);
        Vector4 rayWorld4 = Vector4.TransformRow(rayEye, invView);
        
        Vector3 rayWorldDir = new Vector3(rayWorld4.X, rayWorld4.Y, rayWorld4.Z);
        rayWorldDir.Normalize();

        return new Ray
        {
            Origin = cameraPosition,
            Direction = rayWorldDir
        };
    }
    public static bool RayIntersectsSphere(Ray ray, Vector3 sphereCenter, float sphereRadius, out float distance)
    {
        distance = 0f;
        Vector3 oc = ray.Origin - sphereCenter;
        
        float b = Vector3.Dot(oc, ray.Direction);
        float c = Vector3.Dot(oc, oc) - sphereRadius * sphereRadius;
        
        if (b > 0.0f && c > 0.0f) return false;
        
        float discriminant = b * b - c;
        
        if (discriminant < 0.0f) return false;
        
        distance = -b - (float)Math.Sqrt(discriminant);
        
        if (distance < 0.0f) distance = 0.0f;
        
        return true;
    }

}