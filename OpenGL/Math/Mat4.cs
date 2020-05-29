using Microsoft.VisualBasic;
using System;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace MathR {

  public class Mat4 {

    #region Public Properties

    public float[] E { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Mat4() : this(1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1) {
    }
    public static implicit operator IntPtr(Mat4 mat) => Marshal.UnsafeAddrOfPinnedArrayElement(mat.E, 0);

    public Mat4(float m11, float m12, float m13, float m14,
        float m21, float m22, float m23, float m24,
        float m31, float m32, float m33, float m34,
        float m41, float m42, float m43, float m44) :

      this(new[] { m11, m12, m13, m14,
        m21, m22, m23, m24,
        m31, m32, m33, m34,
        m41, m42, m43, m44
      }) {
    }

    public Mat4(float[] array) {
      E = array;
    }

    public Mat4(Mat4 other) : this(other.E) {
    }

    #endregion Public Constructors

    #region Public Methods

    // binary operators with scalars
    public static Mat4 operator *(Mat4 a, float scalar) => new Mat4(
        a.E[0] * scalar, a.E[1] * scalar, a.E[2] * scalar, a.E[3] * scalar,
        a.E[4] * scalar, a.E[5] * scalar, a.E[6] * scalar, a.E[7] * scalar,
        a.E[8] * scalar, a.E[9] * scalar, a.E[10] * scalar, a.E[11] * scalar,
        a.E[12] * scalar, a.E[13] * scalar, a.E[14] * scalar, a.E[15] * scalar
      );

    public static Mat4 operator +(Mat4 a, float scalar) => new Mat4(
        a.E[0] + scalar, a.E[1] + scalar, a.E[2] + scalar, a.E[3] + scalar,
        a.E[4] + scalar, a.E[5] + scalar, a.E[6] + scalar, a.E[7] + scalar,
        a.E[8] + scalar, a.E[9] + scalar, a.E[10] + scalar, a.E[11] + scalar,
        a.E[12] + scalar, a.E[13] + scalar, a.E[14] + scalar, a.E[15] + scalar
      );

    public static Mat4 operator -(Mat4 a, float scalar) => new Mat4(
        a.E[0] - scalar, a.E[1] - scalar, a.E[2] - scalar, a.E[3] - scalar,
        a.E[4] - scalar, a.E[5] - scalar, a.E[6] - scalar, a.E[7] - scalar,
        a.E[8] - scalar, a.E[9] - scalar, a.E[10] - scalar, a.E[11] - scalar,
        a.E[12] - scalar, a.E[13] - scalar, a.E[14] - scalar, a.E[15] - scalar
      );

    public static Mat4 operator /(Mat4 a, float scalar) => new Mat4(
        a.E[0] / scalar, a.E[1] / scalar, a.E[2] / scalar, a.E[3] / scalar,
        a.E[4] / scalar, a.E[5] / scalar, a.E[6] / scalar, a.E[7] / scalar,
        a.E[8] / scalar, a.E[9] / scalar, a.E[10] / scalar, a.E[11] / scalar,
        a.E[12] / scalar, a.E[13] / scalar, a.E[14] / scalar, a.E[15] / scalar
      );

    public static Mat4 operator *(Mat4 self, Mat4 other) {
      var result = new Mat4();
      for (var x = 0; x < 16; x += 4) {
        for (var y = 0; y < 4; y++) {
          result.E[x + y] = (self.E[0 + x] * other.E[0 + y]) +

                      (self.E[1 + x] * other.E[4 + y]) +

                      (self.E[2 + x] * other.E[8 + y]) +

                      (self.E[3 + x] * other.E[12 + y]);
        }
      }

      return result;
    }

    public static Vec3 operator *(Mat4 self, Vec3 other) {
      var w = (other.X * self.E[3]) + (other.Y * self.E[7]) + (other.Z * self.E[11]) + (1 * self.E[15]);

      return new Vec3(((other.X * self.E[0]) + (other.Y * self.E[4]) + (other.Z * self.E[8]) + (1 * self.E[12])) / w,
            ((other.X * self.E[1]) + (other.Y * self.E[5]) + (other.Z * self.E[9]) + (1 * self.E[13])) / w,
            ((other.X * self.E[2]) + (other.Y * self.E[6]) + (other.Z * self.E[10]) + (1 * self.E[14])) / w
      );
    }

    public static Mat4 Translation(Vec3 trans) => Translation(trans.X, trans.Y, trans.Z);

    public static Mat4 Translation(float x, float y, float z) => new Mat4(new[] {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        x, y, z, 1 });

    public static Mat4 Scaling(Vec3 scale) => Scaling(scale.X, scale.Y, scale.Z);

    public static Mat4 Scaling(float x, float y, float z) => new Mat4(new[] {
        x, 0, 0, 0,
        0, y, 0, 0,
        0, 0, z, 0,
        0, 0, 0, 1 });

    public static Mat4 RotationX(float degree) {
      var angle = Deg2Rad(degree);
      var cosAngle = MathF.Cos(angle);
      var sinAngle = MathF.Sin(angle);

      return new Mat4(new[]{
      1, 0, 0, 0,
      0, cosAngle, sinAngle, 0,
      0, -sinAngle, cosAngle, 0,
      0, 0, 0, 1});
    }

    public static Mat4 RotationY(float degree) {
      var angle = Deg2Rad(degree);
      var cosAngle = MathF.Cos(angle);
      var sinAngle = MathF.Sin(angle);

      return new Mat4(new[]{
      cosAngle, 0, -sinAngle, 0,
      0, 1, 0, 0,
      sinAngle, 0, cosAngle, 0,
      0, 0, 0, 1});
    }

    public static Mat4 RotationZ(float degree) {
      var angle = Deg2Rad(degree);
      var cosAngle = (float)Math.Cos(angle);
      var sinAngle = (float)Math.Sin(angle);

      return new Mat4(new float[]{
        cosAngle, sinAngle, 0, 0,
        -sinAngle, cosAngle, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1});
    }

    public static Mat4 RotationAxis(Vec3 axis, float degree) {
      var angle = Deg2Rad(degree);
      var cosAngle = (float)Math.Cos(angle);
      var sinAngle = (float)Math.Sin(angle);
      var oneMinusCosAngle = 1 - cosAngle;

      var sqrAxis = axis * axis;

      return new Mat4(new float[]{
      cosAngle + (oneMinusCosAngle* sqrAxis.X), (oneMinusCosAngle* axis.X * axis.Y) - (sinAngle* axis.Z),(oneMinusCosAngle* axis.X * axis.Z) + (sinAngle* axis.Y),0,
      (oneMinusCosAngle* axis.X * axis.Y) + (sinAngle* axis.Z), cosAngle + (oneMinusCosAngle* sqrAxis.Y),(oneMinusCosAngle* axis.Y * axis.Z) - (sinAngle* axis.X),0,
      (oneMinusCosAngle* axis.X * axis.Z) - (sinAngle* axis.Y), (oneMinusCosAngle* axis.Y * axis.Z) + (sinAngle* axis.X), cosAngle + (oneMinusCosAngle* sqrAxis.Z),0,
      0, 0, 0, 1});
    }

    public static Mat4 Transpose(Mat4 m) => new Mat4(new float[]{
        m.E[0],m.E[4],m.E[8],m.E[12],
        m.E[1],m.E[5],m.E[9],m.E[13],
        m.E[2],m.E[6],m.E[10],m.E[14],
        m.E[3],m.E[7],m.E[11],m.E[15]
      });

    public static Mat4 Inverse(Mat4 m) {
      var result = new Mat4();

      var Q = (m.E[4] * ((m.E[11] * ((m.E[1] * m.E[14]) - (m.E[2] * m.E[13]))) +
              (m.E[3] * ((-m.E[9] * m.E[14]) + (m.E[13] * m.E[10]))) +
              (m.E[15] * ((m.E[2] * m.E[9]) - (m.E[1] * m.E[10])))))
            +
            (m.E[7] * ((m.E[0] * ((m.E[9] * m.E[14]) - (m.E[13] * m.E[10]))) +
              (m.E[2] * ((-m.E[12] * m.E[9]) + (m.E[8] * m.E[13]))) +
              (m.E[1] * ((-m.E[8] * m.E[14]) + (m.E[12] * m.E[10])))))
            +
            (m.E[15] * ((m.E[5] * ((-m.E[8] * m.E[2]) + (m.E[0] * m.E[10]))) +
              (m.E[6] * ((-m.E[0] * m.E[9]) + (m.E[1] * m.E[8])))))
            +
            (m.E[11] * ((m.E[0] * ((-m.E[5] * m.E[14]) + (m.E[6] * m.E[13]))) +
              (m.E[12] * ((m.E[2] * m.E[5]) - (m.E[6] * m.E[1])))))
            +
            (m.E[3] * ((m.E[6] * ((m.E[9] * m.E[12]) - (m.E[13] * m.E[8]))) +
              (m.E[5] * ((m.E[8] * m.E[14]) - (m.E[12] * m.E[10])))));

      result.E[0] = ((m.E[7] * m.E[9] * m.E[14])
          + (m.E[15] * m.E[5] * m.E[10])
          - (m.E[15] * m.E[6] * m.E[9])
          - (m.E[11] * m.E[5] * m.E[14])
          - (m.E[7] * m.E[13] * m.E[10])
          + (m.E[11] * m.E[6] * m.E[13])) / Q;
      result.E[4] = -((m.E[4] * m.E[15] * m.E[10])
        - (m.E[4] * m.E[11] * m.E[14])
        - (m.E[15] * m.E[6] * m.E[8])
        + (m.E[11] * m.E[6] * m.E[12])
        + (m.E[7] * m.E[8] * m.E[14])
        - (m.E[7] * m.E[12] * m.E[10])) / Q;
      result.E[8] = ((-m.E[4] * m.E[11] * m.E[13])
          + (m.E[4] * m.E[15] * m.E[9])
          - (m.E[15] * m.E[8] * m.E[5])
          - (m.E[7] * m.E[12] * m.E[9])
          + (m.E[11] * m.E[12] * m.E[5])
          + (m.E[7] * m.E[8] * m.E[13])) / Q;
      result.E[12] = -((m.E[4] * m.E[9] * m.E[14])
        - (m.E[4] * m.E[13] * m.E[10])
        + (m.E[12] * m.E[5] * m.E[10])
        - (m.E[9] * m.E[6] * m.E[12])
        - (m.E[8] * m.E[5] * m.E[14])
        + (m.E[13] * m.E[6] * m.E[8])) / Q;
      /// 2
      result.E[1] = ((-m.E[1] * m.E[15] * m.E[10])
          + (m.E[1] * m.E[11] * m.E[14])
          - (m.E[11] * m.E[2] * m.E[13])
          - (m.E[3] * m.E[9] * m.E[14])
          + (m.E[15] * m.E[2] * m.E[9])
          + (m.E[3] * m.E[13] * m.E[10])) / Q;

      result.E[5] = ((-m.E[15] * m.E[2] * m.E[8])
          + (m.E[15] * m.E[0] * m.E[10])
          - (m.E[11] * m.E[0] * m.E[14])
          - (m.E[3] * m.E[12] * m.E[10])
          + (m.E[11] * m.E[2] * m.E[12])
          + (m.E[3] * m.E[8] * m.E[14])) / Q;

      result.E[9] = -((-m.E[1] * m.E[15] * m.E[8])
          + (m.E[1] * m.E[11] * m.E[12])
          + (m.E[15] * m.E[0] * m.E[9])
          - (m.E[3] * m.E[9] * m.E[12])
          + (m.E[3] * m.E[13] * m.E[8])
          - (m.E[11] * m.E[0] * m.E[13])) / Q;

      result.E[13] = ((-m.E[1] * m.E[8] * m.E[14])
          + (m.E[1] * m.E[12] * m.E[10])
          + (m.E[0] * m.E[9] * m.E[14])
          - (m.E[0] * m.E[13] * m.E[10])
          - (m.E[12] * m.E[2] * m.E[9])
          + (m.E[8] * m.E[2] * m.E[13])) / Q;
      /// 3
      result.E[2] = -((m.E[15] * m.E[2] * m.E[5])
          - (m.E[7] * m.E[2] * m.E[13])
          - (m.E[3] * m.E[5] * m.E[14])
          + (m.E[1] * m.E[7] * m.E[14])
          - (m.E[1] * m.E[15] * m.E[6])
          + (m.E[3] * m.E[13] * m.E[6])) / Q;

      result.E[6] = ((-m.E[4] * m.E[3] * m.E[14])
          + (m.E[4] * m.E[15] * m.E[2])
          + (m.E[7] * m.E[0] * m.E[14])
          - (m.E[15] * m.E[6] * m.E[0])
          - (m.E[7] * m.E[12] * m.E[2])
          + (m.E[3] * m.E[6] * m.E[12])) / Q;

      result.E[10] = -((-m.E[15] * m.E[0] * m.E[5])
          + (m.E[15] * m.E[1] * m.E[4])
          + (m.E[3] * m.E[12] * m.E[5])
          + (m.E[7] * m.E[0] * m.E[13])
          - (m.E[7] * m.E[1] * m.E[12])
          - (m.E[3] * m.E[4] * m.E[13])) / Q;

      result.E[14] = -((m.E[14] * m.E[0] * m.E[5])
          - (m.E[14] * m.E[1] * m.E[4])
          - (m.E[2] * m.E[12] * m.E[5])
          - (m.E[6] * m.E[0] * m.E[13])
          + (m.E[6] * m.E[1] * m.E[12])
          + (m.E[2] * m.E[4] * m.E[13])) / Q;
      /// 4
      result.E[3] = ((-m.E[1] * m.E[11] * m.E[6])
          + (m.E[1] * m.E[7] * m.E[10])
          - (m.E[7] * m.E[2] * m.E[9])
          - (m.E[3] * m.E[5] * m.E[10])
          + (m.E[11] * m.E[2] * m.E[5])
          + (m.E[3] * m.E[9] * m.E[6])) / Q;

      result.E[7] = -((-m.E[4] * m.E[3] * m.E[10])
          + (m.E[4] * m.E[11] * m.E[2])
          + (m.E[7] * m.E[0] * m.E[10])
          - (m.E[11] * m.E[6] * m.E[0])
          + (m.E[3] * m.E[6] * m.E[8])
          - (m.E[7] * m.E[8] * m.E[2])) / Q;

      result.E[11] = ((-m.E[11] * m.E[0] * m.E[5])
          + (m.E[11] * m.E[1] * m.E[4])
          + (m.E[3] * m.E[8] * m.E[5])
          + (m.E[7] * m.E[0] * m.E[9])
          - (m.E[7] * m.E[1] * m.E[8])
          - (m.E[3] * m.E[4] * m.E[9])) / Q;

      result.E[15] = ((m.E[10] * m.E[0] * m.E[5])
          - (m.E[10] * m.E[1] * m.E[4])
          - (m.E[2] * m.E[8] * m.E[5])
          - (m.E[6] * m.E[0] * m.E[9])
          + (m.E[6] * m.E[1] * m.E[8])
          + (m.E[2] * m.E[4] * m.E[9])) / Q;
      return result;
    }

    public static float Deg2Rad(float d) => (float)Math.PI * d / 180.0f;

    public static Mat4 Perspective(float fovy, float aspect, float znear, float zfar) {
      var cotan = 1.0f / MathF.Tan(Deg2Rad(fovy) / 2.0f);

      return new Mat4(cotan / aspect, 0, 0, 0,
                      0, cotan, 0, 0,
                      0, 0, -(zfar + znear) / (zfar - znear), -1,
                      0, 0, -2.0f * (zfar * znear) / (zfar - znear), 0);
    }

    public static Mat4 Ortho(float left, float right, float bottom, float top, float znear, float zfar) {
      return new Mat4(2 / (right - left), 0, 0, 0,
                      0, 2.0f / (top - bottom), 0, 0,
                      0, 0, -2.0f / (zfar - znear), 0,
                      -(right + left) / (right - left), -(top + bottom) / (top - bottom), -(zfar + znear) / (zfar - znear), 1);
    }

    public static Mat4 LookAt(Vec3 vEye, Vec3 vAt, Vec3 vUp) {
      var f = vAt - vEye;
      var u = vUp;
      var s = f.Cross(u);
      u = s.Cross(f);

      f = f.Normalize();
      u = u.Normalize();
      s = s.Normalize();

      return new Mat4(s.X, u.X, -f.X, 0,
                      s.Y, u.Y, -f.Y, 0,
                      s.Z, u.Z, -f.Z, 0,
                      -s.Dot(vEye), -u.Dot(vEye), f.Dot(vEye), 1);
    }

    public override string ToString() {
      var s = new StringBuilder();
      s.Append("[").Append(E[0]).Append(", ").Append(E[1]).Append(", ").Append(E[2]).Append(", ").Append(E[3]).Append(Environment.NewLine)
        .Append(" ").Append(E[4]).Append(", ").Append(E[5]).Append(", ").Append(E[6]).Append(", ").Append(E[7]).Append(Environment.NewLine)
        .Append(" ").Append(E[8]).Append(", ").Append(E[9]).Append(", ").Append(E[10]).Append(", ").Append(E[11]).Append(Environment.NewLine)
        .Append(" ").Append(E[12]).Append(", ").Append(E[13]).Append(", ").Append(E[14]).Append(", ").Append(E[15]).Append("]");
      return s.ToString();
    }

    #endregion Public Methods
  }
}