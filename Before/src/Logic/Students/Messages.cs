using CSharpFunctionalExtensions;
using System;

namespace Logic.Students
{
    public sealed class Messages
    {
        private readonly IServiceProvider _serviceProvider;

        public Messages(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Result Dispatch(ICommand command)
        {
            Type type = typeof(ICommandHandler<>);
            Type typeArg = command.GetType();
            Type handlerType = type.MakeGenericType(typeArg);

            var handler = (ICommandHandler<ICommand>)_serviceProvider.GetService(handlerType);
            Result result = handler.Handle(command);

            return result;
        }
    }
}
