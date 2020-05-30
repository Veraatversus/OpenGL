#version 110

uniform vec3 lightPos;

uniform sampler2D tex;

varying vec3 normal;
varying vec3 pos;
varying vec2 tc;

void main() {
  vec3 lightDir = normalize(lightPos-pos);
  
  vec3 r = reflect(-lightDir, normal);
  
  
  float ambient = 0.1;
  float diffuse = max(0.0,dot(lightDir, normalize(normal)));
  float specular = 0.0;//pow(dot(eyeDir, r),20);
    
  vec4 texValue = texture2D(tex, tc*20.0);
  
  gl_FragColor = (ambient+diffuse)*texValue + specular*vec4(1.0,1.0,1.0,0.0);
}
