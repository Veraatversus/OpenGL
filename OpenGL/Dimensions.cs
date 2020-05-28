namespace OpenGL {
  public struct Dimensions {
    public int Width { get; set; }
    public int Height { get; set; }

    public float Aspect => (float)Width / Height;
  }
}