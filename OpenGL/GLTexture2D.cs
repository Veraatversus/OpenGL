using Pencil.Gaming.Graphics;
using System;

namespace OpenGL {

  public class GLTexture2D : IDisposable {

    #region Public Properties

    public int Width { get; private set; }

    public int Height { get; private set; }

    public uint ComponentCount { get; private set; }

    public uint Id { get; private set; }

    public PixelInternalFormat Internalformat { get; private set; } 

    public PixelFormat Format { get; private set; }

    public PixelType Type { get; private set; }

    #endregion Public Properties

    #region Public Constructors

    public GLTexture2D(int width, int height, uint componentCount = 4,
                       int magFilter = (int)TextureMagFilter.Nearest, int minFilter = (int)TextureMinFilter.Nearest,
                       int wrapX = (int)TextureWrapMode.Repeat, int wrapY = (int)TextureWrapMode.Repeat) {
      Width = width;
      Height = height;
      ComponentCount = componentCount;

      GL.GenTextures(1, out uint id);
      Id = id;
      GL.BindTexture(TextureTarget.Texture2D, Id);

      GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapX);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapY);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, magFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, minFilter);
    }

    #endregion Public Constructors

    #region Public Methods

    public void Dispose() {
      GL.DeleteTexture(Id);
    }

    public void SetData(byte[] data) {
      if (data.Length != ComponentCount * Width * Height) {
        throw new GLException("Data size and texure dimensions do not match.");
      }

      Type = PixelType.UnsignedByte;
      switch (ComponentCount) {
        case 1:
          Internalformat = PixelInternalFormat.R8;
          Format = PixelFormat.Red;
          break;

        case 2:
          Internalformat = PixelInternalFormat.Rg8;
          Format = PixelFormat.Rg;
          break;

        case 3:
          Internalformat = PixelInternalFormat.Rgb8;
          Format = PixelFormat.Rgb;
          break;

        case 4:
          Internalformat = PixelInternalFormat.Rgba8;
          Format = PixelFormat.Rgba;
          break;
      }

      GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

      GL.BindTexture(TextureTarget.Texture2D, Id);
      GL.TexImage2D(TextureTarget.Texture2D, 0, Internalformat, Width, Height, 0, Format, Type, data);
    }

    #endregion Public Methods

    #region Private Destructors

    ~GLTexture2D() {
      Dispose();
    }

    #endregion Private Destructors
  }
}