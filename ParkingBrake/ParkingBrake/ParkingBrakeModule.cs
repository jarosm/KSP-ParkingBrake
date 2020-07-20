using KSP.Localization;
using UnityEngine;

namespace ParkingBrake
{
    public class ParkingBrakeModule : PartModule
    {
        /// <summary>
        /// Parking brake is active
        /// </summary>
        [KSPField(isPersistant = true)]
        private bool brakeActive = false;
        public bool BrakeActive
        {
            get { return brakeActive; }
            set
            {
                brakeActive = value;
                Events["ToggleParkingBrake"].guiName = (!brakeActive ? Localizer.Format("#LOC_PB_ContextMenu_Engage") : Localizer.Format("#LOC_PB_ContextMenu_Disengage"));
            }
        }


        /// <summary>
        /// Module start
        /// </summary>
        /// <param name="state">Start state</param>
        public override void OnStart(PartModule.StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                SynchronizeParkingBrakeModules();
                ParkingBrake.onParkingBrake.Fire(this, false);
            }
        }


        /// <summary>
        /// Toggle parking brake
        /// </summary>
        [KSPEvent(guiName = "Engage parking brake", guiActive = true, externalToEVAOnly = true, guiActiveEditor = false, active = true, guiActiveUnfocused = true, unfocusedRange = 3.0f)]
        public void ToggleParkingBrake()
        {
            brakeActive = !brakeActive;

            if (brakeActive)
            {
                if (!vessel.Landed)
                {
                    brakeActive = false;
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_PB_NotLanded")).color = Color.red;
                    return;
                }

                if (vessel.speed > 0.25)
                {
                    brakeActive = false;
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_PB_Moving")).color = Color.red;
                    return;
                }

                vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, true);
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_PB_Engaged"));
            }
            else
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_PB_Disengaged"));
            }

            ParkingBrake.onParkingBrake.Fire(this, true);
            SynchronizeParkingBrakeModules();
        }


        /// <summary>
        /// Synchronize brake state for all modules
        /// </summary>
        private void SynchronizeParkingBrakeModules()
        {
            var modules = vessel.FindPartModulesImplementing<ParkingBrakeModule>();
            var count = modules.Count;
            for (int i = 0; i < count; ++i)
                modules[i].BrakeActive = brakeActive;
        }

    }

}
