Shader "Custom/ProceduralMeshGeneration"
{
    Properties
    {
        _MainTex ("Flag Texture", 2D) = "white" {}
        _WavingAmplitude("Waving Amplitude", Range(0, 5.0)) = 1
        _WavingFrequency("Waving Frequency", Range (0, 5.0)) = 1
        _ScrollSpeed("Texture Scroll Speed", Range(0, 5.0)) = 1
    }
    SubShader
    {
        Tags {"Queue"="Geometry" "RenderType"="Opaque"}
    
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WavingAmplitude;
            float _WavingFrequency;
            float _ScrollSpeed;
            
            struct Input {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                fixed4 lightingColor : COLOR0;
                
                SHADOW_COORDS(1)
            };
            
            Input vert (appdata_base v) {
                Input output;
                v.vertex.z += v.texcoord.x * sin(_Time.y * _WavingFrequency + v.vertex.x) * _WavingAmplitude;
                
                output.pos = UnityObjectToClipPos(v.vertex);
                output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half parallel = max (0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                output.lightingColor = parallel * _LightColor0;
                
                TRANSFER_SHADOW(output)
                
                return output;
            }
            
            fixed4 frag (Input i) : SV_Target {
                i.uv.x += _ScrollSpeed * _Time.x; 
                fixed4 color = tex2D (_MainTex, i.uv);
                
                fixed shadow = SHADOW_ATTENUATION(i);
                
                fixed3 lightAndShadow = i.lightingColor * shadow;
                color.rgb *= lightAndShadow;
                
                return color;
            }
            
            ENDCG
        }
        
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    FallBack "Diffuse"
}
