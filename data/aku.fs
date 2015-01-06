#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec3 pos;
in vec3 normal;
in vec3 eye_dir;
in vec3 light_dir;

// Ouput data
out vec3 color;

// Values that stay constant for the whole mesh.
uniform sampler2D sampler;
uniform mat4 MV;
uniform vec3 light;

void main(){

	// Light emission properties
	// You probably want to put them as uniforms
	vec3 LightColor = vec3(1,0.9,0.8);
	float LightPower = 150.0f;

	// Material properties
	vec3 MaterialDiffuseColor = texture2D( sampler, UV ).rgb;
	vec3 MaterialAmbientColor = vec3(0.2,0.2,0.2) * MaterialDiffuseColor;
	vec3 MaterialSpecularColor = vec3(0.3,0.3,0.3);

	// Distance to the light
	float distance = length( light - pos );

	// Normal of the computed fragment, in camera space
	vec3 n = normalize( normal );
	// Direction of the light (from the fragment to the light)
	vec3 l = normalize( light_dir );
	// Cosine of the angle between the normal and the light direction,
	// clamped above 0
	//  - light is at the vertical of the triangle -> 1
	//  - light is perpendicular to the triangle -> 0
	//  - light is behind the triangle -> 0
	float cosTheta = clamp( dot( n,l ), 0,1 );

	// Eye vector (towards the camera)
	vec3 E = normalize(eye_dir);
	// Direction in which the triangle reflects the light
	vec3 R = reflect(-l,n);
	// Cosine of the angle between the Eye vector and the Reflect vector,
	// clamped to 0
	//  - Looking into the reflection -> 1
	//  - Looking elsewhere -> < 1
	float cosAlpha = clamp( dot( E,R ), 0,1 );

	color =
		// Ambient : simulates indirect lighting
		MaterialAmbientColor +
		// Diffuse : "color" of the object
		MaterialDiffuseColor * LightColor * LightPower * cosTheta / (distance*distance) +
		// Specular : reflective highlight, like a mirror
		MaterialSpecularColor * LightColor * LightPower * pow(cosAlpha,5) / (distance*distance);

}