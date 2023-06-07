namespace Report_MS.Repository;

public interface IMongoRepository<T>
{
    Task Create(T entity);
    Task<T> GetById(string id);
    Task<IEnumerable<T>> GetAll();
    Task Update(T entity);
    Task Delete(string id);
}