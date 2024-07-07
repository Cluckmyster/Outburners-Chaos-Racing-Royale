Shader "Effects/FrostedGlass"
{
    Properties
    {
        _FrostTex("Frost Texture", 2D) = "white" {}
        _FrostIntensity("Frost Intensity", Range(0.0, 1.0)) = 0.5
        _ReflectionIntensity("Reflection Intensity", Range(0.0, 1.0)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" }
            LOD 100

            Pass
            {
                Tags { "SHADOWSUPPORT" = "true" "RenderType" = "Opaque" "PerformanceChecks" = "False" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uvfrost : TEXCOORD0;
                    float4 uvgrab : TEXCOORD1;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD2;
                    float3 viewDir : TEXCOORD3;
                };

                sampler2D _FrostTex;
                float4 _FrostTex_ST;

                float _FrostIntensity;
                float _ReflectionIntensity;

                sampler2D _GrabBlurTexture_0;
                sampler2D _GrabBlurTexture_1;
                sampler2D _GrabBlurTexture_2;
                sampler2D _GrabBlurTexture_3;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uvfrost = TRANSFORM_TEX(v.uv, _FrostTex);
                    o.uvgrab = ComputeGrabScreenPos(o.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Calculate world position
                    o.viewDir = normalize(WorldSpaceViewDir(v.vertex)); // Calculate view direction
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float surfSmooth = 1 - tex2D(_FrostTex, i.uvfrost) * _FrostIntensity;
                    surfSmooth = clamp(surfSmooth, 0, 1); // Correct clamping order

                    half4 refraction;

                    half4 ref00 = tex2Dproj(_GrabBlurTexture_0, i.uvgrab);
                    half4 ref01 = tex2Dproj(_GrabBlurTexture_1, i.uvgrab);
                    half4 ref02 = tex2Dproj(_GrabBlurTexture_2, i.uvgrab);
                    half4 ref03 = tex2Dproj(_GrabBlurTexture_3, i.uvgrab);

                    float step00 = smoothstep(0.75, 1.00, surfSmooth);
                    float step01 = smoothstep(0.5, 0.75, surfSmooth);
                    float step02 = smoothstep(0.05, 0.5, surfSmooth);
                    float step03 = smoothstep(0.00, 0.05, surfSmooth);

                    refraction = lerp(ref03, lerp(lerp(lerp(ref03, ref02, step02), ref01, step01), ref00, step00), step03);

                    // Add reflection component using reflection probe
                    float3 worldNormal = float3(0, 0, 1); // Normal vector for flat surface
                    float3 reflectDir = reflect(-i.viewDir, worldNormal); // Reflect view direction
                    half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflectDir); // Sample from reflection probe

                    // Combine refraction and reflection based on intensity
                    return lerp(refraction, reflection, _ReflectionIntensity);
                }
                ENDCG
            }
        }
}
