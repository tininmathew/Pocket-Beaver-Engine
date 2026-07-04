#version 330 core

#define LIGHT_COUNT 3

struct Light {
    vec3 transform;
    vec3 color;
    float intensity;
};

in vec3 color;
in vec3 normal;
in vec3 FragPos;

uniform Light lights[LIGHT_COUNT];
uniform Light dirLight;

out vec4 FragColor;

void main()
{
    vec3 norm = normalize(normal);

    vec3 result = vec3(0,0,0);
    for (int i = 0; i < LIGHT_COUNT; i++)
    {
        vec3 lightDir = normalize(lights[i].transform - FragPos);
        result += (lights[i].color * max(dot(norm, lightDir) * lights[i].intensity, 0.0));
    }
    vec3 DirLightDir = normalize(-dirLight.transform);
    result += (dirLight.color * max(dot(norm, DirLightDir) * dirLight.intensity, 0.0));

    //result *= color;

    FragColor = 
        //vec4(normal * 0.5 + 0.5, 1.0);
        vec4(result, 1.0);
}