using CSharpFunctionalExtensions;
using Logic.Utils;

namespace Logic.Students.Commands
{
    public sealed class UnregisterCommand : ICommand
    {
        public long Id { get; set; }

        public UnregisterCommand(long id)
        {
            Id = id;
        }

        internal sealed class UnregisterCommandHandler : ICommandHandler<UnregisterCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public UnregisterCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }

            public Result Handle(UnregisterCommand command)
            {
                var unitOfWork = new UnitOfWork(_sessionFactory);
                var studentRepository = new StudentRepository(unitOfWork);

                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                studentRepository.Delete(student);
                unitOfWork.Commit();

                return Result.Ok();
            }

            Result ICommandHandler<UnregisterCommand>.Handle(UnregisterCommand command)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
