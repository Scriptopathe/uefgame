sampler s0;
float wave2;
float wave1;
float obj;
float lum(float4 color)
{
	return 0.4*color.r + 0.3*color.b + 0.3*color.g;
}
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 c = tex2D(s0, coords);
	float2 pos = coords;
	float4 color;
	c.rgba = float4(0, 0, 0, 1);
	if(lum(tex2D(s0, float2(pos.x, pos.y))) < 0.3)
	{
		if(lum(tex2D(s0, float2(pos.x+0.001, pos.y))) >= 0.4)
		{
			c.rgba = 1;
		}
		if(lum(tex2D(s0, float2(pos.x-0.001, pos.y))) >= 0.4)
		{
			c.rgba = 1;
		}
		if(lum(tex2D(s0, float2(pos.x, pos.y+0.001))) >= 0.4)
		{
			c.rgba = 1;
		}
		if(lum(tex2D(s0, float2(pos.x, pos.y+0.001))) >= 0.4)
		{
			c.rgba = 1;
		}
	} 
	

	return c;
}

float4 BlackScreen(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 c = tex2D(s0, coords);
	float2 pos = coords;
	
	c.rgb += - distance(pos, float2(0.5, 0.5))*2;

	return c;
}


technique Technique1  
{  
    pass Pass1  
    {  
        PixelShader = compile ps_2_0 PixelShaderFunction();  
    }
}  