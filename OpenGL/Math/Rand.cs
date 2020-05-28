using System;
using System.Threading;

namespace MathR {

  public static class Rand {

    #region Public Methods

    public static float Uniform(this float min, float max) {
      return (float)(random.Value.NextDouble() * (max - min)) + min;
    }

    public static float Rand01() => Uniform(0.0f, 1.0f);

    public static float Rand005() => Uniform(0.0f, 0.5f);

    public static float Rand051() => Uniform(0.5f, 1.0f);

    public static float Rand11() => Uniform(-1.0f, 1.0f);

    public static float Rand0Pi() => Uniform(0.0f, (float)Math.PI);

    #endregion Public Methods

    #region Public Fields

    public static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

    #endregion Public Fields

    #region Private Fields

    private static int seed = Environment.TickCount;

    #endregion Private Fields
  }
}