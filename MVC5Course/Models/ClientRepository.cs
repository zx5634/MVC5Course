using System;
using System.Linq;
using System.Collections.Generic;
	
namespace MVC5Course.Models
{   
	public  class ClientRepository : EFRepository<Client>, IClientRepository
	{
        //public IQueryable<Client> All(bool isAll = false)
        //{
        //    if(isAll)
        //    {
        //        return base.All();
        //    }
        //    return base.All().Where(x => x.CreditRating < 2);
        //}

        public IQueryable<Client> All()
        {
            return base.All().Where(x => x.Disable == false);
        }

        public IQueryable FindName(string keyword)
        {
            var client = this.All();
            if (!string.IsNullOrEmpty(keyword))
            {
                client = client.Where(x => x.FirstName.Contains(keyword));
            }
            return client;
        }

        public Client Find(int id)
        {
            return this.All().FirstOrDefault(x => x.ClientId == id);
        }

        public override void Delete(Client client)
        {
            client.Disable = true;
        }
	}

	public  interface IClientRepository : IRepository<Client>
	{

	}
}