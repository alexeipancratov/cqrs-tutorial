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
    }
}
