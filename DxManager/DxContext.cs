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
    public class DxContext : IDisposable
    {
        private static DxContext instance;
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
        public RenderTargetView RenderTarget { get; private set; }
        /// <summary>
        /// 深度バッファ
        /// </summary>
        public DepthStencilView DepthStencil { get; private set; }

        /// <summary>
        /// 描画対象のフォームコントロール
        /// </summary>
        public Control TargetControl { get; }
        /// <summary>
        /// リフレッシュレート
        /// </summary>
        public int RefreshRate { get; set; } = 60;

        /// <summary>
        /// インスタンスを取得する
        /// </summary>
        /// <param name="targetControl">描画対象コントロール</param>
        /// <returns>シングルトンなインスタンス</returns>
        public static DxContext GetInstance(Control targetControl)
        {
            instance = instance ?? new DxContext(targetControl);
            return instance;
        }

        private DxContext(Control targetControl)
        {
            TargetControl = targetControl;

            (Device, SwapChain, RenderTarget, DepthStencil) = InitializeProperties();
        }

        private (Device, SwapChain, RenderTargetView, DepthStencilView) InitializeProperties()
        {
            Device device;
            SwapChain swapChain;
            RenderTargetView renderTarget;
            DepthStencilView depthStencil;

            // DeviceとSwapChain
            (device, swapChain) = CreateDeviceAndSwapChain();

            // RenderTarget
            renderTarget = CreateRenderTarget(device, swapChain);

            // DepthStencil
            depthStencil = CreateDepthStencil(device);

            // Viewport
            SetViewport(device);

            return (device, swapChain, renderTarget, depthStencil);
        }

        private (Device device, SwapChain swapChain) CreateDeviceAndSwapChain()
        {
            Device device;
            SwapChain swapChain;

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
                                    RefreshRate = new SlimDX.Rational(RefreshRate, 1),
                                    Format = Format.R8G8B8A8_UNorm,
                                },
                                Usage = Usage.RenderTargetOutput,
                                Flags = SwapChainFlags.AllowModeSwitch
                            },
                            out device,
                            out swapChain
                        );
            return (device, swapChain);
        }

        private static RenderTargetView CreateRenderTarget(Device device, SwapChain swapChain)
        {
            RenderTargetView renderTarget;
            using (Texture2D backBuffer = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0))
            {
                renderTarget = new RenderTargetView(device, backBuffer);
                device.ImmediateContext.OutputMerger.SetTargets(renderTarget);
            }

            return renderTarget;
        }

        private DepthStencilView CreateDepthStencil(Device device)
        {
            DepthStencilView depthStencil;
            Texture2DDescription depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float,
                Width = TargetControl.ClientSize.Width,
                Height = TargetControl.ClientSize.Height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0)
            };
            using (Texture2D depthBuffer = new Texture2D(device, depthBufferDesc))
                depthStencil = new DepthStencilView(device, depthBuffer);
            return depthStencil;
        }

        private void SetViewport(Device device)
        {
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport
            {
                Width = TargetControl.Width,
                Height = TargetControl.Height,
                MaxZ = 1
            }
            );
        }

        /// <summary>
        /// 描画解像度の更新
        /// </summary>
        public void ResizeResolution()
        {
            RenderTarget?.Dispose();
            SwapChain.ResizeBuffers(1, TargetControl.Width, TargetControl.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
            RenderTarget = CreateRenderTarget(Device, SwapChain);
            DepthStencil?.Dispose();
            DepthStencil = CreateDepthStencil(Device);
            SetViewport(Device);
        }

        /// <summary>
        /// フォームの描画を開始
        /// </summary>
        /// <param name="form">描画するフォーム</param>
        /// <param name="process">描画処理の記述されたクラス</param>
        /// <param name="shaderPath">シェーダーファイルのパス</param>
        public void Run(Form form, DxProcess process, string shaderPath)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.CompileFromFile(shaderPath, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                process.Effect = new Effect(Device, shaderBytecode);
            Run(form, process);
        }

        /// <summary>
        /// フォームの描画を開始
        /// </summary>
        /// <param name="form">描画するフォーム</param>
        /// <param name="process">描画処理の記述されたクラス</param>
        /// <param name="shaderFile">シェーダーファイル</param>
        public void Run(Form form, DxProcess process, byte[] shaderFile)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.Compile(shaderFile, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                process.Effect = new Effect(Device, shaderBytecode);
            Run(form, process);
        }

        /// <summary>
        /// フォームの描画を開始
        /// </summary>
        /// <param name="form">描画するフォーム</param>
        /// <param name="process">描画処理の記述されたクラス</param>
        private void Run(Form form, DxProcess process)
        {
            process.Context = this;
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
        ~DxContext()
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
