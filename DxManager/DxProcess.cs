﻿using SlimDX.D3DCompiler;
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
        public DxContext Context { get; set; }

        /// <summary>
        /// シェーダーファイルを指定してインスタンスを作成
        /// </summary>
        /// <param name="shaderFile">シェーダーファイル Property.Resourceのものが指定できる</param>
        protected DxProcess(byte[] shaderFile)
        {
            Effect = LoadEffect(shaderFile);
        }

        /// <summary>
        /// シェーダーファイルを指定してインスタンスを作成
        /// </summary>
        /// <param name="shaderPath">シェーダーファイルのパス</param>
        protected DxProcess(string shaderPath)
        {
            Effect = LoadEffect(shaderPath);
        }

        /// <summary>
        /// シェーダーファイルからエフェクトを読み込む
        /// </summary>
        /// <param name="shaderPath">シェーダーのパス</param>
        /// <returns>エフェクト</returns>
        protected Effect LoadEffect(string shaderPath)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.CompileFromFile(shaderPath, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                return new Effect(Context.Device, shaderBytecode);
        }

        /// <summary>
        /// シェーダーファイルからエフェクトを読み込む
        /// </summary>
        /// <param name="shaderFile">シェーダーのバイト列 Property.Resourceを想定</param>
        /// <returns>エフェクト</returns>
        protected Effect LoadEffect(byte[] shaderFile)
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.Compile(shaderFile, "fx_5_0", ShaderFlags.None, EffectFlags.None))
                return new Effect(Context.Device, shaderBytecode);
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
