namespace NServiceBus
{
    using System;
    using System.Diagnostics;
    using System.Messaging;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Transports;
    using NServiceBus.Transports.Msmq;

    abstract class MsmqReceiveBehavior:ReceiveBehavior
    {
       
        [DebuggerNonUserCode]
        protected bool TryReceiveMessage(Func<Message> receive, IncomingContext context, out Message message)
        {
            message = null;

            var peekResetEvent = context.Get<MsmqContext>().PeekResetEvent;
             
            try
            {
                message = receive();
                return true;
            }
            catch (MessageQueueException messageQueueException)
            {
                if (messageQueueException.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    //We should only get an IOTimeout exception here if another process removed the message between us peeking and now.
                    return false;
                }
            }
            finally
            {
                peekResetEvent.Set();
            }

            return false;
        }


        protected void HandleCorruptMessage( IncomingContext context,Message message,Exception ex, Action<MessageQueue,Message> onError)
        {
            var errorQueue = context.Get<MsmqContext>().ErrorQueueAddress;

            LogCorruptedMessage(message, ex, errorQueue);


            using (var nativeErrorQueue = new MessageQueue(errorQueue.FullPath, false, true, QueueAccessMode.Send))
            {
                onError(nativeErrorQueue, message);
            }
        }


        void LogCorruptedMessage(Message message, Exception ex, MsmqAddress errorQueue)
        {
            var error = string.Format("Message '{0}' is corrupt and will be moved to '{1}'", message.Id, errorQueue.Queue);
            Logger.Error(error, ex);
        }

        static ILog Logger = LogManager.GetLogger<MsmqReceiveWithTransactionScopeBehavior>();
    }
}