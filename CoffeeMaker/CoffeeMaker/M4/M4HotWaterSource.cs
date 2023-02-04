using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMaker.M4
{
    public class M4HotWaterSource : HotWaterSource, Pollable
    {
        private CoffeeMakerAPI api;
        public M4HotWaterSource(CoffeeMakerAPI api)
        {
            this.api = api;
        }

        public override bool IsReady()
        {
            BoilerStatus boilerStatus = api.GetBoilerStatus();
            return boilerStatus == BoilerStatus.NOT_EMPTY;
        }

        public override void Pause()
        {
            api.SetBolierState(BoilerState.OFF);
            api.SetReliefValveState(ReliefValveState.OPEN);
        }

        public void Poll()
        {
            BoilerStatus boilerStatus = api.GetBoilerStatus();
            if(isBrewing)
            {
                if(boilerStatus== BoilerStatus.EMPTY) 
                {
                    api.SetBolierState(BoilerState.OFF);
                    api.SetReliefValveState(ReliefValveState.CLOSED);
                    DeclareDone();
                }
            }
        }

        public override void Resume()
        {
            api.SetBolierState(BoilerState.ON);
            api.SetReliefValveState(ReliefValveState.CLOSED);
        }

        public override void StartBrewing()
        {
            api.SetReliefValveState(ReliefValveState.CLOSED);
            api.SetBolierState(BoilerState.ON);
        }

    }
}
