sampler foreground : register(s0);
float pixelWidth = 0.002;
float PixelKernel[13] =
{
    -6,
    -5,
    -4,
    -3,
    -2,
    -1,
     0,
     1,
     2,
     3,
     4,
     5,
     6,
};
float BlurWeights[13] = 
{
    0.002216,
    0.008764,
    0.026995,
    0.064759,
    0.120985,
    0.176033,
    0.199471,
    0.176033,
    0.120985,
    0.064759,
    0.026995,
    0.008764,
    0.002216,
};
// Effect function
float4 EffectProcess( float2 Tex : TEXCOORD0 ) : COLOR0
{
    // Apply surrounding pixels
    float4 color = 0;
    float2 samp = Tex;
    samp.x = Tex.x;

    for (int i = 0; i < 13; i++) {
        samp.y = Tex.y + PixelKernel[i] * pixelWidth;
        color += tex2D(foreground, samp.xy) * BlurWeights[i];
    }

    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 EffectProcess();
    }
}