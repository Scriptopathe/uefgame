sampler s0;
sampler map : register(s11);

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 bloomC = tex2D(s0, coords);
	float4 mapC = tex2D(map, coords);
	return mapC+bloomC*2;
}  

technique Technique1  
{  
    pass Pass1  
    {  
        PixelShader = compile ps_2_0 PixelShaderFunction();  
    }
}  