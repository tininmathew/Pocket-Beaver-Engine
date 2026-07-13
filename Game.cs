using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;
using System;
using System.IO;
using System.Drawing;
using Engine.Scripts;

namespace Engine;

public class Game : GameWindow
{
    Shader shader;
    Scene mainGame;
    Camera camera = new Camera();
    float speed;
    ObjParser objParser;

    PointLight[] lights;


    public Game(GameWindowSettings gameSettings, NativeWindowSettings nativeSettings)
        : base(gameSettings, nativeSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        VSync = VSyncMode.On; 
        mainGame = new Scene();
        shader = new Shader();
        GL.ClearColor(Constants.bgColor[0],Constants.bgColor[1],Constants.bgColor[2], Constants.bgColor[3]);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace); 
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
 
        Scene.shader = shader;
        
        objParser = new ObjParser();
        lights = new PointLight[]
        {
            new PointLight(new Vector3(0,10,0), new Vector3(1,1,1), 1f, "1l", mainGame, objParser.LoadMesh("./models/quad.obj")),
        };
        DirLight.Rotation = new Vector3(-0.5f,-1,0);
        DirLight.Intensity = 0.41f;
        DirLight.Color = new Vector3(1,1,1);
        GameObject map = new GameObject("plane", objParser.LoadMesh("./models/textureField.obj"), mainGame, scale: new Vector3(100));
        

        GameObject multiObj = new GameObject("multy", objParser.LoadMesh("models/multi-object.obj"), mainGame, position: new Vector3(0,5,0), rotation: new Vector3(0,-90,0));


        //GameObject icon = new GameObject("icon", objParser.LoadMesh("./models/quad.obj"), mainGame, position: new Vector3(0,7,0));
        //icon.AddComponent(new Rotater());
        //icon.AddComponent(new OverrideTexture("resources/code.png", 0));
        
        camera.Position = new Vector3( 0, 5, 5);
        CursorState = CursorState.Grabbed;
        speed = Constants.Speed;

        
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        camera.Yaw += MouseState.Delta.X * Constants.MouseSensibility;
        camera.Pitch -= MouseState.Delta.Y * Constants.MouseSensibility;
        speed = speed + MouseState.ScrollDelta.Y;
        camera.UpdateVectors();

        KeysCheck((float)e.Time);
    }
    float rot = 0;
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
        if(input.IsKeyDown(Keys.R))
        {
            
            Ray ray = Raycaster.GetRayFromScreen(Constants.ScreenSize.X/2, Constants.ScreenSize.Y/2, camera.GetViewMatrix(), camera.Projection, camera.Position);
            ray.Direction *= 5;
            Random random = new Random();
            GameObject a = new GameObject(random.Next().ToString(), objParser.LoadMesh("models/cube.obj"), mainGame, position: ray.Direction+ray.Origin, rotation: new Vector3(rot,rot,rot), scale: new Vector3(0.1f));
            rot++;
            a.AddComponent(new Remover(5));
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
        shader.SetMatrix4("projection",camera.Projection);

        SwapBuffers();
        mainGame.Cleanup();
    }
}
