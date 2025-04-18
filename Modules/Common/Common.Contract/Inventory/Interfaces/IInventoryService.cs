namespace Common.Contract.Inventory.Interfaces
{
    public interface IInventoryService
    {
        Task<FluentResults.Result> DecreaseStockAsync(Guid productId, int quantity);
        Task<FluentResults.Result<int>> GetStockAsync(Guid productId);
    }
}
