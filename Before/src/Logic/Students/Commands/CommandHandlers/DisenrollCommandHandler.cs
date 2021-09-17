using CSharpFunctionalExtensions;
using Logic.Utils;

namespace Logic.Students.Commands.CommandHandlers
{
    public sealed class DisenrollCommandHandler : ICommandHandler<DisenrollCommand>
    {
        private readonly UnitOfWork _unitOfWork;

        public DisenrollCommandHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Result Handle(DisenrollCommand command)
        {
            var studentRepository = new StudentRepository(_unitOfWork);

            Student student = studentRepository.GetById(command.Id);
            if (student == null)
                return Result.Fail($"No student found for Id {command.Id}");

            if (string.IsNullOrWhiteSpace(command.Comment))
                return Result.Fail("Disenrollment comment is required");

            Enrollment enrollment = student.GetEnrollment(command.EnrollmentNumber);
            if (enrollment == null)
                return Result.Fail($"No enrollment found with number '{command.EnrollmentNumber}'");

            student.RemoveEnrollment(enrollment, command.Comment);

            _unitOfWork.Commit();

            return Result.Ok();
        }
    }
}
