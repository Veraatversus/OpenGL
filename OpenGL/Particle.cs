namespace OpenGL {

  public class Particle {

    #region Public Properties

    public Vec3 Color { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Particle(Vec3 position, Vec3 direction, Vec3 acceleration,
                    Vec3 color, float opacity, uint maxAge, Vec3 minPos,
                    Vec3 maxPos, bool bounce) {
      this.position = position;
      this.direction = direction;
      this.Acceleration = acceleration;
      this.Color = color;
      this.opacity = opacity;
      this.Bounce = bounce;
      this.maxAge = maxAge;
      this.age = 0;
      this.minPos = minPos;
      this.maxPos = maxPos;
    }

    #endregion Public Constructors

    #region Public Methods

    public bool IsDead() => age >= maxAge;

    public void Update() {
      age++;
      if (IsDead()) {
        opacity = 0.0f;
        return;
      }

      var nextPosition = position + direction;

      if (Bounce) {
        if (nextPosition.X < minPos.X || nextPosition.X > maxPos.X)
          direction *= new Vec3(-0.5f, 0.0f, 0.0f);
        if (nextPosition.Y < minPos.Y || nextPosition.Y > maxPos.Y)
          direction *= new Vec3(0.0f, -0.5f, 0.0f);
        if (nextPosition.Z < minPos.Z || nextPosition.Z > maxPos.Z)
          direction *= new Vec3(0.0f, 0.0f, -0.5f);
        nextPosition = position + direction;
      }
      else {
        if (nextPosition.X < minPos.X || nextPosition.X > maxPos.X ||
          nextPosition.Y < minPos.Y || nextPosition.Y > maxPos.Y ||
          nextPosition.Z < minPos.Z || nextPosition.Z > maxPos.Z) {
          direction = new Vec3(0, 0, 0);
          Acceleration = new Vec3(0, 0, 0);
          nextPosition = position;
        }
      }
      position = nextPosition;
      direction += Acceleration;
    }

    public float[] GetData() {
      return new[] { position.X, position.Y, position.Z, Color.X, Color.Y, Color.Z, opacity };
    }

    public void Restart(Vec3 position, Vec3 direction, Vec3 color, float opacity, uint maxAge) {
      this.position = position;
      this.direction = direction;
      this.Color = color;
      this.opacity = opacity;
      this.maxAge = maxAge;

      age = 0;
    }

    #endregion Public Methods

    #region Private Fields

    private Vec3 position;
    private Vec3 direction;
    public Vec3 Acceleration { get; set; }
    private float opacity;
    public bool Bounce { get; set; }
    private uint maxAge;
    private uint age;
    private readonly Vec3 minPos;
    private readonly Vec3 maxPos;

    #endregion Private Fields
  }
}