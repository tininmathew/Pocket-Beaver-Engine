using OpenTK.Mathematics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using Engine.Scripts;

namespace Engine;

public enum TranslateMode
{
    Moving, 
    Rotating,
    Scaling
}
public enum TransformOrientation
{
    Local, 
    Global
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
    TransformOrientation orientation;

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
        if (e.Key == Keys.P)
        {
            if(GL.IsEnabled(EnableCap.Multisample))
            {
                GL.Disable(EnableCap.Multisample); 
            }
            else
            {
                GL.Enable(EnableCap.Multisample); 
            }
        }
        if (e.Key == Keys.G)
        {
            mode = TranslateMode.Moving;
            (scene.Find("TEXT").GetComponent(typeof(Text)) as Text)?.TEXT ="Moving Mode";
        }
        if (e.Key == Keys.R)
        {
            mode = TranslateMode.Rotating;
            (scene.Find("TEXT").GetComponent(typeof(Text)) as Text)?.TEXT ="Rotating Mode";
        }
        if (e.Key == Keys.S)
        {
            mode = TranslateMode.Scaling;
            (scene.Find("TEXT").GetComponent(typeof(Text)) as Text)?.TEXT ="Scaling Mode";
        }
        if (e.Key == Keys.Tab)
        {
            if((int)orientation < Enum.GetValues(typeof(TransformOrientation)).Length-1)
            {
                orientation++;
            }
            else
            {
                orientation = 0;
            }
            Console.WriteLine(orientation);
        }
    }
    void MoveAlong(Vector3 along)
    {
        if(scene.Selected == null) return;

        switch(mode)
        {
            case TranslateMode.Moving:
                if(orientation == TransformOrientation.Local)
                {
                    scene.Selected.Transform.Position += scene.Selected.Transform.Rotation * (-along * (inpM.Delta.X/100 + inpM.Delta.Y/100));
                }
                else
                {
                    scene.Selected.Transform.Position += along * (inpM.Delta.X/100 + inpM.Delta.Y/100);
                }
                
                break;
            case TranslateMode.Rotating:
                if(orientation == TransformOrientation.Local)
                {
                    scene.Selected.Transform.Rotation *= Quaternion.FromAxisAngle(along, inpM.Delta.X/100 + inpM.Delta.Y/100);
                }
                else
                {
                    scene.Selected.Transform.Rotation = Quaternion.FromAxisAngle(along, inpM.Delta.X/100 + inpM.Delta.Y/100) * scene.Selected.Transform.Rotation;
                }
                
                if(inpM.IsButtonPressed(MouseButton.Right))
                {
                    scene.Selected.Transform.Angles = Vector3.Zero;
                }
                break;
            case TranslateMode.Scaling:
                scene.Selected.Transform.Scale += along * (inpM.Delta.X/100 + inpM.Delta.Y/100);
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
            Quaternion pitchRotation = Quaternion.FromAxisAngle(Vector3.UnitX, -inpM.Delta.Y * Constants.MouseSensibility);
            Quaternion yawRotation = Quaternion.FromAxisAngle(Vector3.UnitY, -inpM.Delta.X * Constants.MouseSensibility);
            camera.Rotation = yawRotation * camera.Rotation * pitchRotation;
            camera.Rotation = Quaternion.Normalize(camera.Rotation);
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
                    if(Raycaster.RayIntersectsSphere(ray, i.Transform.WorldPosition, 1, out float distance))
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