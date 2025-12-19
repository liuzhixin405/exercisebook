namespace PipelinePattern
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Request request = new Request
            {
                Content = new RequestContent
                {
                    Content = "Query order method",

                },
                ClientType = RequestClientTypeFactory.CreateRequestClientTypeForNetClient()
            };

            ClientPipelineObject pipe = new ClientPipelineObject();
            pipe.AddModule(ClientPipelineModules.CheckRequestContent);
            pipe.AddModule(ClientPipelineModules.AddRequestHeader);
            pipe.AddModule(ClientPipelineModules.TransferRequestFormat);
            pipe.AddModule(ClientPipelineModules.ReduceRequest);
            pipe.RunPipeline(request);

            IBuildOperationLogicPipelineObject buildOperationLogicPipelineObject = OperationLogicPipelineFactory.Creat(request.ClientType);
            OperationLogicPipelineObject operationLogicPipelineObject = buildOperationLogicPipelineObject.BuildOperationPipeline(request);
            operationLogicPipelineObject.RunPipeline(request);
        }
    }

    internal class OperationLogicPipelineFactory
    {
        internal static IBuildOperationLogicPipelineObject Creat(RequestClientType rtype)
        {
           if(rtype.type == RequestClientType.App)
            {
                return new ClientForAppType();
            }
           else if(rtype.type == RequestClientType.NetClient)
            {
                return new ClientForWebType();
            }
           else
            {
                throw new NotImplementedException();
            }
        }
    }
    public class ClientForAppType : IBuildOperationLogicPipelineObject
    {
        OperationLogicPipelineObject IBuildOperationLogicPipelineObject.BuildOperationPipeline(Request request)
        {
            var result = new OperationLogicPipelineObject();
            result.AddModule(requestObject =>
            {
                //log
            });
            result.AddModule(requestObject =>
            {
                //app
            });
            return result;
        }
    }

    public class ClientForWebType : IBuildOperationLogicPipelineObject
    {
        OperationLogicPipelineObject IBuildOperationLogicPipelineObject.BuildOperationPipeline(Request request)
        {
            var result = new OperationLogicPipelineObject();
            result.AddModule(requestObject =>
            {
                //log
            });
            result.AddModule(requestObject =>
            {
                //web
            });
            return result;
        }
    }
}
