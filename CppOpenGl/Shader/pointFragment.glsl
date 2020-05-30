#version 120

uniform sampler2D sprite;

varying vec4 color;

void main() {
    vec4 texValue = texture2D(sprite, gl_PointCoord);
    texValue.a = texValue.r;
    gl_FragColor = vec4(color*texValue);
}
