using Cinemachine;
using UnityEngine;

namespace miniRAID.PixelArtRenderer
{
    public class CinemachinePixelPerfect3D : CinemachineExtension
    {
        public Vector3 targetPosition;
        
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Finalize)
            {
                var currentPos = state.CorrectedPosition;
                state.PositionCorrection += (targetPosition - currentPos);
            }
        }
    }
}