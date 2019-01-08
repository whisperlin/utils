struct LayaGI
{
	half3 diffuse;
	half3 specular;
};

half4 BRDF1_Laya_PBS (half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness, half3 normal, half3 viewDir, half3 lightDir, LayaGI gi)
{
    half perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
    half3 halfDir = Unity_SafeNormalize (lightDir + viewDir);

	half nv = abs(dot(normal, viewDir));    

	half nl = saturate(dot(normal, lightDir));
	half nh = saturate(dot (normal, halfDir));

	half lv = saturate(dot(lightDir, viewDir));
	half lh = saturate(dot(lightDir, halfDir));

	// Diffuse term
	half diffuseTerm = DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;

	// Specular term
	half roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
		
	half V = SmithJointGGXVisibilityTerm (nl, nv, roughness);
	half D = GGXTerm (nh, roughness);

	half specularTerm = V * D * UNITY_PI;
	specularTerm = sqrt(max(1e-4h, specularTerm));
	specularTerm = max(0, specularTerm * nl);

	half surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
	half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));

	half3 color = diffColor * (gi.diffuse + _LightColor0 * diffuseTerm)
				+ specularTerm * _LightColor0 * FresnelTerm(specColor, lh)
				+ surfaceReduction * gi.specular * FresnelLerp(specColor, grazingTerm, nv);
	
    return half4(color, 1);
}
