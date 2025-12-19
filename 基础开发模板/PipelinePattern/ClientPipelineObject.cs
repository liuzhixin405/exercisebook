using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern
{
    internal delegate void ClientPipelineObjectModules(Request request);
    internal class ClientPipelineObject
    {
        private ClientPipelineObjectModules modules;

        public void AddModule(ClientPipelineObjectModules module)
        {
            module += module;
        }

        public void RunPipeline(Request request)
        {
            modules(request);
        }
    }
}
