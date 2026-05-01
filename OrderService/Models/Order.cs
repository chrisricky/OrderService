namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Username { get; set; }   
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }     // Pending, Shipped, Completed
    }
}
