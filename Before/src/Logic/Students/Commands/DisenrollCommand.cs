namespace Logic.Students.Commands
{
    public sealed class DisenrollCommand : ICommand
    {
        public long Id { get; }

        public int EnrollmentNumber { get; }

        public string Comment { get; }

        public DisenrollCommand(long id, int enrollmentNumber, string comment)
        {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Comment = comment;
        }
    }
}
