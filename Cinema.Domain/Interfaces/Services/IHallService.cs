using Cinema.Domain.Entities;

namespace Cinema.Domain.Interfaces.Services;

public interface IHallService
{
    Task<IReadOnlyList<Hall>> GetAllAsync(CancellationToken ct = default);
    Task<Hall> GetByIdAsync(int hallId, CancellationToken ct = default);
    Task<Hall> AddAsync(string name, int rows, int columns, bool isVip, CancellationToken ct = default);
}