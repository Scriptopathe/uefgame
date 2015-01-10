sampler s0;
float wave2;
float wave1;
float obj;
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 c = tex2D(s0, coords);
	c += tex2D(s0, float2(coords.x+0.01, coords.y));
	c += tex2D(s0, float2(coords.x-0.01, coords.y));
	c += tex2D(s0, float2(coords.x, coords.y+0.01));
	c += tex2D(s0, float2(coords.x, coords.y-0.01));

	c += tex2D(s0, float2(coords.x+0.005, coords.y));
	c += tex2D(s0, float2(coords.x-0.005, coords.y));
	c += tex2D(s0, float2(coords.x, coords.y+0.005));
	c += tex2D(s0, float2(coords.x, coords.y-0.005));

	c += tex2D(s0, float2(coords.x+0.005, coords.y+0.005));
	c += tex2D(s0, float2(coords.x-0.005, coords.y+0.005));
	c += tex2D(s0, float2(coords.x+0.005, coords.y-0.005));
	c += tex2D(s0, float2(coords.x-0.005, coords.y-0.005));
	return c/5;
}  

technique Technique1  
{  
    pass Pass1  
    {  
        PixelShader = compile ps_2_0 PixelShaderFunction();  
    }
}  