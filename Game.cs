using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using System;
using System.IO;
using Engine.Graphics;
using System.Drawing;

namespace Engine;

public class Game : GameWindow
{
    Shader shader;
    Scene mainGame;
    Camera camera = new Camera();
    GameObject Object;
    float speed;

    public Game(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
        : base(gameSettings, nativeSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        mainGame = new Scene();
        Mesh quadMesh = new Mesh
        (
            //verts
            [
                new()
                {
                    position = new Vector3(-1f, 1f, 0),
                    normal = new Vector3(0, 0, 1),
                },
                new()
                {
                    position = new Vector3(-1f, -1f, 0),
                    normal = new Vector3(0, 0, 1),
                },
                new()
                {
                    position = new Vector3(1f, -1f, 0),
                    normal = new Vector3(0, 0, 1),
                },
                new()
                {
                    position = new Vector3(1f, 1f, 0),
                    normal = new Vector3(0, 0, 1),
                },
            ],
            [0, 1, 2,
            0, 2, 3]
        );
        ObjParser objParser = new ObjParser();
        objParser.LoadObj("./models/thing.obj");
        Mesh cubeMesh = new Mesh(objParser.Vertices.ToArray(), objParser.VertexIndices.ToArray());
        Object = new GameObject("obj", cubeMesh, mainGame, _scale: new Vector3(10));
        shader = new Shader();
        GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);  
        camera.Position.Y = 3;
        CursorState = CursorState.Grabbed;
        speed = Constants.Speed;
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        //lightPos.Position.X = (float)Math.Sin(e.Time);
        //lightPos.Position.Y = (float)Math.Cos(e.Time);


        mainGame.Find("obj").Transform.Rotation.Y += (float)e.Time;

        camera.Yaw += MouseState.Delta.X * Constants.MouseSensibility;
        camera.Pitch -= MouseState.Delta.Y * Constants.MouseSensibility;
        camera.UpdateVectors();

        KeysCheck((float)e.Time);
    }
    void KeysCheck(float e)
    {
        var input = KeyboardState;
        if (input.IsKeyDown(Keys.W))
        {
            camera.Position += camera.Front * speed*(float)e;
        }
        if (input.IsKeyDown(Keys.S))
        {
            camera.Position -= camera.Front * speed*(float)e;
        }
        if (input.IsKeyDown(Keys.D))
        {
            camera.Position += camera.Right * speed*(float)e;
        }
        if (input.IsKeyDown(Keys.A))
        {
            camera.Position -= camera.Right * speed*(float)e;
        }
        if (input.IsKeyDown(Keys.E))
        {
            camera.Position += camera.Up * speed*(float)e;
        }
        if (input.IsKeyDown(Keys.Q))
        {
            camera.Position -= camera.Up * speed*(float)e;
        }
        if (input.IsKeyPressed(Keys.LeftShift))
        {
            speed *= 2;
        }
        else if(input.IsKeyReleased(Keys.LeftShift))
        {
            speed /= 2;
        }
        if (input.IsKeyPressed(Keys.LeftControl))
        {
            speed /= 2;
        }
        else if(input.IsKeyReleased(Keys.LeftControl))
        {
            speed *= 2;
        }
        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    double anim;
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        shader.Use();

        //render objects
        shader.SetVector3("lightPos", camera.Position);
        foreach(GameObject i in mainGame.List)
        {
            i.Render(shader);
        }
        //render camera
        shader.SetMatrix4("view", camera.GetViewMatrix());
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView
        (
            MathHelper.DegreesToRadians(70f),
            Constants.ScreenSize.X / Constants.ScreenSize.Y,
            0.1f,100f
        );
        shader.SetMatrix4("projection",projection);

        SwapBuffers();
    }
}
