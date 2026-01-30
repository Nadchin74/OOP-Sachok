using System.Collections.Generic;
using System.Linq;

namespace IndependentWork16
{
    public interface ICartCalculator
    {
        decimal CalculateTotal(List<CartItem> items);
    }

    public class StandardCartCalculator : ICartCalculator
    {
        public decimal CalculateTotal(List<CartItem> items)
        {
            // Проста сума: ціна * кількість
            return items.Sum(item => item.Price * item.Quantity);
        }
    }
}