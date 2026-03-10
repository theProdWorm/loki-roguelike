Shader "Unlit/GlobalVolumetricFog"
{
    Properties
    {
        Colour("Color", Color) = (1, 1, 1, 1)
        MaxDistance("Max distance", float) = 100
        StepSize("Step size", Range(0.1, 20)) = 1
        DensityMultiplier("Density multiplier", Range(0, 10)) = 1
        NoiseOffset("Noise offset", float) = 0
        
        // _FogNoise("Fog noise", 3D) = "white" {}
        // _NoiseTiling("Noise tiling", float) = 1
        // DensityThreshold("Density threshold", Range(0, 1)) = 0.1
        
        // [HDR]LightContribution("Light contribution", Color) = (1, 1, 1, 1)
        // LightScattering("Light scattering", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 Colour;
            float MaxDistance;
            float DensityMultiplier;
            float StepSize;
            float NoiseOffset;
            TEXTURE3D(FogNoise);
            float DensityThreshold;
            float NoiseTiling;
            float4 LightContribution;
            float LightScattering;

            // float henyey_greenstein(float angle, float scattering)
            // {
            //     return (1.0 - angle * angle) / (4.0 * PI * pow(1.0 + scattering * scattering - (2.0 * scattering) * angle, 1.5f));
            // }
            
            float get_density()
            {
                return DensityMultiplier; // Placeholder for noise-based density
            }

            // float get_density(float3 worldPos)
            // {
            //     //float4 noise = FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
            //     //float density = dot(noise, noise);
            //     density = saturate(density - DensityThreshold) * DensityMultiplier;
            //     return density;
            // }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                float depth = SampleSceneDepth(IN.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

                float3 entryPoint = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float3 rayDir = normalize(viewDir);

                float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
                float distLimit = min(viewLength, MaxDistance);
                float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * NoiseOffset;
                float transmittance = 1;
                //float4 fogCol = Colour;

                while(distTravelled < distLimit)
                {
                    //float3 rayPos = entryPoint + rayDir * distTravelled;
                    float density = get_density();
                    if (density > 0)
                    {
                        // Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                        // fogCol.rgb += mainLight.color.rgb * LightContribution.rgb * henyey_greenstein(dot(rayDir, mainLight.direction), LightScattering) * density * mainLight.shadowAttenuation * StepSize;
                        transmittance *= exp(-density * StepSize);
                    }
                    distTravelled += StepSize;
                }
                
                return lerp(col, Colour, 1.0 - saturate(transmittance));
            }
            ENDHLSL
        }
    }
}
