using MathR;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using static Pencil.Gaming.Glfw;
namespace OpenGL {
  internal class Program {
    private static GlfwWindowPtr window;

    private static int Main(string[] args) {
      var gl = new GLEnv(640, 480, "Interactive Late Night Coding");

      //var sphere = new GLBuffer(BufferTarget.ArrayBuffer).SetData(new[] { });
      var sphere = Tesselation.GenSphere(new Vec3(0, 0, 0), 1, 100, 100);

      var vbPos = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.Vertices, 3);

      var vbNormal = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.Normals, 3);

      var ib = new GLBuffer(BufferTarget.ElementArrayBuffer)
        .SetData(sphere.Indices);

      var program = GLProgram.CreateFromFile("Shader/vertexShader.glsl", "Shader/fragmentShader.glsl");

      var mvpLocation = program.GetUniformLocation("MVP");
      var mitLocation = program.GetUniformLocation("Mit");
      var mLocation = program.GetUniformLocation("M");

      var lightPosLocation = program.GetUniformLocation("lightPos");
      var posLocation = program.GetAttribLocation("vPos");
      var normLocation = program.GetAttribLocation("vNormal");
      GLProgram.CheckAndThrow();

      vbPos.ConnectVertexAttrib(posLocation, 3);
      vbNormal.ConnectVertexAttrib(normLocation, 3);

      GL.Enable(EnableCap.DepthTest);
      GL.DepthFunc(DepthFunction.Less);
      GL.ClearDepth(1);
      GL.ClearColor(0, 0, 1, 0);

      GL.Enable(EnableCap.CullFace);
      GL.CullFace(CullFaceMode.Back);


      do {

        var dimensions = gl.GetFramebufferSize();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.Viewport(0, 0, dimensions.Width, dimensions.Height);


        var p = Mat4.Perspective(90, dimensions.Aspect, 0.0001f, 1000.0f);
        var m = Mat4.RotationY(Convert.ToSingle(GetTime() * 33)) * Mat4.RotationZ(Convert.ToSingle(GetTime() * 20));
        var v = Mat4.LookAt(new Vec3(0, 0, 2), new Vec3(0, 0, 0), new Vec3(0, 1, 0));
        var mvp = m * v * p;

        program.Enable();

        program.SetUniform(mvpLocation, mvp);
        program.SetUniform(mitLocation, Mat4.Inverse(m), true);
        program.SetUniform(mLocation, m);
        program.SetUniform(lightPosLocation, new Vec3(0, 2, 2));

        GL.DrawElements(BeginMode.Triangles, sphere.Indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.DrawArrays(BeginMode.Triangles, 0, 3);

        gl.EndOfFrame();
      } while (!gl.ShouldClose());

      gl.Dispose();
      vbPos.Dispose();
      vbNormal.Dispose();
      ib.Dispose();
      return 0;
    }



    private void keyCallback(GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods) {
      if (key == Key.Escape && action == KeyAction.Press) {
        SetWindowShouldClose(window, true);
      }
    }
  }
}
