using System;
using System.Threading.Tasks;
using Grpc.Core.Logging;
using MagicOnion.Server.Hubs;

namespace Pommel.Server.Controller.Filter
{
    public class ExceptionHandlingFilterAttribute : StreamingHubFilterAttribute
    {
        private readonly ILogger m_logger;

        public ExceptionHandlingFilterAttribute(ILogger logger)
        {
            m_logger = logger;
        }

        public override async ValueTask Invoke(StreamingHubContext context, Func<StreamingHubContext, ValueTask> next)
        {
            try
            {
                await next(context);
            }
            catch(Exception e)
            {
                m_logger.Debug($"{context.Path} catched exception.");
                m_logger.Debug($"{e}");
            }
        }
    }
}