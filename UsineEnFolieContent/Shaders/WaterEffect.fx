sampler s0;
float wave2;
float wave1;
float obj;
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 color = tex2D(s0, coords);
	float2 c = coords;
	if(c.y < 0.5)
	{
		c.y = coords.y - cos(coords.x*26.4+ wave2)/30 + cos(coords.x*15+wave1)/20;
		color.rgb = tex2D(s0, c);
	}
	
	if(color.r > 0.9)
		return float4(0, 0, 0, 0);
    return color;
}  

technique Technique1  
{  
    pass Pass1  
    {  
        PixelShader = compile ps_2_0 PixelShaderFunction();  
    }
}  