using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMaker
{
    public abstract class ContainmentVessel
    {
        protected UserInterface ui;
        protected HotWaterSource hws;
        protected bool isBrewing;
        protected bool isComplete;
        public ContainmentVessel()
        {
            isBrewing = false;
            isComplete = true;
        }
        public void Init(UserInterface ui, HotWaterSource hws)
        {
            this.ui = ui;
            this.hws = hws;
        }
        public void Start()
        {
            isBrewing = true;
            isComplete=false;
        }
        public void Done()
        {
            isBrewing = false;
        }
        protected void DeclareComplete()
        {
          isComplete= true;
            ui.Complete();
        }
        protected void ContainerAvaliable()
        {
            hws.Resume();
        }
        protected void ContainerUnavailable()
        {
            hws.Done();
        }
        public abstract bool IsReady();
    }
}
