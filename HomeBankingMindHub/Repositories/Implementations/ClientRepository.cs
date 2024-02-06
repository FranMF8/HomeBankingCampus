﻿using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories.Classes
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Client FindById(long id)
        {
            return FindByCondition(client => client.Id == id)
                .Include(client => client.Accounts)
                .Include(client => client.Cards)
                .Include(client => client.ClientLoans)
                .ThenInclude(cl => cl.Loan)
                .FirstOrDefault();
        }

        public IEnumerable<Client> GetAllClients()
        {
            return FindAll()
                .Include(client => client.Accounts)
                .Include(client => client.Cards)
                .Include(client => client.ClientLoans)
                .ThenInclude(cl => cl.Loan)
                .ToList();
        }

        public void Save(Client client)
        {
            Create(client);
            SaveChanges();
        }
    }
}
