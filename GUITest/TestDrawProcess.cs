﻿using DxManager;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITest
{
    class TestDrawProcess : DxProcess
    {
        InputLayout vertexLayout;
        SlimDX.Direct3D11.Buffer vertexBuffer;

        public override void Init()
        {
            // 頂点情報のフォーマットを設定
            vertexLayout = new InputLayout(
                Context.Device,
                Effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature,
                new[] {
                    new InputElement
                    {
                        SemanticName = "SV_Position",
                        Format = SlimDX.DXGI.Format.R32G32B32_Float
                    }
                }
            );

            // 頂点バッファに頂点を追加
            using (SlimDX.DataStream vertexStream = new SlimDX.DataStream(
                new[] {
                    new SlimDX.Vector3(0, 0.5f, 0),
                    new SlimDX.Vector3(0.5f, 0, 0),
                    new SlimDX.Vector3(-0.5f, 0, 0),
                },
                true,
                true
            ))
            {
                vertexBuffer = new SlimDX.Direct3D11.Buffer(
                    Context.Device,
                    vertexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)vertexStream.Length,
                        BindFlags = BindFlags.VertexBuffer,
                    }
                );
            }
        }

        public override void Update()
        {
            // 背景を青一色に
            Context.Device.ImmediateContext.ClearRenderTargetView(Context.RenderTarget, new SlimDX.Color4(1, 0, 0, 1));

            // 三角形をデバイスに入力
            Context.Device.ImmediateContext.InputAssembler.InputLayout = vertexLayout;
            Context.Device.ImmediateContext.InputAssembler.SetVertexBuffers(
                0,
                new VertexBufferBinding(vertexBuffer, sizeof(float) * 3, 0)
            );
            Context.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // 三角形を描画
            Effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply(Context.Device.ImmediateContext);
            Context.Device.ImmediateContext.Draw(3, 0);

            // 描画内容を反映
            Context.SwapChain.Present(0, SlimDX.DXGI.PresentFlags.None);
        }
    }
}
