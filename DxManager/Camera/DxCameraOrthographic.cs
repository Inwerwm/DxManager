using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxManager.Camera
{
    /// <summary>
    /// 平行投影カメラ
    /// </summary>
    public class DxCameraOrthographic : DxCamera
    {
        public override DxProjectionType ProjectionType => DxProjectionType.Orthographic;

        /// <summary>
        /// ビューボリュームの(幅, 高)
        /// </summary>
        public (float Width, float Height) ViewVolumeSize { get; set; }
        /// <summary>
        /// ビューボリュームのZの(最小, 最大)
        /// </summary>
        public (float Near, float Far) ViewVolumeDepth { get; set; }

        public override Matrix GetMatrix()
        {
            var view = base.GetMatrix();
            switch (Hand)
            {
                case HandedSystem.Right:
                    return view * Matrix.OrthoRH(ViewVolumeSize.Width, ViewVolumeSize.Height, ViewVolumeDepth.Near, ViewVolumeDepth.Far);
                case HandedSystem.Left:
                    return view * Matrix.OrthoLH(ViewVolumeSize.Width, ViewVolumeSize.Height, ViewVolumeDepth.Near, ViewVolumeDepth.Far);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
