using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System;
using static Pencil.Gaming.Glfw;
namespace OpenGL {
  public static class Program {
    static bool bounce = true;
    static ParticleSystem particleSystem;
    static readonly Vec3[] colors = new[] { ParticleSystem.RandomColor, (1, 0, 0), (0, 1, 0), (0, 0, 1), (1, 1, 0), (0, 1, 1), (1, 0, 1) };
    static uint currentColor;
    static readonly Vec3[] accelerations = new Vec3[] { (0, 0, 0), (0, -0.005f, 0), (0, 0.005f, 0) };
    static uint currentAcceleration;

    private static int Main(string[] _) {
      using var gl = new GLEnv(640, 480, "Interactive Late Night Coding");
      gl.OnKeyPressed += KeyCallback;
      var sphere = Tesselation.GenSphere(new Vec3(0, 0, 0), 0.4f, 50, 50);

      using var vbBallPos = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.Vertices, 3);

      using var vbBallNorm = new GLBuffer(BufferTarget.ArrayBuffer)
             .SetData(sphere.Normals, 3);

      using var vbBallTan = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.Tangents, 3);

      using var vbBallTc = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(sphere.TexCoords, 2);

      using var ibBall = new GLBuffer(BufferTarget.ElementArrayBuffer)
        .SetData(sphere.Indices);

      var ballAlbedoImage = BMP.Load("Pics/ballAlbedo.bmp");
      using var ballAlbedo = new GLTexture2D(ballAlbedoImage, TextureMagFilter.Linear, TextureMinFilter.Linear);

      var ballNormalImage = BMP.Load("Pics/ballNormal.bmp");
      using var ballNormalMap = new GLTexture2D(ballNormalImage, TextureMagFilter.Linear, TextureMinFilter.Linear);

      // generate wall geoemtry (for all 5 walls)
      var square = Tesselation.GenRectangle(new Vec3(0, 0, 0), 4.0f, 4.0f);
      using var vbWallPos = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(square.Vertices, 3);
      using var vbWallNorm = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(square.Normals, 3);
      using var vbWallTan = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(square.Tangents, 3);
      using var vbWallTc = new GLBuffer(BufferTarget.ArrayBuffer)
        .SetData(square.TexCoords, 2);
      using var ibWall = new GLBuffer(BufferTarget.ElementArrayBuffer)
        .SetData(square.Indices);

      // load brick wall textures (sides
      var brickWallAlbedoImage = BMP.Load("Pics/brickWallAlbedo.bmp");
      using var brickWallAlbedo = new GLTexture2D(brickWallAlbedoImage, TextureMagFilter.Linear, TextureMinFilter.Linear);

      var brickWallNormalImage = BMP.Load("Pics/brickWallNormal.bmp");
      using var brickWallNormalMap = new GLTexture2D(brickWallNormalImage, TextureMagFilter.Linear, TextureMinFilter.Linear);

      // load brick wall textures (floor)
      var floorAlbedoImage = BMP.Load("Pics/floorAlbedo.bmp");
      using var floorAlbedo = new GLTexture2D(floorAlbedoImage, TextureMagFilter.Linear, TextureMinFilter.Linear);

      var floorNormalImage = BMP.Load("Pics/floorNormal.bmp");
      using var floorNormalMap = new GLTexture2D(floorNormalImage, TextureMagFilter.Linear, TextureMinFilter.Linear);

      // load brick wall textures (ceiling)
      var ceilingAlbedoImage = BMP.Load("Pics/ceilingAlbedo.bmp");
      using var ceilingAlbedo = new GLTexture2D(ceilingAlbedoImage, TextureMagFilter.Linear, TextureMinFilter.Linear);

      var ceilingNormalImage = BMP.Load("Pics/ceilingNormal.bmp");
      using var ceilingNormalMap = new GLTexture2D(ceilingNormalImage, TextureMagFilter.Linear, TextureMinFilter.Linear);


      // setup normal mapping shader
      var progNormalMap = GLProgram.CreateFromFile("Shader/normalMapVertex.glsl", "Shader/normalMapFragment.glsl");
      var mvpLocationNormalMap = progNormalMap.GetUniformLocation("MVP");
      var mLocationNormalMap = progNormalMap.GetUniformLocation("M");
      var mitLocationNormalMap = progNormalMap.GetUniformLocation("Mit");
      var invVLocationNormalMap = progNormalMap.GetUniformLocation("invV");
      var posLocationNormalMap = progNormalMap.GetAttributeLocation("vPos");
      var tanLocationNormalMap = progNormalMap.GetAttributeLocation("vTan");
      var tcLocationNormalMap = progNormalMap.GetAttributeLocation("vTc");
      var normLocationNormalMap = progNormalMap.GetAttributeLocation("vNorm");
      var lpLocationNormalMap = progNormalMap.GetUniformLocation("vLightPos");
      var texRescaleLocationNormalMap = progNormalMap.GetUniformLocation("texRescale");
      var texLocationNormalMap = progNormalMap.GetUniformLocation("textureSampler");
      var normMapLocationNormalMap = progNormalMap.GetUniformLocation("normalSampler");

      GLProgram.CheckAndThrow();

      GL.Enable(EnableCap.CullFace);
      GL.CullFace(CullFaceMode.Back);

      GL.Enable(EnableCap.DepthTest);
      GL.DepthFunc(DepthFunction.Less);

      GL.ClearDepth(1);
      GL.ClearColor(0.5f, 0.5f, 1, 1);

      GLProgram.CheckAndThrow();

      var lookFromVec = new Vec3(0, 0, 5);
      var lookAtVec = new Vec3(0, 0, 0);
      var upVec = new Vec3(0, 1, 0);
      var v = Mat4.LookAt(lookFromVec, lookAtVec, upVec);

      do {

        // setup viewport and clear buffers
        var dim = gl.GetFramebufferSize();
        GL.Viewport(0, 0, dim.Width, dim.Height);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        var p = Mat4.Perspective(45, dim.Aspect, 0.0001f, 100);

        // animate lightpos
        progNormalMap.Enable();

        var lightPos = Mat4.RotationY(Convert.ToSingle(GetTime() * 55)) * new Vec3(0, 0, 1);
        progNormalMap.SetUniform(lpLocationNormalMap, lightPos);

        // ************* the ball

        // setup texures
        progNormalMap.SetTexture(normMapLocationNormalMap, ballNormalMap, 0);
        progNormalMap.SetTexture(texLocationNormalMap, ballAlbedo, 1);

        // bind geometry
        vbBallPos.ConnectVertexAttrib(posLocationNormalMap, 3);
        vbBallNorm.ConnectVertexAttrib(normLocationNormalMap, 3);
        vbBallTan.ConnectVertexAttrib(tanLocationNormalMap, 3);
        vbBallTc.ConnectVertexAttrib(tcLocationNormalMap, 2);
        ibBall.Bind();

        // setup transformations
        var mBall = Mat4.Translation(new Vec3(0.0f, 0.0f, 0.8f)) * Mat4.RotationX(Convert.ToSingle(GetTime() * 157)) * Mat4.Translation(new Vec3(0.8f, 0.0f, 0.0f)) * Mat4.RotationY(Convert.ToSingle(GetTime() * 47));
        progNormalMap.SetUniform(texRescaleLocationNormalMap, 1.0f);
        progNormalMap.SetUniform(mvpLocationNormalMap, mBall * v * p);
        progNormalMap.SetUniform(mLocationNormalMap, mBall);
        progNormalMap.SetUniform(mitLocationNormalMap, Mat4.Inverse(mBall), true);
        progNormalMap.SetUniform(invVLocationNormalMap, Mat4.Inverse(v));

        // render geometry
        GL.DrawElements(BeginMode.Triangles, sphere.Indices.Length, DrawElementsType.UnsignedInt, 0);


        // ************* the left wall

        // setup texures (shader is already active)
        progNormalMap.SetTexture(normMapLocationNormalMap, brickWallNormalMap, 0);
        progNormalMap.SetTexture(texLocationNormalMap, brickWallAlbedo, 1);

        // bind geometry
        vbWallPos.ConnectVertexAttrib(posLocationNormalMap, 3);
        vbWallNorm.ConnectVertexAttrib(normLocationNormalMap, 3);
        vbWallTan.ConnectVertexAttrib(tanLocationNormalMap, 3);
        vbWallTc.ConnectVertexAttrib(tcLocationNormalMap, 2);
        ibWall.Bind();

        var mLeftWall = Mat4.RotationY(90) * Mat4.Translation(-2.0f, 0.0f, 0.0f);
        progNormalMap.SetUniform(mvpLocationNormalMap, mLeftWall * v * p);
        progNormalMap.SetUniform(mLocationNormalMap, mLeftWall);
        progNormalMap.SetUniform(mitLocationNormalMap, Mat4.Inverse(mLeftWall), true);

        // render geometry
        GL.DrawElements(BeginMode.Triangles, square.Indices.Length, DrawElementsType.UnsignedInt, 0);

        // ************* the right wall
        var mRightWall = Mat4.RotationY(-90) * Mat4.Translation(2.0f, 0.0f, 0.0f);
        progNormalMap.SetUniform(mvpLocationNormalMap, mRightWall * v * p);
        progNormalMap.SetUniform(mLocationNormalMap, mRightWall);
        progNormalMap.SetUniform(mitLocationNormalMap, Mat4.Inverse(mRightWall), true);

        // render geometry
        GL.DrawElements(BeginMode.Triangles, square.Indices.Length, DrawElementsType.UnsignedInt, 0);

        // ************* the top wall
        var mTopWall = Mat4.RotationX(90) * Mat4.Translation(0.0f, 2.0f, 0.0f);
        progNormalMap.SetUniform(mvpLocationNormalMap, mTopWall * v * p);
        progNormalMap.SetUniform(mLocationNormalMap, mTopWall);
        progNormalMap.SetUniform(mitLocationNormalMap, Mat4.Inverse(mTopWall), true);

        progNormalMap.SetTexture(normMapLocationNormalMap, ceilingNormalMap, 0);
        progNormalMap.SetTexture(texLocationNormalMap, ceilingAlbedo, 1);

        // render geometry
        GL.DrawElements(BeginMode.Triangles, square.Indices.Length, DrawElementsType.UnsignedInt, 0);

        // ************* the bottom wall

        var mBottomWall = Mat4.RotationX(-90) * Mat4.Translation(0.0f, -2.0f, 0.0f);
        progNormalMap.SetUniform(mvpLocationNormalMap, mBottomWall * v * p);
        progNormalMap.SetUniform(mLocationNormalMap, mBottomWall);
        progNormalMap.SetUniform(mitLocationNormalMap, Mat4.Inverse(mBottomWall), true);

        progNormalMap.SetTexture(normMapLocationNormalMap, floorNormalMap, 0);
        progNormalMap.SetTexture(texLocationNormalMap, floorAlbedo, 1);

        // render geometry
        GL.DrawElements(BeginMode.Triangles, square.Indices.Length, DrawElementsType.UnsignedInt, 0);

        // ************* the back wall

        var mBackWall = Mat4.Translation(0.0f, 0.0f, -2.0f);
        progNormalMap.SetUniform(mvpLocationNormalMap, mBackWall * v * p);
        progNormalMap.SetUniform(mLocationNormalMap, mBackWall);
        progNormalMap.SetUniform(mitLocationNormalMap, Mat4.Inverse(mBackWall), true);

        // render geometry
        GL.DrawElements(BeginMode.Triangles, square.Indices.Length, DrawElementsType.UnsignedInt, 0);

        // ************* particles
        if (particleSystem == null)
          particleSystem = new ParticleSystem(2000, mBall * (0.0f, 0.0f, 0.0f), 0.1f, accelerations[currentAcceleration], (-1.9f, -1.9f, -1.9f), (1.9f, 1.9f, 1.9f), 200, 100);

        particleSystem.PointSize = (float)dim.Height / 30;
        particleSystem.Center = mBall * (0.0f, 0.0f, 0.0f);

        particleSystem.Render(v, p);
        particleSystem.Update();

        gl.EndOfFrame();
      } while (!gl.ShouldClose());

      return 0;
    }

    private static void KeyCallback(Key key, int scancode, KeyAction action, KeyModifiers mods) {
      if (key == Key.B && action == KeyAction.Press) {
        bounce = !bounce;
        particleSystem.SetBounce(bounce);
      }

      if (key == Key.C && action == KeyAction.Press) {
        currentColor = (uint)((currentColor + 1) % colors.Length);
        particleSystem.SetColor(colors[currentColor]);
      }

      if (key == Key.A && action == KeyAction.Press) {
        currentAcceleration = (uint)((currentAcceleration + 1) % accelerations.Length);
        particleSystem.SetAcceleration(accelerations[currentAcceleration]);
      }
    }

  }
}
