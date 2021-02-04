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
    /// </summary>
    public class DxManager : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// 描画に使用するデバイス
        /// </summary>
        public Device Device { get; private set; }
        /// <summary>
        /// デバイスによって描画された画像を表示させるためのもの
        /// </summary>
        public SwapChain SwapChain { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public RenderTargetView RenderTarget { get; private set; }

        /// <summary>
        /// 描画対象のフォームコントロール
        /// </summary>
        public Control TargetControl { get; }


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
