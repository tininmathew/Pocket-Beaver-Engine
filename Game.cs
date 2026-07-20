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
        shader = new Shader();
        mainGame = new Scene(shader, camera);
        GL.ClearColor(Constants.bgColor[0],Constants.bgColor[1],Constants.bgColor[2], Constants.bgColor[3]);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace); 
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Multisample);
        
        lights = new PointLight[]
        {
            new PointLight(new Vector3(3,5,0), new Vector3(1,1,1), 1f, "1l", mainGame, ObjParser.LoadMesh("./models/quad.obj")),
        };
        DirLight.Rotation = new Vector3(-0.5f,-1,0);
        DirLight.Intensity = 0.41f;
        DirLight.Color = new Vector3(1,1,1);
        GameObject map = new GameObject("plane", ObjParser.LoadMesh("./models/textureField.obj"), mainGame, scale: new Vector3(10));
        

        GameObject multiObj = new GameObject("multy", ObjParser.LoadMesh("models/multi-object.obj"), mainGame, position: new Vector3(0,5,0), rotation: new Vector3(0,-90,0));


        GameObject cross = new GameObject("cross", ObjParser.LoadMesh("./resources/cross.png", MeshType.UI), mainGame, 
        position: new Vector3(0, 0, -1f), scale: new Vector3(32,32,0));
        GameObject Text = new GameObject("TEXT", ObjParser.LoadMesh("resources/fallback texture.bmp", MeshType.UI), mainGame, 
        position: new Vector3(-1800/2, -980/2, -1f));
        Text.AddComponent(new Text("Text"));
        
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
        foreach(GameObject i in mainGame.List)
        {
            i.Update((float)e.Time);
        }
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.SetLights(lights);
        foreach(GameObject i in mainGame.List)
        {
            i.Render(shader);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetVector3("viewPos", camera.Position);
            if(i.Mesh?.type == MeshType.UI)
            {
                shader.SetMatrix4("projection",camera.OrthoProjection);
            }
            else
            {
                shader.SetMatrix4("projection",camera.Projection);
            }
            
        }
        

        SwapBuffers();
        mainGame.Cleanup();
    }
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}
