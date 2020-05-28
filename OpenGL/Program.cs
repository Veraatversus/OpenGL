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
      var gl = new GLEnv(640, 480, "Interactive Late Night Coding");

      var vertices = new[] {
        -0.6f, -0.4f, -1.0f, 1.0f, 0.0f, 0.0f,
        0.6f, -0.4f, -1.0f, 0.0f, 1.0f, 0.0f,
        0.0f,  0.6f, -1.0f, 0.0f, 0.0f, 1.0f
      };
      var verticesHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);

    
      GL.GenBuffers(1, out uint vertexBuffer);
      GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
      GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertices.Length), verticesHandle.AddrOfPinnedObject(), BufferUsageHint.StaticDraw);

      var program = GLProgram.CreateFromFile("Shader/vertexShader.glsl", "Shader/fragmentShader.glsl");

      var mvpLocation = program.GetUniformLocation("MVP");
      var posLocation = program.GetAttribLocation("vPos");
      var colorLocation = program.GetAttribLocation("vCol");
      GLProgram.CheckAndThrow();

      GL.EnableVertexAttribArray(posLocation);
      GL.VertexAttribPointer(posLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);

      GL.EnableVertexAttribArray(colorLocation);
      GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, sizeof(float) * 3);


      do {

       var dimensions = gl.GetFramebufferSize();

        GL.ClearColor(0, 0, 1, 0);
        GL.Clear(ClearBufferMask.ColorBufferBit /*| ClearBufferMask.DepthBufferBit*/);

        GL.Viewport(0, 0, dimensions.Width, dimensions.Height);


        var p = Matrix.CreatePerspectiveFieldOfView(MathR.MathR.ConvertDegreesToRadians(90), dimensions.Aspect, 0.0001f, 1000.0f);
        var m = Matrix.CreateRotationZ(Convert.ToSingle(GetTime()));
        var mvp = m * p;

        program.Use();

        GL.UniformMatrix4(mvpLocation, false, ref mvp);

        GL.DrawArrays(BeginMode.Triangles, 0, 3);

        gl.EndOfFrame();
      } while (!gl.ShouldClose());

      gl.Dispose();
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
