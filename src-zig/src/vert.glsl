#version 330 core
layout(location = 0) in vec3 pos;

uniform vec3 _Offset;
uniform mat4 _P;

void main() {
  vec3 p = pos + _Offset;
  gl_Position = _P * vec4(p.x, p.y, p.z, 1.0);
}