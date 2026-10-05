namespace Cinema.Domain.Interfaces.Services;

public interface IDataPersistenceService
{
    Task LoadAllAsync(CancellationToken ct = default);
    Task SaveAllAsync(CancellationToken ct = default);
}