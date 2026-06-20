
using IdentityService.Models;

namespace IdentityService.Interfaces;
public partial interface IClientTypeRepository
{
    Task<List<ClientTypes>> GetAllClientTypes();
    Task<ClientTypes?> GetClientTypeById(int id);
    Task<int> CreateClientType(ClientTypes clientType);
    Task<int> UpdateClientType(ClientTypes clientType);
    Task<int> DeleteClientType(int id);
}