using CSharpFunctionalExtensions;
using Logic.Students.Commands;
using Logic.Students.Queries;
using System;

namespace Logic.Utils
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

            //var handler = (ICommandHandler<ICommand>)_serviceProvider.GetService(handlerType);
            //Result result = handler.Handle(command);
            dynamic handler = _serviceProvider.GetService(handlerType);
            Result result = handler.Handle((dynamic)command);

            return result;
        }

        public TResult Dispatch<TResult>(IQuery<TResult> query)
        {
            Type type = typeof(IQueryHandler<,>);
            Type[] typeArgs = { query.GetType(), typeof(TResult) };
            Type handlerType = type.MakeGenericType(typeArgs);

            //var handler = (IQueryHandler<IQuery<TResult>, TResult>)_serviceProvider.GetService(handlerType);
            //TResult result = handler.Handle(query);
            dynamic handler = _serviceProvider.GetService(handlerType);
            TResult result = handler.Handle((dynamic)query);

            return result;
        }
    }
}
