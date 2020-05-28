using MathR;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static Pencil.Gaming.Glfw;
namespace OpenGL {
  internal class Program {
    private static GlfwWindowPtr window;

    private static int Main(string[] args) {
      var gLEnv = new GLEnv(640, 480, "Interactive Late Night Coding");

      var vertices = new[] {
        -0.6f, -0.4f, -1.0f, 1.0f, 0.0f, 0.0f,
        0.6f, -0.4f, -1.0f, 0.0f, 1.0f, 0.0f,
        0.0f,  0.6f, -1.0f, 0.0f, 0.0f, 1.0f
      };
      var verticesHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);

    
      GL.GenBuffers(1, out uint vertexBuffer);
      GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertices.Length), verticesHandle.AddrOfPinnedObject(), BufferUsageHint.StaticDraw);
      int nullTerminator = 0;
      var vertexShaderText =
        "#version 110" + Environment.NewLine +
        "uniform mat4 MVP;" +
        "attribute vec3 vPos;" +
        "attribute vec3 vCol;" +
        "varying vec3 color;" +
        "void main() {" +
        " gl_Position = MVP * vec4(vPos, 1.0);" +
        " color = vCol;" +
        "}";

      var vertexShader = GL.CreateShader(ShaderType.VertexShader);
      GL.ShaderSource(vertexShader, vertexShaderText);
      GL.CompileShader(vertexShader);


      var fragmentShaderText =
        "#version 110" + Environment.NewLine +
        "varying vec3 color;" +
        "void main() {" +
        " gl_FragColor = vec4(color, 1.0);" +
        "}";

      var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
      GL.ShaderSource(fragmentShader, fragmentShaderText);
      GL.CompileShader(fragmentShader);

      var program = GL.CreateProgram();
      GL.AttachShader(program, vertexShader);
      GL.AttachShader(program, fragmentShader);
      GL.LinkProgram(program);


      var mvpLocation = GL.GetUniformLocation(program, "MVP");

      var posLocation = GL.GetAttribLocation(program, "vPos");
      GL.EnableVertexAttribArray(posLocation);
      GL.VertexAttribPointer(posLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);

      var colorLocation = GL.GetAttribLocation(program, "vCol");
      GL.EnableVertexAttribArray(colorLocation);
      GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, sizeof(float) * 3);
      CheckError();


      do {

       var dimensions = gLEnv.GetFramebufferSize();

        GL.ClearColor(0, 0, 1, 0);
        GL.Clear(ClearBufferMask.ColorBufferBit /*| ClearBufferMask.DepthBufferBit*/);

        GL.Viewport(0, 0, dimensions.Width, dimensions.Height);
        var p = Matrix.CreatePerspectiveFieldOfView(MathR.MathR.ConvertDegreesToRadians(90), dimensions.Aspect, 0.0001f, 1000.0f);
        var m = Matrix.CreateRotationZ(Convert.ToSingle(GetTime()));
        var mvp = m * p;

        GL.UseProgram(program);
        GL.UniformMatrix4(mvpLocation, false, ref mvp);

        GL.DrawArrays(BeginMode.Triangles, 0, 3);

        gLEnv.EndOfFrame();
      } while (!gLEnv.ShouldClose());

      Terminate();
      verticesHandle.Free();
      return 0;
    }



    private void keyCallback(GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods) {
      if (key == Key.Escape && action == KeyAction.Press) {
        SetWindowShouldClose(window, true);
      }
    }
  }
}
