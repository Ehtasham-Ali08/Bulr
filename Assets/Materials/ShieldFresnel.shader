Shader "Custom/ShieldFresnel"
{
    Properties
    {
        _MainTex ("Hex Texture", 2D) = "white" {}
        _RimColor ("Rim Color (HDR)", Color) = (1,1,1,1)
        _RimPower ("Rim Power / Sharpness", Range(0.5, 8.0)) = 4.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha One // Additive blending for glowing shield effect
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        float4 _RimColor;
        float _RimPower;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            
            // Calculate Fresnel edge factor based on view angle
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            half rimGlow = pow(rim, _RimPower);

            o.Albedo = tex.rgb * _RimColor.rgb;
            // Center is completely transparent, edges glow bright
            o.Alpha = tex.a * rimGlow * _RimColor.a;
            o.Emission = _RimColor.rgb * rimGlow;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}