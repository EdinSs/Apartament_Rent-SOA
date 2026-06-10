using EasyRent.Application.Common.Exceptions;
using EasyRent.Application.DTOs.Apartments;
using EasyRent.Application.DTOs.Common;
using EasyRent.Application.Interfaces.Repositories;
using EasyRent.Application.Interfaces.Services;
using EasyRent.Application.Mapping;

namespace EasyRent.Application.Services;

/// <summary>
/// Apartment use-cases: CRUD and search. Enforces the ownership rule —
/// only the owning landlord (or an admin, for delete) may modify a listing.
/// </summary>
public class ApartmentService : IApartmentService
{
    private readonly IApartmentRepository _apartmentRepo;

    public ApartmentService(IApartmentRepository apartmentRepo)
    {
        _apartmentRepo = apartmentRepo;
    }

    public async Task<PagedResult<ApartmentDto>> SearchAsync(ApartmentSearchDto search)
    {
        var (items, totalCount) = await _apartmentRepo.SearchAsync(search);
        return new PagedResult<ApartmentDto>
        {
            Items      = items.Select(a => a.ToDto()).ToList(),
            Page       = search.Page < 1 ? 1 : search.Page,
            PageSize   = search.PageSize < 1 ? 10 : search.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ApartmentDto> GetByIdAsync(int id)
    {
        var apt = await _apartmentRepo.GetByIdAsync(id)
                  ?? throw new NotFoundException("Apartment not found.");
        return apt.ToDto();
    }

    public async Task<ApartmentDto> CreateAsync(string landlordId, CreateApartmentDto dto)
    {
        var apt = dto.ToEntity(landlordId);
        await _apartmentRepo.AddAsync(apt);
        await _apartmentRepo.SaveChangesAsync();
        return apt.ToDto();
    }

    public async Task<ApartmentDto> UpdateAsync(string landlordId, int id, UpdateApartmentDto dto)
    {
        var apt = await _apartmentRepo.GetByIdAsync(id)
                  ?? throw new NotFoundException("Apartment not found.");

        if (apt.LandlordId != landlordId)
            throw new ForbiddenException("You can only modify your own apartments.");

        dto.ApplyTo(apt);
        _apartmentRepo.Update(apt);
        await _apartmentRepo.SaveChangesAsync();
        return apt.ToDto();
    }

    public async Task DeleteAsync(string requestingUserId, bool isAdmin, int id)
    {
        var apt = await _apartmentRepo.GetByIdAsync(id)
                  ?? throw new NotFoundException("Apartment not found.");

        // A landlord may delete only their own; an admin may delete any.
        if (!isAdmin && apt.LandlordId != requestingUserId)
            throw new ForbiddenException("You can only delete your own apartments.");

        _apartmentRepo.Delete(apt);
        await _apartmentRepo.SaveChangesAsync();
    }
}