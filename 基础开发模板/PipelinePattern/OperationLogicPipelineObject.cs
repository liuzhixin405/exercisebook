using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern
{
    internal delegate void OperationLogicPipelineObjectModules(Request request);
    internal class OperationLogicPipelineObject
    {
        private OperationLogicPipelineObjectModules modules;
        public void AddModule(OperationLogicPipelineObjectModules module)
        {
            module += module;
        }
        public void RunPipeline(Request request)
        {
            modules(request);
        }
    }
}
