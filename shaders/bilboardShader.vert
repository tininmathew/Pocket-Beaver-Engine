#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 light;

out vec3 FragPos;
out vec3 normal;
out vec2 texCoord;

void main()
{
    FragPos = vec3(model * vec4(aPosition, 1.0));
    texCoord = aTexCoord;
    
    gl_Position =
        projection *
        view *
        model *
        vec4(aPosition, 1.0);
}