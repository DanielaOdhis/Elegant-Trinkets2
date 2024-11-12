using System;
using System.Threading.Tasks;

namespace ElegantTrinkets2.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IRepository<CartItem> CartItems { get; }
        Task<int> SaveAsync();
    }
}
