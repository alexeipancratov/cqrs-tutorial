﻿using CSharpFunctionalExtensions;
using Logic.Decorators;
using Logic.Utils;

namespace Logic.Students.Commands
{
    public sealed class EditPersonalInfoCommand : ICommand
    {
        public long Id { get; }

        public string Name { get; }

        public string Email { get; }

        public EditPersonalInfoCommand(long id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        [DatabaseRetry]
        [AuditLog]
        internal sealed class EditPersonalInfoCommandHandler : ICommandHandler<EditPersonalInfoCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public EditPersonalInfoCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }

            public Result Handle(EditPersonalInfoCommand command)
            {
                // Creating the UnitOfWork instance here manually since we want to be able to retry DB queries
                // which is possible on new connection only.
                var unitOfWork = new UnitOfWork(_sessionFactory);
                var repository = new StudentRepository(unitOfWork);

                Student student = repository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                student.Name = command.Name;
                student.Email = command.Email;

                unitOfWork.Commit();

                return Result.Ok();
            }

            Result ICommandHandler<EditPersonalInfoCommand>.Handle(EditPersonalInfoCommand command)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
