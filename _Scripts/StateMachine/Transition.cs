public class Transition : ITransition
{
    public IState To { get; } 

    public IPredicate Condition { get; }

    public Transition ( IState To, IPredicate Condition)
    {
        this.To = To;
        this.Condition = Condition;
    } 
}