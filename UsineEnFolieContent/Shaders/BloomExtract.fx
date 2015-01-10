sampler s0;
const float threshold = 0.4;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 c = tex2D(s0, coords);
	c = saturate((c-0 - threshold) / (1-threshold));
	if(c.r > 0.2)
		c.rgb += 0.4;
	return c;
}  

technique Technique1  
{  
    pass Pass1  
    {  
        PixelShader = compile ps_2_0 PixelShaderFunction();  
    }
}  