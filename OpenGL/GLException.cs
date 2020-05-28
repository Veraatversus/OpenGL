using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGL {

  public class GLException : Exception {
    public GLException(string what) : base(what) {

    }

    public GLException() {
    }

    public GLException(string message, Exception innerException) : base(message, innerException) {
    }
  }
}