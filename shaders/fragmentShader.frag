#version 330 core

in vec3 color;
in vec3 normal;
in vec3 FragPos;

uniform vec3 lightPos;

out vec4 FragColor;

void main()
{
    vec3 norm = normalize(normal);

    vec3 lightDir = normalize(lightPos - FragPos);

    float diffuse = max(dot(norm, lightDir), 0.0);

    vec3 result = vec3(1.0,1.0,1.0) * diffuse;

    FragColor = 
        //vec4(normal * 0.5 + 0.5, 1.0);
        vec4(result, 1.0);
}