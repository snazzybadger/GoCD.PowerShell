namespace GoCD.Powershell
{
    public interface Query<TResult> where TResult : QueryResult { }


    public interface QueryResult { }


    public interface QueryHandler<TQuery, TResult> 
        where TQuery : Query<TResult> 
        where TResult : QueryResult
    {
        TResult Execute(TQuery query);
    }
}
