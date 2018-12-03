// Amplify Impostors
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFYIMPOSTORS_INCLUDED
#define AMPLIFYIMPOSTORS_INCLUDED

// turn the next define to 1 to activate neighbour frame clipping
// use it only if your impostors show small artifacts at edges at some rotations
#define AI_CLIP_NEIGHBOURS_FRAMES 0

float2 VectortoOctahedron( float3 N )
{
	N /= dot( 1.0, abs(N) );
	if( N.z <= 0 )
	{
		N.xy = ( 1 - abs(N.yx) ) * ( N.xy >= 0 ? 1.0 : -1.0 );
	}
	return N.xy;
}

float2 VectortoHemiOctahedron( float3 N )
{
	N.xy /= dot( 1.0, abs(N) );
	return float2( N.x + N.y, N.x - N.y );
}

float3 OctahedronToVector( float2 Oct )
{
	float3 N = float3( Oct, 1.0 - dot( 1.0, abs(Oct) ) );
	if( N.z < 0 )
	{
		N.xy = ( 1 - abs(N.yx) ) * ( N.xy >= 0 ? 1.0 : -1.0 );
	}
	return normalize(N);
}

float3 HemiOctahedronToVector( float2 Oct )
{
	Oct = float2( Oct.x + Oct.y, Oct.x - Oct.y ) *0.5;
	float3 N = float3( Oct, 1 - dot( 1.0, abs(Oct) ) );
	return normalize(N);
}

uniform float _FramesX;
uniform float _FramesY;
uniform float _Frames;
uniform float _ImpostorSize;
uniform float _Parallax;
uniform sampler2D _Albedo;
uniform sampler2D _Normals;
uniform sampler2D _Specular;
uniform sampler2D _Emission;
uniform float _TextureBias;
uniform float _ClipMask;
uniform float _DepthSize;
uniform float _ShadowBias;
uniform float4 _Offset;

#ifdef EFFECT_HUE_VARIATION
	half4 _HueVariation;
#endif

inline void OctaImpostorVertex( inout appdata_full v, inout float4 uvsFrame1, inout float4 uvsFrame2, inout float4 uvsFrame3, inout float4 octaFrame, inout float4 viewPos )
{
	// Inputs
	float framesXY = _Frames;
	float prevFrame = framesXY - 1;
	float2 fractions = 1.0 / float2( framesXY, prevFrame );
	float fractionsFrame = fractions.x;
	float fractionsPrevFrame = fractions.y;
	float UVscale = _ImpostorSize;
	float parallax = -_Parallax; // check sign later

	// Basic data
	v.vertex.xyz += _Offset.xyz;
	float3 worldOrigin = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
	#if defined(UNITY_PASS_SHADOWCASTER)
		float3 worldCameraPos = 0;
		if( unity_LightShadowBias.y == 0.0 ){
			if( _WorldSpaceLightPos0.w == 1 )
				worldCameraPos = _WorldSpaceLightPos0.xyz;
			else
				worldCameraPos = _WorldSpaceCameraPos;
		} else {
			worldCameraPos = UnityWorldSpaceLightDir( mul(unity_ObjectToWorld, v.vertex).xyz ) * -5000.0;
		}
	#else
		float3 worldCameraPos = _WorldSpaceCameraPos;
	#endif

	float3 objectCameraDirection = normalize( mul( (float3x3)unity_WorldToObject, worldCameraPos - worldOrigin ) - _Offset.xyz );
	float3 objectCameraPosition = mul( unity_WorldToObject, float4( worldCameraPos, 1 ) ).xyz - _Offset.xyz; //ray origin

	// Create orthogonal vectors to define the billboard
	float3 upVector = float3( 0,1,0 );
	float3 objectHorizontalVector = normalize( cross( objectCameraDirection, upVector ) );
	float3 objectVerticalVector = cross( objectHorizontalVector, objectCameraDirection );

	// Billboard
	float2 uvExpansion = ( v.texcoord.xy - 0.5f ) * framesXY * fractionsFrame * UVscale;
	float3 billboard = objectHorizontalVector * uvExpansion.x + objectVerticalVector * uvExpansion.y + _Offset.xyz;

	float3 localDir = billboard - objectCameraPosition - _Offset.xyz;

	// Octahedron Frame
	#ifdef _HEMI_ON
		objectCameraDirection.y = max(0.001, objectCameraDirection.y);
		float2 frameOcta = VectortoHemiOctahedron( objectCameraDirection.xzy ) * 0.5 + 0.5;
	#else
		float2 frameOcta = VectortoOctahedron( objectCameraDirection.xzy ) * 0.5 + 0.5;
	#endif

	// Setup for octahedron
	float2 prevOctaFrame = frameOcta * prevFrame;
	float2 baseOctaFrame = floor( prevOctaFrame );
	float2 fractionOctaFrame = ( baseOctaFrame * fractionsFrame );

	// Octa 1
	float2 octaFrame1 = ( baseOctaFrame * fractionsPrevFrame ) * 2.0 - 1.0;
	#ifdef _HEMI_ON
		float3 octa1WorldY = HemiOctahedronToVector( octaFrame1 ).xzy;
	#else
		float3 octa1WorldY = OctahedronToVector( octaFrame1 ).xzy;
	#endif
	float3 octa1WorldX = normalize( cross( upVector , octa1WorldY ) + float3(-0.001,0,0));
	float3 octa1WorldZ = cross( octa1WorldX , octa1WorldY );

	float dotY1 = dot( octa1WorldY , localDir );
	float3 octa1LocalY = normalize( float3( dot( octa1WorldX , localDir ), dotY1, dot( octa1WorldZ , localDir ) ) );

	float lineInter1 = dot( octa1WorldY , -objectCameraPosition ) / dotY1; //minus??
	float3 intersectPos1 = ( lineInter1 * localDir + objectCameraPosition ); // should subtract offset??

	float dotframeX1 = dot( octa1WorldX , -intersectPos1 );
	float dotframeZ1 = dot( octa1WorldZ , -intersectPos1 );

	float2 uvFrame1 = float2( dotframeX1 , dotframeZ1 );

	if( lineInter1 <= 0.0 )
		uvFrame1 = 0;

	float2 uvParallax1 = octa1LocalY.xz * fractionsFrame * parallax;
	uvFrame1 = ( ( uvFrame1 / UVscale ) + 0.5 ) * fractionsFrame + fractionOctaFrame;
	uvsFrame1 = float4( uvParallax1, uvFrame1);

	// Octa 2
	float2 fractPrevOctaFrame = frac( prevOctaFrame );
	float2 cornerDifference = lerp( float2( 0,1 ) , float2( 1,0 ) , saturate( ceil( ( fractPrevOctaFrame.x - fractPrevOctaFrame.y ) ) ));
	float2 octaFrame2 = ( ( baseOctaFrame + cornerDifference ) * fractionsPrevFrame ) * 2.0 - 1.0;
	#ifdef _HEMI_ON
		float3 octa2WorldY = HemiOctahedronToVector( octaFrame2 ).xzy;
	#else
		float3 octa2WorldY = OctahedronToVector( octaFrame2 ).xzy;
	#endif

	float3 octa2WorldX = normalize( cross( upVector , octa2WorldY ) + float3(-0.001,0,0));
	float3 octa2WorldZ = cross( octa2WorldX , octa2WorldY );

	float dotY2 = dot( octa2WorldY , localDir );
	float3 octa2LocalY = normalize( float3( dot( octa2WorldX , localDir ), dotY2, dot( octa2WorldZ , localDir ) ) );

	float lineInter2 = dot( octa2WorldY , -objectCameraPosition ) / dotY2; //minus??
	float3 intersectPos2 = ( lineInter2 * localDir + objectCameraPosition );

	float dotframeX2 = dot( octa2WorldX , -intersectPos2 );
	float dotframeZ2 = dot( octa2WorldZ , -intersectPos2 );

	float2 uvFrame2 = float2( dotframeX2 , dotframeZ2 );

	if( lineInter2 <= 0.0 )
		uvFrame2 = 0;

	float2 uvParallax2 = octa2LocalY.xz * fractionsFrame * parallax;
	uvFrame2 = ( ( uvFrame2 / UVscale ) + 0.5 ) * fractionsFrame + ( ( cornerDifference * fractionsFrame ) + fractionOctaFrame );
	uvsFrame2 = float4( uvParallax2, uvFrame2);


	// Octa 3
	float2 octaFrame3 = ( ( baseOctaFrame + 1 ) * fractionsPrevFrame  ) * 2.0 - 1.0;
	#ifdef _HEMI_ON
		float3 octa3WorldY = HemiOctahedronToVector( octaFrame3 ).xzy;
	#else
		float3 octa3WorldY = OctahedronToVector( octaFrame3 ).xzy;
	#endif

	float3 octa3WorldX = normalize( cross( upVector , octa3WorldY ) + float3(-0.001,0,0));
	float3 octa3WorldZ = cross( octa3WorldX , octa3WorldY );

	float dotY3 = dot( octa3WorldY , localDir );
	float3 octa3LocalY = normalize( float3( dot( octa3WorldX , localDir ), dotY3, dot( octa3WorldZ , localDir ) ) );

	float lineInter3 = dot( octa3WorldY , -objectCameraPosition ) / dotY3; //minus??
	float3 intersectPos3 = ( lineInter3 * localDir + objectCameraPosition );

	float dotframeX3 = dot( octa3WorldX , -intersectPos3 );
	float dotframeZ3 = dot( octa3WorldZ , -intersectPos3 );

	float2 uvFrame3 = float2( dotframeX3 , dotframeZ3 );

	if( lineInter3 <= 0.0 )
		uvFrame3 = 0;

	float2 uvParallax3 = octa3LocalY.xz * fractionsFrame * parallax;
	uvFrame3 = ( ( uvFrame3 / UVscale ) + 0.5 ) * fractionsFrame + ( fractionOctaFrame + fractionsFrame );
	uvsFrame3 = float4( uvParallax3, uvFrame3);

	// maybe remove this?
	octaFrame = 0;
	octaFrame.xy = prevOctaFrame;
	#if AI_CLIP_NEIGHBOURS_FRAMES
		octaFrame.zw = fractionOctaFrame;
	#endif

	// view pos
	viewPos = 0;
	viewPos.xyz = UnityObjectToViewPos( billboard );

	#ifdef EFFECT_HUE_VARIATION
		float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);
		viewPos.w = saturate(hueVariationAmount * _HueVariation.a);
	#endif

	v.vertex.xyz = billboard;
	v.normal.xyz = objectCameraDirection;
}

inline void OctaImpostorFragment( inout SurfaceOutputStandardSpecular o, out float4 clipPos, out float3 worldPos, float4 uvsFrame1, float4 uvsFrame2, float4 uvsFrame3, float4 octaFrame, float4 interpViewPos )
{
	float depthBias = -1.0;
	float textureBias = _TextureBias;

	// Octa1
	float4 parallaxSample1 = tex2Dbias( _Normals, float4( uvsFrame1.zw, 0, depthBias) );
	float2 parallax1 = ( ( 0.5 - parallaxSample1.a ) * uvsFrame1.xy ) + uvsFrame1.zw;
	float4 albedo1 = tex2Dbias( _Albedo, float4( parallax1, 0, textureBias) );
	float4 normals1 = tex2Dbias( _Normals, float4( parallax1, 0, textureBias) );
	float4 mask1 = tex2Dbias( _Emission, float4( parallax1, 0, textureBias) );
	float4 spec1 = tex2Dbias( _Specular, float4( parallax1, 0, textureBias) );

	// Octa2
	float4 parallaxSample2 = tex2Dbias( _Normals, float4( uvsFrame2.zw, 0, depthBias) );
	float2 parallax2 = ( ( 0.5 - parallaxSample2.a ) * uvsFrame2.xy ) + uvsFrame2.zw;
	float4 albedo2 = tex2Dbias( _Albedo, float4( parallax2, 0, textureBias) );
	float4 normals2 = tex2Dbias( _Normals, float4( parallax2, 0, textureBias) );
	float4 mask2 = tex2Dbias( _Emission, float4( parallax2, 0, textureBias) );
	float4 spec2 = tex2Dbias( _Specular, float4( parallax2, 0, textureBias) );

	// Octa3
	float4 parallaxSample3 = tex2Dbias( _Normals, float4( uvsFrame3.zw, 0, depthBias) );
	float2 parallax3 = ( ( 0.5 - parallaxSample3.a ) * uvsFrame3.xy ) + uvsFrame3.zw;
	float4 albedo3 = tex2Dbias( _Albedo, float4( parallax3, 0, textureBias) );
	float4 normals3 = tex2Dbias( _Normals, float4( parallax3, 0, textureBias) );
	float4 mask3 = tex2Dbias( _Emission, float4( parallax3, 0, textureBias) );
	float4 spec3 = tex2Dbias( _Specular, float4( parallax3, 0, textureBias) );

	// Weights
	float2 fraction = frac( octaFrame.xy );
	float2 invFraction = 1 - fraction;
	float3 weights;
	weights.x = min( invFraction.x, invFraction.y );
	weights.y = abs( fraction.x - fraction.y );
	weights.z = min( fraction.x, fraction.y );

	// Blends
	float4 blendedAlbedo = albedo1 * weights.x  + albedo2 * weights.y + albedo3 * weights.z;
	float4 blendedNormal = normals1 * weights.x  + normals2 * weights.y + normals3 * weights.z;
	float4 blendedMask = mask1 * weights.x  + mask2 * weights.y + mask3 * weights.z;
	float4 blendedSpec = spec1 * weights.x  + spec2 * weights.y + spec3 * weights.z;

	float3 localNormal = blendedNormal.rgb * 2.0 - 1.0;
	float3 worldNormal = normalize( mul( unity_ObjectToWorld, float4( localNormal, 0 ) ).xyz );

	float3 viewPos = interpViewPos.xyz;
	viewPos.z += ( ( parallaxSample1.a * weights.x + parallaxSample2.a * weights.y + parallaxSample3.a * weights.z ) * 2.0 - 1.0) * 0.5 * _DepthSize * length( unity_ObjectToWorld[2].xyz );
	#ifdef UNITY_PASS_SHADOWCASTER
		if( _WorldSpaceLightPos0.w == 0 ){
			viewPos.z += -_ShadowBias * unity_LightShadowBias.y;
        } else {
			viewPos.z += -_ShadowBias;
		}
	#endif

	worldPos = mul( UNITY_MATRIX_I_V, float4( viewPos.xyz, 1 ) ).xyz;
	clipPos = mul( UNITY_MATRIX_P, float4( viewPos, 1 ) );
	#ifdef UNITY_PASS_SHADOWCASTER
		clipPos = UnityApplyLinearShadowBias( clipPos );
	#endif
	clipPos.xyz /= clipPos.w;
	if( UNITY_NEAR_CLIP_VALUE < 0 )
		clipPos = clipPos * 0.5 + 0.5;

	#ifdef EFFECT_HUE_VARIATION
		half3 shiftedColor = lerp(blendedAlbedo.rgb, _HueVariation.rgb, interpViewPos.w);
		half maxBase = max(blendedAlbedo.r, max(blendedAlbedo.g, blendedAlbedo.b));
		half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
		maxBase /= newMaxBase;
		maxBase = maxBase * 0.5f + 0.5f;
		shiftedColor.rgb *= maxBase;
		blendedAlbedo.rgb = saturate(shiftedColor);
	#endif
	
	#if AI_CLIP_NEIGHBOURS_FRAMES
		float t = ceil( fraction.x - fraction.y );
		float4 cornerDifference = float4( t, 1 - t, 1, 1 );

		float2 step_1 = ( uvsFrame1.zw - octaFrame.zw ) * _Frames;
		float4 step23 = ( float4( uvsFrame2.zw, uvsFrame3.zw ) -  octaFrame.zwzw ) * _Frames - cornerDifference;

		step_1 = step_1 * (1-step_1);
		step23 = step23 * (1-step23);

		float3 steps;
		steps.x = step_1.x * step_1.y;
		steps.y = step23.x * step23.y;
		steps.z = step23.z * step23.w;
		steps = step(-steps, 0);
	
		float final = dot( steps, weights );

		clip( final - 0.5 );
	#endif

	o.Albedo = blendedAlbedo.rgb;
	o.Normal = worldNormal;
	o.Emission = blendedMask.rgb;
	o.Specular = blendedSpec.rgb;
	o.Smoothness = blendedSpec.a;
	o.Occlusion = blendedMask.a;
	o.Alpha = ( blendedAlbedo.a - _ClipMask );
	clip( o.Alpha );
}

inline void SphereImpostorVertex( inout appdata_full v, inout float2 frameUVs, inout float4 viewPos )
{
	// INPUTS
	float sizeX = _FramesX;
	float sizeY = _FramesY - 1; // adjusted
	float3 fractions = 1 / float3( sizeX, _FramesY, sizeY );
	float2 sizeFraction = fractions.xy;
	float axisSizeFraction = fractions.z;

	// Basic data
	v.vertex.xyz += _Offset.xyz;
	float3 worldOrigin = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
	#if defined(UNITY_PASS_SHADOWCASTER)
		float3 worldCameraPos = 0;
		if( unity_LightShadowBias.y == 0.0 ){
			if( _WorldSpaceLightPos0.w == 1 )
				worldCameraPos = _WorldSpaceLightPos0.xyz;
			else
				worldCameraPos = _WorldSpaceCameraPos;
		} else {
			worldCameraPos = UnityWorldSpaceLightDir( mul(unity_ObjectToWorld, v.vertex).xyz ) * -5000.0;
		}
	#else
		float3 worldCameraPos = _WorldSpaceCameraPos;
	#endif
	float3 objectCameraDirection = normalize( mul( (float3x3)unity_WorldToObject, worldCameraPos - worldOrigin ) - _Offset.xyz );

	// Create orthogonal vectors to define the billboard
	float3 upVector = float3( 0,1,0 );
	float3 objectHorizontalVector = normalize( cross( objectCameraDirection, upVector ) );
	float3 objectVerticalVector = cross( objectHorizontalVector, objectCameraDirection );

	// Create vertical radial angle
	float verticalAngle = frac( atan2( -objectCameraDirection.z, -objectCameraDirection.x ) / UNITY_TWO_PI ) * sizeX + 0.5;

	// Create horizontal radial angle
	float verticalDot = dot( objectCameraDirection, upVector );
	float upAngle = ( acos( -verticalDot ) / UNITY_PI ) + axisSizeFraction * 0.5f;
	float yRot = sizeFraction.x * UNITY_PI * verticalDot * ( 2 * frac( verticalAngle ) - 1 );

	// Billboard rotation
	float2 uvExpansion = v.texcoord.xy - 0.5;
	float cosY = cos( yRot );
	float sinY = sin( yRot );
	float2 uvRotator = mul( uvExpansion, float2x2( cosY , -sinY , sinY , cosY ) ) * _ImpostorSize;

	// Billboard
	float3 billboard = objectHorizontalVector * uvRotator.x + objectVerticalVector * uvRotator.y + _Offset.xyz;

	// Frame coords
	float2 relativeCoords = float2( floor( verticalAngle ), min( floor( upAngle * sizeY ), sizeY ) );
	float2 frameUV = ( v.texcoord.xy + relativeCoords ) * sizeFraction;

	frameUVs.xy = frameUV;
	viewPos.xyz = UnityObjectToViewPos( billboard );

	#ifdef EFFECT_HUE_VARIATION
		float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);
		viewPos.w = saturate(hueVariationAmount * _HueVariation.a);
	#endif

	v.vertex.xyz = billboard;
	v.normal.xyz = objectCameraDirection;
}

inline void SphereImpostorFragment( inout SurfaceOutputStandardSpecular o, out float4 clipPos, out float3 worldPos, float2 frameUV, float4 viewPos )
{
	float4 albedoSample = tex2Dbias( _Albedo, float4( frameUV, 0, _TextureBias) );
	float4 normalSample = tex2Dbias( _Normals, float4( frameUV, 0, _TextureBias) );
	float4 specularSample = tex2Dbias( _Specular, float4( frameUV, 0, _TextureBias) );
	float4 emissionSample = tex2Dbias( _Emission, float4( frameUV, 0, _TextureBias) );

	// Simple outputs
	float3 albedo = albedoSample.rgb;
	float3 emission = emissionSample.rgb;
	float3 specular = specularSample.rgb;
	float smoothness = specularSample.a;
	float occlusion = emissionSample.a;
	float alphaMask = albedoSample.a;

	// Normal
	float4 remapNormal = normalSample * 2 - 1; // object normal is remapNormal.rgb
	float3 worldNormal = normalize( mul( (float3x3)unity_ObjectToWorld, remapNormal.xyz ) );

	// Depth
	float depth = remapNormal.a * _DepthSize * 0.5;
	#if defined(UNITY_PASS_SHADOWCASTER)
	if( _WorldSpaceLightPos0.w == 0 ){
		depth = depth * 0.95 - 0.05  - _ShadowBias * unity_LightShadowBias.y;
    } else {
		depth = depth * 0.95 - 0.05  - _ShadowBias;
	}
	#endif
	viewPos.z += depth;

	// Modified clip position and world position
	worldPos = mul( UNITY_MATRIX_I_V, float4( viewPos.xyz, 1 ) ).xyz;

	clipPos = mul( UNITY_MATRIX_P, float4( viewPos.xyz, 1 ) );
	#ifdef UNITY_PASS_SHADOWCASTER
		clipPos = UnityApplyLinearShadowBias( clipPos );
	#endif
	clipPos.xyz /= clipPos.w;
	if( UNITY_NEAR_CLIP_VALUE < 0 )
		clipPos = clipPos * 0.5 + 0.5;

	#ifdef EFFECT_HUE_VARIATION
		half3 shiftedColor = lerp(albedo.rgb, _HueVariation.rgb, viewPos.w);
		half maxBase = max(albedo.r, max(albedo.g, albedo.b));
		half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
		maxBase /= newMaxBase;
		maxBase = maxBase * 0.5f + 0.5f;
		shiftedColor.rgb *= maxBase;
		albedo.rgb = saturate(shiftedColor);
	#endif

	o.Albedo = albedo;
	o.Normal = worldNormal;
	o.Emission = emission;
	o.Specular = specular;
	o.Smoothness = smoothness;
	o.Occlusion = occlusion;
	o.Alpha = ( alphaMask - _ClipMask );
	clip( o.Alpha );
}
#endif
