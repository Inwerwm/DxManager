using DxManager.Camera;
using SlimDX.Direct3D11;
using SlimDX.Multimedia;
using System;
using System.Diagnostics;
using System.Threading;

namespace DxManager
{
    /// <summary>
    /// SlimDXの描画処理抽象クラス
    /// </summary>
    public abstract class DxProcess : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// エフェクト
        /// </summary>
        public Effect Effect { get; set; }
        /// <summary>
        /// 描画デバイス
        /// </summary>
        public DxContext Context { get; set; }
        /// <summary>
        /// カメラ
        /// </summary>
        public DxCamera Camera { get; set; }

        /// <summary>
        /// 現在のFPS
        /// </summary>
        public float CurrentFPS { get; private set; }
        private Stopwatch Stopwatch { get; } = new Stopwatch();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected DxProcess()
        {
            InitializeInputDevice();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// フレームごとの描画処理 
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// フレームごとの更新処理
        /// </summary>
        public void Update()
        {
            Stopwatch.Restart();
            UpdateCamera();
            Draw();
            WaitTime();
        }

        /// <summary>
        /// カメラ更新処理
        /// </summary>
        protected abstract void UpdateCamera();

        private void WaitTime()
        {
            var processingTime = Stopwatch.ElapsedMilliseconds;
            var refreshRateTime = 1.0 / Context.RefreshRate * 1000;

            var idleTime = refreshRateTime - processingTime;
            if (idleTime >= 0)
                Thread.Sleep((int)Math.Round(idleTime, MidpointRounding.AwayFromZero));
            Stopwatch.Stop();
            CurrentFPS = 1000f / Stopwatch.ElapsedMilliseconds;
        }

        private void InitializeInputDevice()
        {
            SlimDX.RawInput.Device.RegisterDevice(UsagePage.Generic, UsageId.Mouse, SlimDX.RawInput.DeviceFlags.None);
            SlimDX.RawInput.Device.MouseInput += MouseInput;
            SlimDX.RawInput.Device.RegisterDevice(UsagePage.Generic, UsageId.Keyboard, SlimDX.RawInput.DeviceFlags.None);
            SlimDX.RawInput.Device.KeyboardInput += KeyboardInput;
        }

        /// <summary>
        /// マウス入力イベント発生時の処理
        /// </summary>
        protected virtual void MouseInput(object sender, SlimDX.RawInput.MouseInputEventArgs e) { }
        /// <summary>
        /// キーボード入力イベント発生時の処理
        /// </summary>
        protected virtual void KeyboardInput(object sender, SlimDX.RawInput.KeyboardInputEventArgs e) { }

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
                Effect?.Dispose();

                disposedValue = true;
            }
        }

        // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        ~DxProcess()
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
