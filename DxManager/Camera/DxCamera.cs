using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxManager.Camera
{
    /// <summary>
    /// カメラ
    /// </summary>
    public abstract class DxCamera
    {
        /// <summary>
        /// 投影図法
        /// </summary>
        public abstract DxProjectionType ProjectionType { get; }
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position { get; set; } = new Vector3(0, 0, 1);
        /// <summary>
        /// 目標位置
        /// </summary>
        public Vector3 Target { get; set; } = new Vector3(0, 0, 0);
        /// <summary>
        /// 上方向ベクトル
        /// </summary>
        public Vector3 Up { get; set; } = new Vector3(0, 1, 0);
        /// <summary>
        /// 左右手系
        /// </summary>
        public HandedSystem Hand { get; set; } = HandedSystem.Right;

        /// <summary>
        /// 変換行列を取得
        /// </summary>
        /// <returns>変換行列</returns>
        public virtual Matrix GetMatrix()
        {
            return CreateViewMatrix();
        }

        /// <summary>
        /// ビュー座標変換行列を作成
        /// </summary>
        /// <returns>ビュー座標変換行列</returns>
        public Matrix CreateViewMatrix()
        {
            switch (Hand)
            {
                case HandedSystem.Right:
                    return Matrix.LookAtRH(Position, Target, Up);
                case HandedSystem.Left:
                    return Matrix.LookAtLH(Position, Target, Up);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
