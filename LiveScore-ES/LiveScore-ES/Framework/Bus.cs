using LiveScore_ES.Framework.Commands;

namespace LiveScore_ES.Framework
{
    public class Bus
    {
        private static readonly Dictionary<Type, Type> SagaStarters = new Dictionary<Type, Type>();
        private static readonly Dictionary<string, object> SagaInstances = new Dictionary<string, object>();

        public static void RegistorSaga<TStartMessage, TSaga>()
        {
            SagaStarters.Add(typeof(TStartMessage), typeof(TSaga));
        }
        public static void Send<T>(T message) where T : Message
        {
            // Check if the message can start one of the registered sagas
            if (SagaStarters.ContainsKey(typeof(T)))
            {
                var typeOfSaga = SagaStarters[typeof(T)];
                var instance = (IStartWithMessage<T>)Activator.CreateInstance(typeOfSaga);
                instance.Handle(message);

                SagaInstances[instance.SagaId] = instance;
            }
            // Publish & persist the event
            if (message is DomainEvent)
            {
                // Persist the event
                //new EventRepository().Save(message as DomainEvent).Commit();

                // Invoke all registered sagas and give each 
                // a chance to handle the event.
                //foreach (var sagaEntry in SagaInstances)
                //{
                //    var handler = (ICanHandleMessage<T>)sagaEntry.Value;
                //    if (handler != null)
                //        handler.Handle(message);
                //}
            }
            // The message doesn’t start any saga.
            // Check if the message can be delivered to an existing saga instead
            if (message.SagaId == null)
                return;
            if (SagaInstances.ContainsKey(message.SagaId))
            {
                if(message is EndMatchCommand)
                {
                    var saga = (ICanhandleMessage<T>)SagaInstances[message.SagaId];
                    saga.Handle(message);

                    // Saves saga back or remove if completed
                    //if (saga.IsComplete())
                    //    SagaInstances.Remove(message.Id);
                    //else
                    //    SagaInstances[message.Id] = saga;
                }

            }
        }
    }
}
