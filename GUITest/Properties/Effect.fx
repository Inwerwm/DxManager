struct VertexPositionColor
{
	float4 Position : SV_Position;
	float4 Color : COLOR;
};

VertexPositionColor MyVertexShader(VertexPositionColor input)
{
	return input;
}

float4 MyPixelShader(VertexPositionColor input) : SV_Target
{
	return input.Color;
}

technique10 MyTechnique
{
	pass MyPass
	{
		SetVertexShader(CompileShader(vs_5_0, MyVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, MyPixelShader()));
	}
}