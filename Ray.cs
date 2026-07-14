using Engine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public struct Ray
{
    public Vector3 Origin;    // Начало луча (позиция камеры)
    public Vector3 Direction; // Направление луча (нормализованное)
}
public static class Raycaster
{
    public static Ray GetRayFromScreen(float mouseX, float mouseY, Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 cameraPosition)
    {
        // Шаг 1: Перевод в Normalized Device Coordinates (NDC) от -1 до 1
        float x = (2.0f * mouseX) / Constants.ScreenSize.X - 1.0f;
        // В OpenGL ось Y на экране направлена вниз, а в NDC — вверх
        float y = 1.0f - (2.0f * mouseY) / Constants.ScreenSize.Y; 

        // Шаг 2: Создание вектора в Clip Space
        // Для ближней плоскости отсечения z = -1.0f, w = 1.0f
        Vector4 rayClip = new Vector4(x, y, -1.0f, 1.0f);

        // Шаг 3: Перевод в Eye Space (пространство камеры)
        Matrix4 invProjection = Matrix4.Invert(projectionMatrix);
        Vector4 rayEye = Vector4.TransformRow(rayClip, invProjection);
        // Направляем луч вперед (z = -1), сбрасываем гомогенную координату w
        rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

        // Шаг 4: Перевод в World Space (мировое пространство)
        Matrix4 invView = Matrix4.Invert(viewMatrix);
        Vector4 rayWorld4 = Vector4.TransformRow(rayEye, invView);
        
        Vector3 rayWorldDir = new Vector3(rayWorld4.X, rayWorld4.Y, rayWorld4.Z);
        rayWorldDir.Normalize(); // Обязательно нормализуем

        // Шаг 5: Формируем луч
        return new Ray
        {
            Origin = cameraPosition, // Точка старта — сама камера
            Direction = rayWorldDir  // Вектор направления
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