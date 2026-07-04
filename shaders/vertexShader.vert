#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 light;

out vec3 FragPos;
out vec3 normal;

void main()
{   
    normal = mat3(transpose(inverse(model))) * aNormal;
    FragPos = vec3(model * vec4(aPosition, 1.0));
    vec3 raitedPos;
    gl_Position =
        projection *
        view *
        model *
        vec4(aPosition, 1.0);
}