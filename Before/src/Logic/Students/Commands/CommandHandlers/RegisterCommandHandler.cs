using CSharpFunctionalExtensions;
using Logic.Utils;
using System;

namespace Logic.Students.Commands.CommandHandlers
{
    public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly UnitOfWork _unitOfWork;

        public RegisterCommandHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Result Handle(RegisterCommand command)
        {
            var courseRepository = new CourseRepository(_unitOfWork);
            var studentRepository = new StudentRepository(_unitOfWork);

            var student = new Student(command.Name, command.Email);

            if (command.Course1 != null && command.Course1Grade != null)
            {
                Course course = courseRepository.GetByName(command.Course1);
                student.Enroll(course, Enum.Parse<Grade>(command.Course1Grade));
            }

            if (command.Course2 != null && command.Course2Grade != null)
            {
                Course course = courseRepository.GetByName(command.Course2);
                student.Enroll(course, Enum.Parse<Grade>(command.Course2Grade));
            }

            studentRepository.Save(student);
            _unitOfWork.Commit();

            return Result.Ok();
        }
    }
}
