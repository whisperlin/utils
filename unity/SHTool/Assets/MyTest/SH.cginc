
float3 sh(const float3 sph[9],  float3 worldNormal) {
	  float x = worldNormal.x;
	  float y = worldNormal.y;
	  float z = worldNormal.z;

	  float3 result = (
	    sph[0] +

	    sph[1] * x +
	    sph[2] * y +
	    sph[3] * z +

	    sph[4] * z * x +
	    sph[5] * y * z +
	    sph[6] * y * x +
	    sph[7] * (3.0 * z * z - 1.0) +
	    sph[8] * (x*x - y*y)
	  );

	  return max(result, float3(0.0));
}