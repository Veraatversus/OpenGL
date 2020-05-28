using Pencil.Gaming.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenGL {
  public class GLProgram {
    private uint glVertexShader;
    private uint glFragmentShader;
    private uint glProgram;

    public GLProgram(string[] vertexShaders, string[] fragmentShaders) {
      int nullTerminator = 0;

      var glVertexShader = CAT(() => GL.CreateShader(ShaderType.VertexShader));
      CAT(() => GL.ShaderSource(glVertexShader, vertexShaders.Length, vertexShaders, ref nullTerminator));
      CAT(() => GL.CompileShader(glVertexShader));

      var glFragmentShader = CAT(() => GL.CreateShader(ShaderType.FragmentShader));
      CAT(() => GL.ShaderSource(glFragmentShader, fragmentShaders.Length, fragmentShaders, ref nullTerminator));
      CAT(() => GL.CompileShader(glFragmentShader));

      glProgram = CAT(() => GL.CreateProgram());
      CAT(() => GL.AttachShader(glProgram, glVertexShader));
      CAT(() => GL.AttachShader(glProgram, glFragmentShader));
      CAT(() => GL.LinkProgram(glProgram));
    }

    public static GLProgram CreateFromFiles(string[] vertexShader, string[] fragmentShader) {
      var vsText = vertexShader.Select(vs => LoadFile(vs)).ToArray();
      var fsText = fragmentShader.Select(fs => LoadFile(fs)).ToArray();
      return GLProgram.CreateFromStrings(vsText, fsText);
    }
    public static GLProgram CreateFromFile(string vertexShader, string fragmentShader) {
      return GLProgram.CreateFromFiles(new[] { vertexShader }, new[] { fragmentShader });
    }
    public static GLProgram CreateFromStrings(string[] vertexShader, string[] fragmentShader) {
      return new GLProgram(vertexShader, fragmentShader);
    }
    public static GLProgram CreateFromString(string vertexShader, string fragmentShader) {
      return GLProgram.CreateFromStrings(new[] { vertexShader }, new[] { fragmentShader });
    }
    private static string LoadFile(string filename) {
      //var shaderFile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      //if (!shaderFile.CanRead) {
      //  throw new ProgramException($"Unable to open file {filename}");
      //}
      //var builder = new StringBuilder();
      //var reader = new StreamReader(shaderFile);
      //while (reader.Peek() >= 0) {
      //  builder.Append(reader.ReadLine());
      //}
      //return builder.ToString();
      return File.ReadAllText(filename);
    }
    private static T CAT<T>(Func<T> action) {
      var result = action.Invoke();
      var e = GL.GetError();
      if (e != ErrorCode.NoError) {
        throw new ProgramException($"OpenGL Error: {e}");
      }
      return result;
    }
    private static void CAT(Action action) {
      action?.Invoke();
      var e = GL.GetError();
      if (e != ErrorCode.NoError) {
        throw new ProgramException($"OpenGL Error: {e}");
      }
    }
  }
}
