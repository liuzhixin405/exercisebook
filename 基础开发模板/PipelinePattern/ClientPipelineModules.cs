using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelinePattern
{
    internal class ClientPipelineModules
    {
        public static void CheckRequestContent(Request request)
        {
            if(request==null || request.Content==null || string.IsNullOrEmpty(request.Content.Content))
            {
                throw new ArgumentNullException("Request or Request Content cannot be null or empty.");
            }
        }
        public static void AddRequestHeader(Request request)
        {
            request.Head.AppendLine($"Request Client Type: {request.ClientType.type}");
            request.Head.AppendLine($"Request Time: {DateTime.Now}");
        }
        public static void TransferRequestFormat(Request request)
        {
            request.Content.Content = TransferRequestForRest.Transfer(request.Content.Content);
        }
    }

    class TransferRequestForRest
    {
        internal static string Transfer(string content)
        {
           return ("Transferring request content to REST format...");
        }
    }
}
