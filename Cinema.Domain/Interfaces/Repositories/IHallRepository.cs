using Cinema.Domain.Entities;

namespace Cinema.Domain.Interfaces.Repositories;

public interface IHallRepository
{
    Task<IReadOnlyList<Hall>> GetAllAsync(CancellationToken ct = default);
    Task<Hall?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Hall hall, CancellationToken ct = default);
    Task UpdateAsync(Hall hall, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<int> GetNextIdAsync(CancellationToken ct = default);
    Task LoadAsync(CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
}