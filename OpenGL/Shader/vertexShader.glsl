#version 110

uniform mat4 MVP;
uniform mat4 Mit;
uniform mat4 M;

attribute vec3 vPos;
attribute vec3 vNormal;
attribute vec2 vTc;

varying vec3 normal;
varying vec3 pos;
varying vec2 tc;

void main() {
  gl_Position = MVP * vec4(vPos,1.0);
  pos = (M * vec4(vPos,1.0)).xyz;
  normal = (Mit * vec4(vNormal,0.0)).xyz;
  tc = vTc;
}
