using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
    public static float speed;
    InputManager input;

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
        
        lights = new PointLight[]
        {
            new PointLight(new Vector3(3,5,0), new Vector3(1,1,1), 1f, "1l", mainGame, ObjParser.LoadMesh("./models/quad.obj")),
        };
        DirLight.Rotation = new Vector3(-0.5f,-1,0);
        DirLight.Intensity = 0.41f;
        DirLight.Color = new Vector3(1,1,1);
        GameObject map = new GameObject("plane", ObjParser.LoadMesh("./models/textureField.obj"), mainGame, scale: new Vector3(10));
        

        GameObject multiObj = new GameObject("multy", ObjParser.LoadMesh("models/multi-object.obj"), mainGame, position: new Vector3(0,5,0), rotation: new Vector3(0,-90,0));


        GameObject icon = new GameObject("icon", ObjParser.LoadMesh("./models/quad.obj"), mainGame, position: new Vector3(0,5,3), rotation: new Vector3(90,0,0));
        //icon.AddComponent(new Rotater("y"));
        icon.AddComponent(new OverrideTexture("resources/code.png", 0));
        
        camera.Position = new Vector3( 0, 5, 5);
        CursorState = CursorState.Grabbed;
        speed = Constants.Speed;

        input = new InputManager(KeyboardState, MouseState, Close, mainGame, camera);
        KeyDown += input.OnKeyDown;
        KeyUp += input.OnKeyUp;
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        input.Mouse();
        input.InputCheck();
        camera.Position += input.GetMoveDirection() * speed * (float)e.Time;
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
