using MathR;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGL {
  public class Particle {
    Vec3 position;
    Vec3 direction;
    Vec3 acceleration;
    Vec3 color;
    float opacity;
    bool bounce;

    uint maxAge;
    uint age;

    Vec3 minPos;
    Vec3 maxPos;
    public Particle(Vec3 position, Vec3 direction, Vec3 acceleration,
                    Vec3 color, float opacity, uint maxAge, Vec3 minPos,
                    Vec3 maxPos, bool bounce) {
      this.position = position;
      this.direction = direction;
      this.acceleration = acceleration;
      this.color = color;
      this.opacity = opacity;
      this.bounce = bounce;
      this.maxAge = maxAge;
      this.age = 0;
      this.minPos = minPos;
      this.maxPos = maxPos;
    }
    public bool isDead() => age >= maxAge;
    public void setBounce(bool bounce) { this.bounce = bounce; }
    public void setAcceleration(Vec3 acceleration) { this.acceleration = acceleration; }
   
    public void update() {
      age++;
      if (isDead()) {
        opacity = 0.0f;
        return;
      }

      var nextPosition = position + direction;

      if (bounce) {
        if (nextPosition.X < minPos.X || nextPosition.X > maxPos.X)
          direction = direction * new Vec3(-0.5f, 0.0f, 0.0f);
        if (nextPosition.Y < minPos.Y || nextPosition.Y > maxPos.Y)
          direction = direction * new Vec3(0.0f, -0.5f, 0.0f);
        if (nextPosition.Z < minPos.Z || nextPosition.Z > maxPos.Z)
          direction = direction * new Vec3(0.0f, 0.0f, -0.5f);
        nextPosition = position + direction;
      }
      else {
        if (nextPosition.X < minPos.X || nextPosition.X > maxPos.X ||
          nextPosition.Y < minPos.Y || nextPosition.Y > maxPos.Y ||
          nextPosition.Z < minPos.Z || nextPosition.Z > maxPos.Z) {
          direction = new Vec3(0, 0, 0);
          acceleration = new Vec3(0, 0, 0);
          nextPosition = position;
        }
      }
      position = nextPosition;
      direction = direction + acceleration;
    }

    public float[] getData() {
      return new[] { position.X, position.Y, position.Z, color.X, color.Y, color.Z, opacity };
    }

    public void restart(Vec3 position, Vec3 direction, Vec3 color, float opacity) {
      this.position = position;
      this.direction = direction;
      this.color = color;
      this.opacity = opacity;
      age = 0;
    }
  }
}
