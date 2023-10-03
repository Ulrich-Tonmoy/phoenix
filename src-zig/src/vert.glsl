#version 330 core
layout(location = 0) in vec3 pos;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec3 norm;
layout(location = 3) in vec4 col;

uniform vec3 _Offset;
uniform mat4 _P;
uniform mat4 _V;

void main() {
  vec3 p = pos + _Offset;
  gl_Position = _P * _V * vec4(p.x, p.y, p.z, 1.0);
}