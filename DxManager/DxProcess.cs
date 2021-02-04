using SlimDX.Direct3D11;
using System;

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
        public Effect Effect { get; }
        /// <summary>
        /// 描画デバイス
        /// </summary>
        public Device Device { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="device">使用するデバイス</param>
        /// <param name="effect">使用するエフェクト</param>
        protected DxProcess(Device device, Effect effect)
        {
            Device = device;
            Effect = effect;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// フレームごとの描画処理 
        /// </summary>
        public abstract void Update();

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
