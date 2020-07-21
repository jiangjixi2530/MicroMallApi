using MicroMall.ISerivces;
using MicroMall.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroMall.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IRepository repository;

        public OrderServices(IRepository repository)
        {
            this.repository = repository;
        }
        public string GetOrderNum()
        {
            return "";
        }
    }
}
