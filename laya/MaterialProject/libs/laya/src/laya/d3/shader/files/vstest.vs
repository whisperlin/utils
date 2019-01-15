attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;

uniform mat4 unity_MatrixVP;
uniform mat4 unity_ObjectToWorld;
uniform vec3 u_CameraPos;
uniform float u_WaveScale;
uniform vec4 u_WaveSpeed;
uniform float u_Time;

varying vec3 v_Normal;
varying vec3 v_Tangent;
varying vec3 v_Binormal;
varying vec3 v_ViewDir;
varying vec2 v_Texcoord0;
varying vec2 v_Texcoord1;

void main()
{
	vec4 positionWorld = unity_ObjectToWorld * _glesVertex;
	vec4 position = unity_MatrixVP * _glesVertex;
	
	vec4 temp = vec4(positionWorld.x, positionWorld.z, positionWorld.x, positionWorld.z) * u_WaveScale + u_WaveSpeed * u_WaveScale * u_Time;
	
	v_Texcoord0 = temp.xy * vec2(0.4, 0.45);
	v_Texcoord1 = temp.wz;
	
	mat3 worldMat = mat3(unity_ObjectToWorld);
	v_Normal = worldMat * _glesNormal;
	v_Tangent = worldMat * _glesTANGENT.xyz;
	v_Binormal = cross(v_Normal, v_Tangent) * _glesTANGENT.w;
	
	v_ViewDir = u_CameraPos - positionWorld.xyz;
	gl_Position = position;
}