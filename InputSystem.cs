using OpenTK.Mathematics;
using OpenTK.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

namespace Engine;

public enum TranslateMode
{
    Moving, 
    Rotating,
    Scaling
}

public class InputManager
{
    static bool camLocked;
    KeyboardState inpK;
    MouseState inpM;
    Action Close;
    Scene scene;
    Camera camera;
    TranslateMode mode;

    public InputManager(KeyboardState inK, MouseState inM, Action close, Scene sc, Camera cam)
    {
        inpK = inK;
        inpM = inM;
        Close = close;
        scene = sc;
        camera = cam;
    }
    public Vector3 GetMoveDirection()
    {
        Vector3 Out = new Vector3(0,0,0);
        if (inpK.IsKeyDown(Keys.W))
        {
            Out += camera.Front;
        }
        if (inpK.IsKeyDown(Keys.S))
        {
            Out -= camera.Front;
        }
        if (inpK.IsKeyDown(Keys.D))
        {
            Out += camera.Right;
        }
        if (inpK.IsKeyDown(Keys.A))
        {
            Out -= camera.Right;
        }
        if (inpK.IsKeyDown(Keys.E))
        {
            Out += camera.Up;
        }
        if (inpK.IsKeyDown(Keys.Q))
        {
            Out -= camera.Up;
        }
        return Out;
    }
    public void OnKeyDown(KeyboardKeyEventArgs e)
    {
        if (e.Key == Keys.LeftShift)
        {
            Game.speed *= 2;
        }
        if (e.Key == Keys.LeftControl)
        {
            Game.speed /= 2;
        }
        if (e.Key == Keys.Delete)
        {
            if(scene.Selected != null)
            {
                scene.Destroy(scene.Selected);
            }
        }
        if (e.Key == Keys.G)
        {
            mode = TranslateMode.Moving;
        }
        if (e.Key == Keys.R)
        {
            mode = TranslateMode.Rotating;
        }
        if (e.Key == Keys.S)
        {
            mode = TranslateMode.Scaling;
        }
    }
    void MoveAlong(Vector3 along)
    {
        if(scene.Selected == null) return;

        switch(mode)
        {
            case TranslateMode.Moving:
                scene.Selected.Transform.Position += along * (inpM.Delta.X/100 + inpM.Delta.X/100);
                break;
            case TranslateMode.Rotating:
                scene.Selected.Transform.Rotation += along * (inpM.Delta.X/100 + inpM.Delta.X/100);
                if(inpM.IsButtonPressed(MouseButton.Right))
                {
                    scene.Selected.Transform.Rotation = Vector3.Zero;
                }
                break;
            case TranslateMode.Scaling:
                scene.Selected.Transform.Scale += along * (inpM.Delta.X/100 + inpM.Delta.X/100);
                if(inpM.IsButtonPressed(MouseButton.Right))
                {
                    scene.Selected.Transform.Scale = Vector3.One;
                }
                break;
        }
    }
    public void OnKeyUp(KeyboardKeyEventArgs e)
    {
        if(e.Key == Keys.LeftShift)
        {
            Game.speed /= 2;
        }
        if(e.Key == Keys.LeftControl)
        {
            Game.speed *= 2;
        }
        if (e.Key == Keys.Escape)
        {
            Close();
        }
    }
    public void Mouse()
    {
        if(!camLocked)
        {
            camera.Yaw += inpM.Delta.X * Constants.MouseSensibility;
            camera.Pitch -= inpM.Delta.Y * Constants.MouseSensibility;
            Game.speed += inpM.ScrollDelta.Y;
            camera.UpdateVectors();
        }
    }
    public void InputCheck()
    {
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
        }
        if (inpK.IsKeyDown(Keys.X))
        {
            MoveAlong(Vector3.UnitX);
            camLocked = true;
        }
        if (inpK.IsKeyDown(Keys.Y))
        {
            MoveAlong(Vector3.UnitY);
            camLocked = true;
        }
        if (inpK.IsKeyDown(Keys.Z))
        {
            MoveAlong(Vector3.UnitZ);
            camLocked = true;
        }
        if (inpK.IsKeyReleased(Keys.X)|inpK.IsKeyReleased(Keys.Y)|inpK.IsKeyReleased(Keys.Z))
        {
            camLocked = false;
        }
    }
}