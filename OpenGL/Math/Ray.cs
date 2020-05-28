namespace MathR {

  public class Ray {

    #region Public Properties

    public Vec3 Origin { get; set; }
    public Vec3 Direction { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Ray(Vec3 origin, Vec3 direction) {
      Origin = origin;
      Direction = direction;
    }

    #endregion Public Constructors

    #region Public Methods

    public Vec3 PointAtParameter(float t) => Origin + (Direction * t);

    public override string ToString() => $"{{{Origin}}} + {{{Direction}}} * t";

    #endregion Public Methods
  }
}