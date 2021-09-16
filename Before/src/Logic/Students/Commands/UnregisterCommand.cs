namespace Logic.Students.Commands
{
    public sealed class UnregisterCommand : ICommand
    {
        public long Id { get; set; }

        public UnregisterCommand(long id)
        {
            Id = id;
        }
    }
}
