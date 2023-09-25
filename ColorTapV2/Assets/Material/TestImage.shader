Shader "Custom/UIOutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.005
    }
 
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineWidth;
 
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate distance to the center of the UV coordinates
                float2 center = float2(0.5, 0.5);
                float2 delta = i.uv - center;
                float distance = length(delta);
 
                // Apply outline if the distance is within the outline width
                fixed4 outline = (distance > 0.5 - _OutlineWidth && distance < 0.5 + _OutlineWidth) ? _OutlineColor : fixed4(1, 1, 1, 0);
 
                // Sample the main texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
 
                // Combine the texture color and outline color
                fixed4 finalColor = texColor + outline;
                return finalColor;
            }
            ENDCG
        }
    }
}

