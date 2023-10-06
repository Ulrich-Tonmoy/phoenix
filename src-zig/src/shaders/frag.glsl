#version 330 core
out vec4 FragColor;

in vec3 out_norm;

void main() {
  vec3 n = out_norm;
  vec3 l = normalize(vec3(0.5, 0.5, -0.5));
  float light = dot(n, l);

  vec3 diffuse = vec3(1.0, 0.5, 1.0);
  vec3 color = diffuse * light;

  FragColor = vec4(color, 1.0);
}