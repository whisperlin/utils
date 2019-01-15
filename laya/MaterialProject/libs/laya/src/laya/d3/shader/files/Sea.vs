attribute vec4 _glesTANGENT;
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp mat4 unity_ObjectToWorld;
uniform highp mat4 unity_WorldToObject;
uniform highp mat4 unity_MatrixVP;

uniform highp vec3 objScale;
uniform highp float wavesIntensity;
uniform highp float displacementIntensity;
uniform sampler2D wavesTexture;
uniform highp vec4 wavesTexture_ST;
uniform sampler2D maskWavesDisplacement;
uniform highp vec4 maskWavesDisplacement_ST;
uniform sampler2D displacement;
uniform highp float radialWaves;
uniform highp float wavesAmount;
uniform highp float wavesDisplacementSpeed;
uniform highp float displacementScale;
uniform highp float displacementSpeed;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD5;
varying highp vec3 xlv_TEXCOORD6;
void main ()
{

  
  highp vec4 tmpvar_1;
  tmpvar_1.w = _glesVertex.w;
  highp mat3 tmpvar_2;
  tmpvar_2[0] = unity_WorldToObject[0].xyz;
  tmpvar_2[1] = unity_WorldToObject[1].xyz;
  tmpvar_2[2] = unity_WorldToObject[2].xyz;
  highp vec3 tmpvar_3;
  tmpvar_3 = normalize((_glesNormal * tmpvar_2));
  highp vec4 tmpvar_4;
  tmpvar_4.w = 0.0;
  tmpvar_4.xyz = _glesTANGENT.xyz;
  highp vec3 tmpvar_5;
  tmpvar_5 = normalize((unity_ObjectToWorld * tmpvar_4).xyz);
  highp float tmpvar_6;
  tmpvar_6 = (_Time.y * wavesDisplacementSpeed);
  highp vec4 tmpvar_7;
  tmpvar_7.zw = vec2(0.0, 0.0);
  tmpvar_7.xy = (((objScale.xz * displacementScale) * (_glesMultiTexCoord0.xy * 0.1)) + ((displacementSpeed * tmpvar_6) * vec2(-1.0, 1.0)));
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2DLod (displacement, tmpvar_7.xy, 0.0);
  highp vec4 tmpvar_9;
  tmpvar_9 = tmpvar_8;
  highp vec2 x_10;
  x_10 = ((_glesMultiTexCoord0.xy * 2.0) - 1.0);
  highp vec4 tmpvar_11;
  tmpvar_11.zw = vec2(0.0, 0.0);
  tmpvar_11.xy = ((-(_glesMultiTexCoord0.xy) * maskWavesDisplacement_ST.xy) + maskWavesDisplacement_ST.zw);
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2DLod (maskWavesDisplacement, tmpvar_11.xy, 0.0);
  highp vec4 tmpvar_13;
  tmpvar_13 = tmpvar_12;
  highp vec4 tmpvar_14;
  tmpvar_14.zw = vec2(0.0, 0.0);
  tmpvar_14.xy = (((
    (((mix (_glesMultiTexCoord0.xy, 
      (vec2((1.0 - sqrt(dot (x_10, x_10)))) * dot (tmpvar_13.xyz, vec3(0.3, 0.59, 0.11)))
    , vec2(radialWaves)) * objScale.xz) * wavesAmount) * 0.1)
   + vec2(
    -((wavesDisplacementSpeed * tmpvar_6))
  )) * wavesTexture_ST.xy) + wavesTexture_ST.zw);
  lowp vec4 tmpvar_15;
  tmpvar_15 = texture2DLod (wavesTexture, tmpvar_14.xy, 0.0);
  highp vec4 tmpvar_16;
  tmpvar_16 = tmpvar_15;
  tmpvar_1.xyz = (_glesVertex.xyz + ((
    (tmpvar_9.xyz + (dot (tmpvar_16.xyz, vec3(0.3, 0.59, 0.11)) * wavesIntensity))
   * vec3(0.0, 1.0, 0.0)) * displacementIntensity));
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = tmpvar_1.xyz;
  highp mat3 tmpvar_18;
  tmpvar_18[0] = unity_WorldToObject[0].xyz;
  tmpvar_18[1] = unity_WorldToObject[1].xyz;
  tmpvar_18[2] = unity_WorldToObject[2].xyz;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  gl_Position = (unity_MatrixVP * (unity_ObjectToWorld * tmpvar_17));
  xlv_TEXCOORD3 = normalize((_glesNormal * tmpvar_18));
  xlv_TEXCOORD2 = (unity_ObjectToWorld * tmpvar_1);
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = normalize(((
    (tmpvar_3.yzx * tmpvar_5.zxy)
   - 
    (tmpvar_3.zxy * tmpvar_5.yzx)
  ) * _glesTANGENT.w));
}
