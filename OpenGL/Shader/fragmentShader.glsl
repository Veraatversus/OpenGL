#version 110

uniform vec3 lightPos;

varying vec3 normal;
varying vec3 pos;

void main() {
	vec3 lightDir = normalize(lightPos - pos);

	float diffuse = dot(lightDir, normalize(normal));

	gl_FragColor = vec4(diffuse, diffuse, diffuse, 1.0);
}
