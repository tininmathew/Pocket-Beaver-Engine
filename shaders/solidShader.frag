#version 330 core

#define P_LIGHT_COUNT 1

struct Light {
    vec3 transform;
    vec3 color;
    float intensity;
};

in vec3 normal;
in vec3 FragPos;
in vec2 texCoord;

uniform vec3 diffuse;
uniform vec3 ambient;
uniform vec3 specular;
uniform float alpha;
uniform sampler2D texture0;

uniform vec3 viewPos;

uniform Light lights[P_LIGHT_COUNT];
uniform Light dirLight;

out vec4 FragColor;

void main() {
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 totalAmbient = dirLight.color * ambient * 0.1;
    vec3 totalDiffuse = vec3(0.0);
    vec3 totalSpecular = vec3(0.0);

    for (int i = 0; i < P_LIGHT_COUNT; i++) {
        vec3 lightDir = normalize(lights[i].transform - FragPos);
        float diffFactor = max(dot(norm, lightDir), 0.0);
        totalDiffuse += (lights[i].color * diffFactor * lights[i].intensity) / distance(FragPos, lights[i].transform);

        vec3 halfwayDir = normalize(lightDir + viewDir);
        float specFactor = pow(max(dot(norm, halfwayDir), 0.0), 32.0);
        totalSpecular += (lights[i].color * specFactor * lights[i].intensity) / distance(FragPos, lights[i].transform);
    }

    vec3 dirLightDir = normalize(-dirLight.transform);
    float dirDiffFactor = max(dot(norm, dirLightDir), 0.0);
    totalDiffuse += dirLight.color * dirDiffFactor * dirLight.intensity;

    vec3 dirHalfwayDir = normalize(dirLightDir + viewDir);
    float dirSpecFactor = pow(max(dot(norm, dirHalfwayDir), 0.0), 32.0);
    totalSpecular += dirLight.color * dirSpecFactor * dirLight.intensity;

    vec3 finalDiffuse = totalDiffuse * diffuse;
    vec3 finalSpecular = totalSpecular * specular;
    vec3 result = totalAmbient + finalDiffuse + finalSpecular;
    vec4 texcol = texture(texture0, texCoord);
    if(texcol.a < 0.1)
        discard; 
    FragColor = vec4(result , alpha) * texcol;
}
