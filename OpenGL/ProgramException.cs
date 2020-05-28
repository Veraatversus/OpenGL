using System;

namespace OpenGL {
  public class ProgramException : Exception {
    public ProgramException(string what) : base(what) {

    }

    public ProgramException() {
    }

    public ProgramException(string message, Exception innerException) : base(message, innerException) {
    }
  }
}