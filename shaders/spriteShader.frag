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
uniform float alpha;
uniform sampler2D texture0;

uniform vec3 viewPos;

uniform Light lights[P_LIGHT_COUNT];
uniform Light dirLight;

out vec4 FragColor;

void main() {
    vec4 texcol = texture(texture0, texCoord);
    if(texcol.a < 0.1)
        discard; 
    FragColor = vec4(diffuse, alpha) * texcol;
}
