namespace GoCD.Powershell
{
    public interface Command { }


    public interface CommandHandler<TCommand> where TCommand : Command
    {
        void Execute(TCommand command);
    }
}
