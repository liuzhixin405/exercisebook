using System;
using Raven.Client;
using Raven.Client.Embedded;

namespace LiveScoreEs
{
    public class RavenDbConfig
    {
        //public const String MyDefaultIndex = "StateChangeEvent/ByTimeStamp";
        private static IDocumentStore _instance;

        public static IDocumentStore Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("IDocumentStore not initialized.");
                return _instance;
            }
        }

        public static IDocumentStore Initialize()
        {
            _instance = new EmbeddableDocumentStore { ConnectionStringName = "RavenDB" };
            //_instance.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite; 
            _instance.Initialize();

            //_instance.DatabaseCommands.PutIndex(MyDefaultIndex,
            //                            new IndexDefinitionBuilder<StateChangeEvent>
            //                            {
            //                                Map = matchEvents => from mev in matchEvents select new { mev.Timestamp }
            //                            },
            //                            true /* override */);

            return _instance;
        }
    }
}