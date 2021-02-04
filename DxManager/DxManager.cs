using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Device = SlimDX.Direct3D11.Device;

namespace DxManager
{
    /// <summary>
    /// SlimDXをWinformで使用するためのラッパー
    /// <para>シングルトン</para>
    /// </summary>
    public class DxManager : IDisposable
    {
        private static DxManager instance;
        private bool disposedValue;

        /// <summary>
        /// 描画に使用するデバイス
        /// </summary>
        public Device Device { get; }
        /// <summary>
        /// デバイスによって描画された画像を表示させるためのもの
        /// </summary>
        public SwapChain SwapChain { get; }
        /// <summary>
        /// 描画対象
        /// </summary>
        public RenderTargetView RenderTarget { get;}
        /// <summary>
        /// 描画エフェクト
        /// </summary>
        public Effect Effect { get; private set; }

        /// <summary>
        /// 描画対象のフォームコントロール
        /// </summary>
        public Control TargetControl { get; }

        /// <summary>
        /// インスタンスを取得する
        /// </summary>
        /// <param name="targetControl">描画対象コントロール</param>
        /// <returns>シングルトンなインスタンス</returns>
        public static DxManager GetInstance(Control targetControl)
        {
            instance = instance ?? new DxManager(targetControl);
            return instance;
        }

        private DxManager(Control targetControl)
        {
            TargetControl = targetControl;

            (Device, SwapChain, RenderTarget) = InitializeProperties();
        }

        private (Device, SwapChain, RenderTargetView) InitializeProperties()
        {
            Device device;
            SwapChain swapChain;
            RenderTargetView renderTarget;

            // DeviceとSwapChain
            Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.None,
                new SwapChainDescription
                {
                    BufferCount = 1,
                    OutputHandle = TargetControl.Handle,
                    IsWindowed = true,
                    SampleDescription = new SampleDescription
                    {
                        Count = 1,
                        Quality = 0
                    },
                    ModeDescription = new ModeDescription
                    {
                        Width = TargetControl.ClientSize.Width,
                        Height = TargetControl.ClientSize.Height,
                        RefreshRate = new SlimDX.Rational(60, 1),
                        Format = Format.R8G8B8A8_UNorm
                    },
                    Usage = Usage.RenderTargetOutput
                },
                out device,
                out swapChain
            );

            // RenderTarget
            using (Texture2D backBuffer = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0))
            {
                renderTarget = new RenderTargetView(device, backBuffer);
                device.ImmediateContext.OutputMerger.SetTargets(renderTarget);
            }

            // Viewport
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport { Width = TargetControl.Width, Height = TargetControl.Height });

            return (device, swapChain, renderTarget);
        }

        /// <summary>
        /// シェーダーファイルからエフェクトを読み込む
        /// </summary>
        /// <param name="shaderPath">シェーダーのパス</param>
        /// <returns>エフェクト</returns>
        public Effect LoadEffect(string shaderPath)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.CompileFromFile(shaderPath, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                return new Effect(Device, shaderBytecode);
        }

        /// <summary>
        /// シェーダーファイルからエフェクトを読み込む
        /// </summary>
        /// <param name="shaderFile">シェーダーのバイト列 Property.Resourceを想定</param>
        /// <returns>エフェクト</returns>
        public Effect LoadEffect(byte[] shaderFile)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.Compile(shaderFile, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                return new Effect(Device, shaderBytecode);
        }

        /// <summary>
        /// フォームの描画を開始
        /// </summary>
        /// <param name="form">描画するフォーム</param>
        /// <param name="process">描画処理の記述されたクラス</param>
        public void Run(Form form, DxProcess process)
        {
            process.Init();
            SlimDX.Windows.MessagePump.Run(form, process.Update);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                RenderTarget?.Dispose();
                SwapChain?.Dispose();
                Device?.Dispose();

                disposedValue = true;
            }
        }

        // 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        ~DxManager()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
