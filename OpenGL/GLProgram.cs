using MathR;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenGL {
  public class GLProgram {
    private uint glVertexShader;
    private uint glFragmentShader;
    private uint glProgram;

    public GLProgram(string[] vertexShaders, string[] fragmentShaders) {
      int nullTerminator = -1;

      var glVertexShader = CAT(() => GL.CreateShader(ShaderType.VertexShader));
      CAT(() => GL.ShaderSource(glVertexShader, vertexShaders.Length, vertexShaders, ref nullTerminator));
      CATS(glVertexShader, () => GL.CompileShader(glVertexShader));

      var glFragmentShader = CAT(() => GL.CreateShader(ShaderType.FragmentShader));
      CAT(() => GL.ShaderSource(glFragmentShader, fragmentShaders.Length, fragmentShaders, ref nullTerminator));
      CATS(glFragmentShader, () => GL.CompileShader(glFragmentShader));

      glProgram = CAT(() => GL.CreateProgram());
      CAT(() => GL.AttachShader(glProgram, glVertexShader));
      CAT(() => GL.AttachShader(glProgram, glFragmentShader));
      CATP(glProgram, () => GL.LinkProgram(glProgram));
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
      //using var shaderFile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      //if (!shaderFile.CanRead) {
      //  throw new ProgramException($"Unable to open file {filename}");
      //}
      //var builder = new StringBuilder();
      //using var reader = new StreamReader(shaderFile);
      //while (reader.Peek() >= 0) {
      //  builder.AppendLine(reader.ReadLine());
      //}
      //return builder.ToString() + char.MinValue;
      return File.ReadAllText(filename) + char.MinValue;
    }

    public void Enable() {
      GL.UseProgram(glProgram);
    }

    public int GetUniformLocation(string variable) {
      return GL.GetUniformLocation(glProgram, variable);
    }

    public int GetAttribLocation(string variable) {
      return GL.GetAttribLocation(glProgram, variable);
    }

    public void SetUniform(int id, float value) {
      GL.Uniform1(id, value);
    }

    public void SetUniform(int id, Vec3 value) {
      GL.Uniform3(id, 1, value.E);
    }

    public void SetUniform(int id, Mat4 value, bool transpose = false) {
      GL.UniformMatrix4(id, 1, transpose, value.E);
    }
    public void SetUniform(int id, Matrix value, bool transpose = false) {
      GL.UniformMatrix4(id, transpose, ref value);
    }

    private static T CATP<T>(uint program, Func<T> action) {
      var result = action.Invoke();
      CheckAndThrowProgram(program);
      return result;
    }
    private static void CATP(uint program, Action action) {
      action?.Invoke();
      CheckAndThrowProgram(program);
    }
    private static T CATS<T>(uint shader, Func<T> action) {
      var result = action.Invoke();
      CheckAndThrowShader(shader);
      return result;
    }
    private static void CATS(uint shader, Action action) {
      action?.Invoke();
      CheckAndThrowShader(shader);
    }
    private static T CAT<T>(Func<T> action) {
      var result = action.Invoke();
      CheckAndThrow();
      return result;
    }
    private static void CAT(Action action) {
      action?.Invoke();
      CheckAndThrow();
    }

    public static void CheckAndThrow() {
      var e = GL.GetError();
      if (e != ErrorCode.NoError) {
        throw new ProgramException($"OpenGL Error: {e}");
      }
    }
    static void CheckAndThrowShader(uint shader) {
      GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);
      if (success == 0) {
        //GL.GetShader(shader, ShaderParameter.InfoLogLength, out var log_length);
        //log_length = Math.Min(4096, log_length);
        GL.GetShaderInfoLog(Convert.ToInt32(shader), out var log);

        throw new ProgramException(log);
      }
    }
    static void CheckAndThrowProgram(uint program) {
      GL.GetProgram(program, ProgramParameter.LinkStatus, out var linked);
      if (linked != 1) {
        //GL.GetProgram(program,ProgramParameter.InfoLogLength, out var log_length);
        //log_length = Math.Min(4096, log_length);
        GL.GetProgramInfoLog(Convert.ToInt32(program), out var log);

        throw new ProgramException(log);
      }
    }
  }
}
