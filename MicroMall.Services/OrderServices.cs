using MicroMall.IRepository;
using MicroMall.ISerivces;

namespace MicroMall.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IBaseRepository repository;

        public OrderServices(IBaseRepository repository)
        {
            this.repository = repository;
        }
        public string GetOrderNum()
        {
            return "";
        }
    }
}
