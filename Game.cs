using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using System;
using System.IO;
using System.Drawing;

namespace Engine;

public class Game : GameWindow
{
    Shader shader;
    Scene mainGame;
    Camera camera = new Camera();
    float speed;

    PointLight[] lights;


    public Game(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
        : base(gameSettings, nativeSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        mainGame = new Scene();
        shader = new Shader();
        GL.ClearColor(Constants.bgColor[0],Constants.bgColor[1],Constants.bgColor[2], Constants.bgColor[3]);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace); 
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
 
        Scene.shader = shader;
        
        ObjParser objParser = new ObjParser();
        lights = new PointLight[]
        {
            new PointLight(new Vector3(0,10,0), new Vector3(1,1,1), 1f, "1l", mainGame, objParser.LoadMesh("./models/cube.obj")),
        };
        DirLight.Rotation = new Vector3(-0.5f,-1,0);
        DirLight.Intensity = 0.41f;
        DirLight.Color = new Vector3(1,1,1);
        GameObject map = new GameObject("plane", objParser.LoadMesh("./models/textureField.obj"), mainGame, scale: new Vector3(10));

        GameObject multiObj = new GameObject("multy", objParser.LoadMesh("./models/multi-object.obj"), mainGame, position: new Vector3(0,5,0));
        
        camera.Position = new Vector3( 0, 5, 5);
        CursorState = CursorState.Grabbed;
        speed = Constants.Speed;

        
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

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
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        shader.Use();

        //render objects
        shader.SetLights(lights);
        foreach(GameObject i in mainGame.List)
        {
            i.Render(shader);
            i.Update((float)args.Time);
        }
        //render camera
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetVector3("viewPos", camera.Position);
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
