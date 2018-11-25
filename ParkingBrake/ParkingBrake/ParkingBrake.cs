using UnityEngine;

namespace ParkingBrake
{
    public class ParkingBrake : VesselModule
    {
        public static EventData<ParkingBrakeModule> onParkingBrake = new EventData<ParkingBrakeModule>("onParkingBrake");
        private bool currentBrakeState; // Current state of the brake


        /// <summary>
        /// Module start
        /// </summary>
        public new void Start()
        {
            onParkingBrake.Add(EngageParkingBrake);
        }


        /// <summary>
        /// Module destroy
        /// </summary>
        public void OnDestroy()
        {
            onParkingBrake.Remove(EngageParkingBrake);
        }


        /// <summary>
        /// Toggle brake
        /// </summary>
        /// <param name="v"></param>
        /// <param name="brakeState"></param>
        private void EngageParkingBrake(ParkingBrakeModule m)
        {
            if (m.vessel != vessel)
                return;

            if (m.BrakeActive == currentBrakeState)
                return;

            currentBrakeState = m.BrakeActive;
        }


        /// <summary>
        /// Stabilize vessel
        /// Borrowed from USI Tools
        /// </summary>
        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            if (!currentBrakeState)
                return;

            if (!vessel.Landed)
                return;
            
            vessel.permanentGroundContact = true;

            var c = vessel.parts.Count;
            for (int i = 0; i < c; ++i)
            {
                var r = vessel.parts[i].Rigidbody;
                if (r != null)
                {
                    r.angularVelocity *= 0;
                    r.velocity *= 0;
                }
            }
        }

    }

}
