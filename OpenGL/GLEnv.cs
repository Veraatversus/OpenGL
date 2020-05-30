using Pencil.Gaming;
using System;
using static Pencil.Gaming.Glfw;

namespace OpenGL {

  public sealed class GLEnv : IDisposable {

    #region Public Events

    public event Action<Key, int, KeyAction, KeyModifiers> OnKeyPressed;

    #endregion Public Events

    #region Public Constructors

    public GLEnv(int width, int height, string titel, int samples = 4) {
      SetErrorCallback(ErrorCallback);
      if (!Init()) {
        throw new GLException("Could not init Glfw.");
      }

      WindowHint(Pencil.Gaming.WindowHint.Samples, samples);

      window = CreateWindow(width, height, titel, GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
      if (window.Equals(GlfwWindowPtr.Null)) {
        Terminate();
        throw new GLException("Failed to Open GLFW window.");
      }

      MakeContextCurrent(window);
      Glfw.SetKeyCallback(window, KeyCallback);
    }

    #endregion Public Constructors

    #region Public Methods

    public void EndOfFrame() {
      SwapBuffers(window);
      PollEvents();
    }

    public Dimensions GetFramebufferSize() {
      Glfw.GetFramebufferSize(window, out var width, out var height);
      return new Dimensions { Width = width, Height = height };
    }

    public bool ShouldClose() {
      return WindowShouldClose(window);
    }

    public void Dispose() {
      Terminate();
      GC.SuppressFinalize(this);
    }

    #endregion Public Methods

    #region Private Destructors

    ~GLEnv() {
      Dispose();
    }

    #endregion Private Destructors

    #region Private Methods

    private void KeyCallback(GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods) {
      if (key == Key.Escape && action == KeyAction.Press) {
        SetWindowShouldClose(window, true);
      }
      OnKeyPressed?.Invoke(key, scanCode, action, mods);
    }

    private void ErrorCallback(GlfwError code, string desc) {
      Console.WriteLine($"Fatal Error: {desc} ({code})");
    }

    #endregion Private Methods

    #region Private Fields

    private GlfwWindowPtr window;

    #endregion Private Fields
  }
}