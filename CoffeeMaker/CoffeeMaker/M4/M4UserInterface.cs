using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMaker.M4
{
    public class M4UserInterface : UserInterface, Pollable
    {
        private CoffeeMakerAPI api;
        public M4UserInterface(CoffeeMakerAPI api)
        {
            this.api = api;
        }

        public override void CompleteCycle()
        {
            api.SetIndicatorState(IndicatorState.OFF);
        }

        public override void Done()
        {
            api.SetIndicatorState(IndicatorState.ON);
        }

        public void Poll()
        {
            BrewButtonStatus buttonStatus=api.GetBrewButtonStatus();
            if (buttonStatus == BrewButtonStatus.PUSHED)
            {
                StartBrewing();
            }
        }
    }
}
