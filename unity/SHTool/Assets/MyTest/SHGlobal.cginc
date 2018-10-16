
uniform float4 g_sph0;
uniform float4 g_sph1;
uniform float4 g_sph2;
uniform float4 g_sph3;
uniform float4 g_sph4;
uniform float4 g_sph5;
uniform float4 g_sph6;
uniform float4 g_sph7;
uniform float4 g_sph8;


 

float3 g_sh(  float3 worldNormal) {
	  float x = worldNormal.x;
	  float y = worldNormal.y;
	  float z = worldNormal.z;

	  float3 result = (
	    g_sph0.xyz +

	    g_sph1.xyz * x +
	    g_sph2.xyz * y +
	    g_sph3.xyz * z +

	    g_sph4.xyz * z * x +
	    g_sph5.xyz * y * z +
	    g_sph6.xyz * y * x +
	    g_sph7.xyz * (3.0 * z * z - 1.0) +
	    g_sph8.xyz * (x*x - y*y) 
	  );

	  return max(result, float3(0,0,0));
}