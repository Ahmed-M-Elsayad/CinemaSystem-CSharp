using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces.Repositories;
using Cinema.Domain.Interfaces.Services;

namespace Cinema.Application.Services;

public sealed class HallService : IHallService
{
    private readonly IHallRepository _hallRepository;

    public HallService(IHallRepository hallRepository)
    {
        _hallRepository = hallRepository
            ?? throw new ArgumentNullException(nameof(hallRepository));
    }

    public Task<IReadOnlyList<Hall>> GetAllAsync(CancellationToken ct = default)
        => _hallRepository.GetAllAsync(ct);

    public async Task<Hall> GetByIdAsync(int hallId, CancellationToken ct = default)
    {
        var hall = await _hallRepository.GetByIdAsync(hallId, ct);
        if (hall is null)
            throw new EntityNotFoundException(nameof(Hall), hallId);
        return hall;
    }

    public async Task<Hall> AddAsync(
        string name, int rows, int columns, bool isVip,
        CancellationToken ct = default)
    {
        var nextId = await _hallRepository.GetNextIdAsync(ct);
        var hall = new Hall(nextId, name, rows, columns, isVip);
        await _hallRepository.AddAsync(hall, ct);
        return hall;
    }
}