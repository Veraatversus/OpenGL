using System;

namespace MathR {

  public static class MathR {

    #region Public Methods

    public static Vec3 Lerp(this Vec3 a, Vec3 b, float t) => (a * (1 - t)) + (b * t);

    public static Vec3 Cross(this Vec3 self, Vec3 other) {
      return new Vec3(
                  (self.Y * other.Z) - (self.Z * other.Y),
                  (self.Z * other.X) - (self.X * other.Z),
                  (self.X * other.Y) - (self.Y * other.X));
    }

    public static float Dot(this Vec3 self, Vec3 other) => (self.X * other.X) + (self.Y * other.Y) + (self.Z * other.Z);

    public static Vec3 Normalize(this Vec3 self) => self / self.Lenght();

    public static float Lenght(this Vec3 self) => MathF.Sqrt(self.SquareLenght());

    public static float SquareLenght(this Vec3 self) => (self.X * self.X) + (self.Y * self.Y) + (self.Z * self.Z);

    public static Vec3 Reflect(this Vec3 self, Vec3 other) => self - (other * Dot(self, other) * 2);

    public static Vec3 Refract(this Vec3 uv, Vec3 n, float etaiOverEtat) {
      var cosTheta = Math.Min(-MathR.Dot(uv, n), 1);
      var rOutParallel = (uv + (n * cosTheta)) * etaiOverEtat;
      var rOutPerpendicular = n * -MathF.Sqrt(1 - Math.Min(rOutParallel.SquareLenght(), 1));
      return rOutPerpendicular + rOutParallel;
    }

    public static float Clamp(this float value, float min = 0F, float max = 1F) => MathF.Max(MathF.Min(value, max), min);

    public static Vec3 RandomPointInSphere(float squareLenght = 1) {
      while (true) {
        var p = new Vec3(Rand.Rand11(), Rand.Rand11(), Rand.Rand11());
        if (p.SquareLenght() > squareLenght)
          continue;
        return p;
      }
    }

    public static Vec3 RandomPointInDisc(float squareLenght = 1) {
      while (true) {
        var p = new Vec3(Rand.Rand01(), Rand.Rand01(), 0);
        if (p.SquareLenght() > squareLenght)
          continue;
        return p;
      }
    }

    public static Vec3 RandomUnitVector() {
      var a = Rand.Rand0Pi();
      var z = Rand.Rand11();
      var r = MathF.Sqrt(1.0f - (z * z));

      return new Vec3(r * MathF.Cos(a), r * MathF.Sin(a), z);
    }

    public static float ConvertDegreesToRadians(float degrees) {
      var radians = MathF.PI / 180 * degrees;
      return radians;
    }

    public static float Schlick(float cosine, float refIndex) {
      var r0 = (1 - refIndex) / (1 + refIndex);
      r0 *= r0;
      return r0 - ((1 - r0) * MathF.Pow(1 - cosine, 5));
    }

    #endregion Public Methods
  }
}