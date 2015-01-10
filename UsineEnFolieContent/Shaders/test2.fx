sampler s0;
float wave2;
float wave1;
float obj;
float lum(float4 color)
{
	return 0.4*color.r + 0.3*color.b + 0.3*color.g;
}

float4 RayFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float2 pos = coords;
	float lum = 0.0;
	for(int i = 1; i < 10; i++)
	{
		pos.x += 0.0005;
		lum += tex2D(s0, pos).r*i/50.0;
	}
	return float4(lum, lum, lum, 1);
}



technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 RayFunction();
	}
}