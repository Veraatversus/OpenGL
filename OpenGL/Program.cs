using MathR;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System;
using static Pencil.Gaming.Glfw;
namespace OpenGL {
  public static class Program {

    private static int Main(string[] _) {
      using var gl = new GLEnv(640, 480, "Interactive Late Night Coding");

      var sphere = Tesselation.GenSphere(new Vec3(0, 0, 0), 1, 100, 100);

      using var vbPos = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.Vertices, 3);

      using var vbNorm = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.Normals, 3);

      using var vbTan = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.Tangents, 3);

      using var vbTc = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.TexCoords, 2);

      using var ib = new GLBuffer(BufferTarget.ElementArrayBuffer)
        .SetData(sphere.Indices);


      var image = BMP.Load("Pics/albedo.bmp");
      var lena = new GLTexture2D(image.Width, image.Height, image.ComponentCount, TextureMagFilter.Linear, TextureMinFilter.Linear);
      lena.SetData(image.data);

      var normals = BMP.Load("Pics/normal.bmp");
      var normalMap = new GLTexture2D(normals.Width, normals.Height, normals.ComponentCount, TextureMagFilter.Linear, TextureMinFilter.Linear);
      normalMap.SetData(normals.data);

      var program = GLProgram.CreateFromFile("Shader/vertex.glsl", "Shader/fragment.glsl");

      var mvpLocation = program.GetUniformLocation("MVP");
      var mLocation = program.GetUniformLocation("M");
      var mitLocation = program.GetUniformLocation("Mit");
      var invVLocation = program.GetUniformLocation("invV");


      var posLocation = program.GetAttribLocation("vPos");
      var tanLocation = program.GetAttribLocation("vTan");
      var tcLocation = program.GetAttribLocation("vTc");
      var normLocation = program.GetAttribLocation("vNorm");

      var lpLocation = program.GetUniformLocation("vLightPos");
      var texLocation = program.GetUniformLocation("textureSampler");
      var normMapLocation = program.GetUniformLocation("normalSampler");

      GLProgram.CheckAndThrow();

      vbPos.ConnectVertexAttrib(posLocation, 3);
      vbNorm.ConnectVertexAttrib(normLocation, 3);
      vbTan.ConnectVertexAttrib(tanLocation, 3);
      vbTc.ConnectVertexAttrib(tcLocation, 2);
      GLProgram.CheckAndThrow();

      GL.Enable(EnableCap.CullFace);
      GL.CullFace(CullFaceMode.Back);

      GL.Enable(EnableCap.DepthTest);
      GL.DepthFunc(DepthFunction.Less);

      GL.ClearDepth(1);
      GL.ClearColor(0.5f, 0.5f, 1, 1);

      GLProgram.CheckAndThrow();

      do {

        var dimensions = gl.GetFramebufferSize();
        GL.Viewport(0, 0, dimensions.Width, dimensions.Height);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        program.Enable();
        program.SetUniform(lpLocation, new Vec3(0, 0, 2));
        program.SetTexture(normMapLocation, normalMap, 0);
        program.SetTexture(texLocation, lena, 1);


        var m = Mat4.Translation(new Vec3(0.0f, 0.0f, 0.2f)) * Mat4.RotationX(Convert.ToSingle(GetTime() * 57)) * Mat4.Translation(new Vec3(0.2f, 0.0f, 0.0f)) * Mat4.RotationY(Convert.ToSingle(GetTime() * 17));
        var v = Mat4.LookAt(new Vec3(0, 0, 2), new Vec3(0, 0, 0), new Vec3(0, 1, 0));
        var p = Mat4.Perspective(90, dimensions.Aspect, 0.0001f, 100);
        var mvp = m * v * p;
        program.SetUniform(mvpLocation, mvp);
        program.SetUniform(mLocation, m);
        program.SetUniform(mitLocation, Mat4.Inverse(m), true);
        program.SetUniform(invVLocation, Mat4.Inverse(v));

        GL.DrawElements(BeginMode.Triangles, sphere.Indices.Length, DrawElementsType.UnsignedInt, 0);

        gl.EndOfFrame();
      } while (!gl.ShouldClose());

      return 0;
    }
  }
}
