namespace Basket.API.Entities;

public class ShoppingCart
{
    public string Username { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new();
    
    public ShoppingCart(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException(nameof(username));
        }
        Username = username;
    }

    public decimal TotalPrice => Items.Sum(i => i.Price*i.Quantity);
}