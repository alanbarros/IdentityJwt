namespace IdentityJwt.Repository
{
    public interface IRepository<T> where T : class
    {
        int Add(T entity);
    }
}