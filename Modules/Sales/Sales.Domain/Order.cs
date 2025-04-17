namespace Sales.Domain;

public class Order
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public int OrderNo { get; set; }
    public string Customer { get; set; }
    public DateTime OrderDate { get; set; }

}
