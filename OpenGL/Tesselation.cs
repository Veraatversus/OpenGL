using System;
using System.Collections.Generic;

namespace OpenGL {

  public class Tesselation {

    #region Public Properties

    public float[] Vertices { get; set; }

    public float[] Normals { get; set; }
    public float[] Tangents { get; set; }

    public float[] TexCoords { get; set; }

    public uint[] Indices { get; set; }

    #endregion Public Properties

    #region Public Methods

    public static Tesselation GenSphere(Vec3 center, float radius, uint sectorCount, uint stackCount) {

      var vertices = new List<float>();
      var normals = new List<float>();
      var tangents = new List<float>();
      var texCoords = new List<float>();
      var indices = new List<uint>();

      var lengthInv = 1.0f / radius;
      var sectorStep = 2.0f * (float)Math.PI / sectorCount;
      var stackStep = (float)Math.PI / stackCount;

      for (uint i = 0; i <= stackCount; ++i) {
        var stackAngle = ((float)Math.PI / 2.0f) - (i * stackStep);// starting from pi/2 to -pi/2
        var xy = radius * MathF.Cos(stackAngle);               // r * cos(u)
        var z = radius * MathF.Sin(stackAngle);                // r * sin(u)

        // add (sectorCount+1) vertices per stack the first and last vertices have same position and
        // normal, but different tex coords
        for (uint j = 0; j <= sectorCount; ++j) {
          var sectorAngle = j * sectorStep;                   // starting from 0 to 2pi

          // vertex position (x, y, z)
          var x = xy * MathF.Cos(sectorAngle);                // r * cos(u) * cos(v)
          var y = xy * MathF.Sin(sectorAngle);                // r * cos(u) * sin(v)
          vertices.Add(center.X + x);
          vertices.Add(center.Y + y);
          vertices.Add(center.Z + z);

          // normalized vertex normal (nx, ny, nz)
          normals.Add(x * lengthInv);
          normals.Add(y * lengthInv);
          normals.Add(z * lengthInv);

          var nextSectorAngle = (j + 1) * sectorStep;
          var nx = xy * MathF.Cos(nextSectorAngle);
          var ny = xy * MathF.Sin(nextSectorAngle);

          // compute the tangent an make sure it is perpendicular to the normal
          var n = new Vec3(x * lengthInv, y * lengthInv, z * lengthInv);
          var t = new Vec3(nx, ny, z).Normalize() - new Vec3(x, y, z).Normalize();
          var b = t.Cross(n);
          var tCorr = n.Cross(b);

          // normalized vertex tangent (tx, ty, tz)
          tangents.Add(tCorr.X);
          tangents.Add(tCorr.Y);
          tangents.Add(tCorr.Z);

          // vertex tex coord (s, t) range between [0, 1]
          texCoords.Add((float)j / sectorCount);
          texCoords.Add((float)i / stackCount);
        }
      }

      // generate CCW index list of sphere triangles
      for (uint i = 0; i < stackCount; ++i) {
        var k1 = i * (sectorCount + 1);     // beginning of current stack
        var k2 = k1 + sectorCount + 1;      // beginning of next stack

        for (uint j = 0; j < sectorCount; ++j, ++k1, ++k2) {
          // 2 triangles per sector excluding first and last stacks k1 => k2 => k1+1
          if (i != 0) {
            indices.Add(k1);
            indices.Add(k2);
            indices.Add(k1 + 1);
          }

          // k1+1 => k2 => k2+1
          if (i != (stackCount - 1)) {
            indices.Add(k1 + 1);
            indices.Add(k2);
            indices.Add(k2 + 1);
          }
        }
      }

      return new Tesselation {
        Indices = indices.ToArray(),
        Normals = normals.ToArray(),
        TexCoords = texCoords.ToArray(),
        Vertices = vertices.ToArray(),
        Tangents = tangents.ToArray()
      };
    }

    public static Tesselation GenRectangle(Vec3 center, float width, float height) {
      var hWidth = width / 2.0f;
      var hHeight = height / 2.0f;

      var tess = new Tesselation {
        Vertices = new[] {
          -hWidth, -hHeight, 0.0f,
           hWidth, -hHeight, 0.0f,
           hWidth,  hHeight, 0.0f,
          -hWidth,  hHeight, 0.0f
        },

        Normals = new[] {
          0.0f, 0.0f, 1.0f,
          0.0f, 0.0f, 1.0f,
          0.0f, 0.0f, 1.0f,
          0.0f, 0.0f, 1.0f
        },

        Tangents = new[] {
          1.0f, 0.0f, 0.0f,
          1.0f, 0.0f, 0.0f,
          1.0f, 0.0f, 0.0f,
          1.0f, 0.0f, 0.0f
        },

        TexCoords = new[] {
          0.0f, 0.0f,
          1.0f, 0.0f,
          1.0f, 1.0f,
          0.0f, 1.0f
        },

        Indices = new uint[] { 0, 1, 2, 0, 2, 3 }

      };

      return tess;
    }

    #endregion Public Methods
  }
}