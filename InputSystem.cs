using OpenTK.Mathematics;
using OpenTK.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

public static class InputManager
{
    static bool camLocked;
    // KeyboardState inpK;
    // MouseState inpM;
    // float deltaTime;
    // Action Close;
    // Scene scene;

    // public InputManager(KeyboardState inK, MouseState inM, float e, Action Close, Scene sc)
    // {
    //     inpK = inK;
    //     inpM = inM;

    // }
    public static void InputCheck(KeyboardState inpK, MouseState inpM, Camera camera, ref float speed, float e, Action Close, Scene scene)
    {
        if(!camLocked)
        {
            camera.Yaw += inpM.Delta.X * Constants.MouseSensibility;
            camera.Pitch -= inpM.Delta.Y * Constants.MouseSensibility;
            speed += inpM.ScrollDelta.Y;
            camera.UpdateVectors();
        }

        if (inpK.IsKeyDown(Keys.W))
        {
            camera.Position += camera.Front * speed * e;
        }
        if (inpK.IsKeyDown(Keys.S))
        {
            camera.Position -= camera.Front * speed * e;
        }
        if (inpK.IsKeyDown(Keys.D))
        {
            camera.Position += camera.Right * speed * e;
        }
        if (inpK.IsKeyDown(Keys.A))
        {
            camera.Position -= camera.Right * speed * e;
        }
        if (inpK.IsKeyDown(Keys.E))
        {
            camera.Position += camera.Up * speed * e;
        }
        if (inpK.IsKeyDown(Keys.Q))
        {
            camera.Position -= camera.Up * speed * e;
        }
        if (inpK.IsKeyPressed(Keys.LeftShift))
        {
            speed *= 2;
        }
        else if(inpK.IsKeyReleased(Keys.LeftShift))
        {
            speed /= 2;
        }
        if (inpK.IsKeyPressed(Keys.LeftControl))
        {
            speed /= 2;
        }
        else if(inpK.IsKeyReleased(Keys.LeftControl))
        {
            speed *= 2;
        }
        if (inpK.IsKeyPressed(Keys.Escape))
        {
            Close();
        }
        if(inpM.IsButtonPressed(MouseButton.Left))
        {
            if(inpK.IsKeyDown(Keys.LeftControl))
            {
                scene.Deselect();
            }
            else
            {
                Ray ray = Raycaster.GetRayFromScreen(Constants.ScreenSize.X/2, Constants.ScreenSize.Y/2, camera.GetViewMatrix(), camera.Projection, camera.Position);
                foreach(GameObject i in scene.List)
                {
                    if(Raycaster.RayIntersectsSphere(ray, i.Transform.Position, 1, out float distance))
                    {
                        scene.Select(i);
                    }
                }
            }
        }
        if(inpM.IsButtonDown(MouseButton.Left))
        {
            if(scene.Selected == null) return;
            camLocked = inpK.IsKeyDown(Keys.R);
            if(!inpK.IsKeyDown(Keys.R))
            {
                Ray ray = Raycaster.GetRayFromScreen(Constants.ScreenSize.X/2, Constants.ScreenSize.Y/2, camera.GetViewMatrix(), camera.Projection, camera.Position);
                Vector3 hand = 5 * ray.Direction + ray.Origin;
                scene.Selected.Transform.Position = hand;
                Console.WriteLine(scene.Selected.Transform.Position);
            }
            else
            {
                if(inpK.IsKeyDown(Keys.X))
                {
                    scene.Selected.Transform.Rotation.Y += inpM.Delta.X/100 + inpM.Delta.Y/100;
                }
                else if(inpK.IsKeyDown(Keys.Y))
                {
                    scene.Selected.Transform.Rotation.Z += inpM.Delta.X/100 + inpM.Delta.Y/100;
                }
                else
                {
                    scene.Selected.Transform.Rotation.Y += inpM.Delta.X/100;
                    scene.Selected.Transform.Rotation.Z += inpM.Delta.Y/100;
                }
            }
        }
        if (inpK.IsKeyPressed(Keys.Delete))
        {
            if(scene.Selected != null)
            {
                scene.Destroy(scene.Selected);
            }
        }
    }
}