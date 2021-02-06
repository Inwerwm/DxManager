using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;
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
        #region フィールド
        private static DxContext instance;
        private bool disposedValue;
        private int refreshRate = 60;
        #endregion

        #region プロパティ
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
        public int RefreshRate
        {
            get => refreshRate;
            set
            {
                refreshRate = value;
                if (!(SwapChain is null))
                {
                    ChangeRefreshRate();
                }
            }
        }
        /// <summary>
        /// 描画処理のハンドル
        /// </summary>
        private EventHandler Drawloop { get; set; }
        #endregion

        #region 初期処理

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

        #endregion

        #region プロパティのインスタンスの作成

        private (Device device, SwapChain swapChain) CreateDeviceAndSwapChain()
        {
            Device device;
            SwapChain swapChain;
            int sample = 2;

            device = new Device(DriverType.Hardware, DeviceCreationFlags.None);
            swapChain = new SwapChain(device.Factory, device,
                new SwapChainDescription
                {
                    BufferCount = 1,
                    OutputHandle = TargetControl.Handle,
                    IsWindowed = true,
                    SampleDescription = new SampleDescription
                    {
                        Count = sample,
                        Quality = device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, sample) - 1
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
                }
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

        #endregion

        #region 描画設定

        private void SetViewport(Device device)
        {
            //var smallerEdge = TargetControl.Width > TargetControl.Height ? TargetControl.Height : TargetControl.Width;
            var largerEdge = TargetControl.Width < TargetControl.Height ? TargetControl.Height : TargetControl.Width;
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport
            {
                Width = largerEdge,
                Height = largerEdge,
                X = (TargetControl.Width - largerEdge) / 2,
                Y = (TargetControl.Height - largerEdge) / 2,
                MinZ = 0,
                MaxZ = 1,
            }
            );
        }

        /// <summary>
        /// 描画サイズを現在のTargetControlのサイズに変更
        /// </summary>
        public void ChangeResolution()
        {
            ChangeResolution(TargetControl.Width, TargetControl.Height);
        }

        /// <summary>
        /// 描画サイズの変更
        /// </summary>
        public void ChangeResolution(int width, int height)
        {
            RenderTarget?.Dispose();
            SwapChain.ResizeBuffers(1, width, height, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
            RenderTarget = CreateRenderTarget(Device, SwapChain);
            DepthStencil?.Dispose();
            DepthStencil = CreateDepthStencil(Device);
            SetViewport(Device);
        }

        private void ChangeRefreshRate()
        {
            SwapChain.ResizeTarget(new ModeDescription()
            {
                Width = SwapChain.Description.ModeDescription.Width,
                Height = SwapChain.Description.ModeDescription.Height,
                RefreshRate = new SlimDX.Rational(RefreshRate, 1),
                Format = SwapChain.Description.ModeDescription.Format,
            }
            );
        }

        #endregion

        #region 実行処理
        private void InitializeProcess(DxProcess process)
        {
            process.Context = this;
            process.Init();
        }
        private void CompileShader(DxProcess process, string shaderPath)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.CompileFromFile(shaderPath, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                process.Effect = new Effect(Device, shaderBytecode);
        }
        private void CompileShader(DxProcess process, byte[] shaderFile)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.Compile(shaderFile, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                process.Effect = new Effect(Device, shaderBytecode);
        }

        /// <summary>
        /// SlimDXによる描画処理を開始する
        /// </summary>
        /// <param name="process">実行する描画処理</param>
        public void StartDrawLoop(DxProcess process)
        {
            InitializeProcess(process);
            Application.Idle += Drawloop;
        }
        /// <summary>
        /// SlimDXによる描画処理を停止する
        /// </summary>
        public void StopDrawLoop()
        {
            Application.Idle -= Drawloop;
        }
        /// <summary>
        /// フォームの描画を開始
        /// </summary>
        /// <param name="form">描画するフォーム</param>
        /// <param name="process">実行する描画処理</param>
        public void Run(Form form, DxProcess process)
        {
            InitializeProcess(process);
            Application.Run(form);
        }

        /// <summary>
        /// フォームの描画を開始
        /// </summary>
        /// <param name="form">描画するフォーム</param>
        /// <param name="process">描画処理の記述されたクラス</param>
        /// <param name="shaderPath">シェーダーファイルのパス</param>
        public void Run(Form form, DxProcess process, string shaderPath)
        {
            CompileShader(process, shaderPath);
            CreateDrawloop(process);
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
            CompileShader(process, shaderFile);
            CreateDrawloop(process);
            Run(form, process);
        }
        /// <summary>
        /// フォームの描画にSlimDXによる描画を追加する
        /// </summary>
        /// <param name="process">描画処理の記述されたクラス</param>
        /// <param name="shaderPath">シェーダーファイルのパス</param>
        public void AddDrawloop(DxProcess process, string shaderPath)
        {
            CompileShader(process, shaderPath);
            CreateDrawloop(process);
            StartDrawLoop(process);
        }
        /// <summary>
        /// フォームの描画にSlimDXによる描画を追加する
        /// </summary>
        /// <param name="process">描画処理の記述されたクラス</param>
        /// <param name="shaderFile">シェーダーファイル</param>
        public void AddDrawloop(DxProcess process, byte[] shaderFile)
        {
            CompileShader(process, shaderFile);
            CreateDrawloop(process);
            StartDrawLoop(process);
        }

        private void CreateDrawloop(DxProcess process)
        {
            Drawloop = (sender, e) =>
            {
                while (AppStillIdle)
                {
                    process.Update();
                }
            };
        }

        #endregion

        #region Win32API
        struct Message
        {
            public System.IntPtr hWnd;
            public uint msg;
            public System.IntPtr wParam;
            public System.IntPtr lParam;
            public uint time;
            public System.Drawing.Point pt;
        }

        class Win32Api
        {
            [System.Security.SuppressUnmanagedCodeSecurity] //for performance
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool PeekMessage(
                out Message msg,
                System.IntPtr hWnd,
                uint messageFilterMin,
                uint messageFilterMax,
                uint flags
                );
        }
        bool AppStillIdle
        {
            get
            {
                Message msg;
                return !Win32Api.PeekMessage(out msg, System.IntPtr.Zero, 0, 0, 0);
            }
        }
        #endregion

        #region IDisposable

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

        #endregion
    }
}
