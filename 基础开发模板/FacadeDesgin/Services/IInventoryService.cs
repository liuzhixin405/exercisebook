namespace FacadeDesgin.Services;

public interface IInventoryService
{
    Task<bool> ReserveAsync(string product, int quantity);
    Task ReleaseAsync(string product, int quantity);
}
