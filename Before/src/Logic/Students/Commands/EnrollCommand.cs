namespace Logic.Students.Commands
{
    public sealed class EnrollCommand : ICommand
    {
        public long Id { get; }

        public string Course { get; }

        public string Grade { get; }

        public EnrollCommand(long id, string course, string grade)
        {
            Id = id;
            Course = course;
            Grade = grade;
        }
    }
}
