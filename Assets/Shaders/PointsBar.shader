Shader "Unlit/PointsBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Points ("Points", Float) = 0.5
        _LeftColour ("LeftColour", Color) = (1,0,0,1)
        _RightColour ("RightColour", Color) = (0,1,0,1)
        _BlendWidth ("BlendWidth", Float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Points;
            float _BlendWidth;
            fixed4 _LeftColour;
            fixed4 _RightColour;

            fixed4 lerp(fixed4 a, fixed4 b, float mixAmount){
                return a + mixAmount*(b - a);
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 xy = i.uv.xy;
    
                // calculate the distance from the current pixel's x-coordinate to the center of the blending range
                float distance = smoothstep(_Points - _BlendWidth/2.0, _Points + _BlendWidth/2.0, xy.x);

                // interpolate between the two colors based on the distance
                fixed4 col = lerp(_LeftColour, _RightColour, distance);

                return col;
            }
            ENDCG

            
        }
    }
}