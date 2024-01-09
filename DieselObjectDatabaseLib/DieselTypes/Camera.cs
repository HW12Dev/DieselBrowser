using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
    public class Camera : Object3D
    {
        public class State
        {
            public float Unknown;
            public float TargetDist;
            public float NearRange;
            public float FarRange;
            public float Aspect;
            public static State Read(BinaryReader br)
            {
                State state = new State();

                state.Unknown = br.ReadSingle();
                state.TargetDist = br.ReadSingle();
                state.NearRange = br.ReadSingle();
                state.FarRange = br.ReadSingle();
                state.Aspect = br.ReadSingle();

                return state;
            }
        }

        public State CameraState { get; set; }

        public static Camera Read(BinaryReader br, bool Ballistics)
        {
            Camera camera = new Camera();

            camera.ReadObject3D(br, Ballistics);

            camera.CameraState = State.Read(br);

            return camera;
        }
    }
}
