using Pencil.Gaming.Graphics;
using System;
using System.Runtime.InteropServices;

namespace OpenGL {

  public class GLBuffer : IDisposable {

    #region Public Constructors

    public GLBuffer(BufferTarget target) {
      this.target = target;
      bufferID = 0;
      elemSize = 0;
      stride = 0;
      type = 0;
      GL.GenBuffers(1, out bufferID);
    }

    #endregion Public Constructors

    #region Public Methods

    public void Dispose() {
      GL.BindBuffer(target, 0);
      GL.DeleteBuffer(bufferID);
    }

    #endregion Public Methods

    #region Private Destructors

    ~GLBuffer() {
      Dispose();
    }

    #endregion Private Destructors

    #region Private Methods

    public GLBuffer SetData<T>(T[] data, int valuesPerElement = 1) where T : struct {
      elemSize = Marshal.SizeOf<T>();
      stride = valuesPerElement * elemSize;
      type = data[0] switch
      {
        uint _ => VertexAttribPointerType.UnsignedInt,
        int _ => VertexAttribPointerType.Int,
        float _ => VertexAttribPointerType.Float,
        _ => VertexAttribPointerType.Byte
      };

      GL.BindBuffer(target, bufferID);
      GL.BufferData(target, (IntPtr)(elemSize * data.Length), data, BufferUsageHint.StaticDraw);
      return this;
    }

    public void ConnectVertexAttrib(int location, int count, int offset = 0) {
      GL.BindBuffer(target, bufferID);
      GL.EnableVertexAttribArray(location);
      GL.VertexAttribPointer(location, count, type, false, stride, (offset * elemSize));
    }

    public void Bind() {
      GL.BindBuffer(target, bufferID);
    }

    #endregion Private Methods

    #region Private Fields

    private BufferTarget target;
    private int bufferID;
    private int elemSize;
    private int stride;
    private VertexAttribPointerType type;

    #endregion Private Fields
  }
}