using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMaker.M4
{
    public class M4ContainmentVessel : ContainmentVessel, Pollable
    {
        private CoffeeMakerAPI api;
        private WarmerPlateStatus lastPotStatus;
        public M4ContainmentVessel(CoffeeMakerAPI api)
        {
            this.api = api;
            this.lastPotStatus = WarmerPlateStatus.POT_EMPTY;
        }

        public override bool IsReady()
        {
            WarmerPlateStatus plateStatus = api.GetWarmerPlayeStatus();
            return plateStatus == WarmerPlateStatus.POT_EMPTY;
        }

        public void Poll()
        {
            WarmerPlateStatus potStatus = api.GetWarmerPlayeStatus();
            if (potStatus != lastPotStatus)
            {
                if (isBrewing)
                {
                    HandleBrewingEvent(potStatus);
                }else if (isComplete == false)
                {
                    HandleBrewingEvent(potStatus);
                }
                lastPotStatus=potStatus;
            }
        }

        private void HandleBrewingEvent(WarmerPlateStatus potStatus)
        {
            if(potStatus== WarmerPlateStatus.POT_NOT_EMPTY)
            {
                ContainerAvaliable();
                api.SetWarmerState(WarmerState.ON);
            }else if(potStatus== WarmerPlateStatus.WARMER_EMPTY)
            {
                ContainerAvaliable();
                api.SetWarmerState(WarmerState.OFF);
            }
            else
            {
                //potStatus == POT_EMPTY;
                ContainerAvaliable();
            }
        }
    }
}
