using CSharpFunctionalExtensions;

namespace Logic.Students.Commands
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Result Handle(TCommand command);
    }
}
