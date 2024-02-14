using HomeBankingMindHub.Handlers.Implementations;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Classes;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Repositories.Implementations
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Card FindById(long id)
        {
            return FindByCondition( card => card.Id == id ).FirstOrDefault();
        }

        public Card FindByNumber(string number)
        {
            return FindByCondition(card => card.Number == number).FirstOrDefault();
        }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll().ToList();
        }

        public IEnumerable<Card> GetCardsByClient(long clientId)
        {
            return FindByCondition( card => card.ClientId == clientId).ToList();
        }

        public void Save(Card card)
        {
            bool condition = true;
            string cvv = string.Empty;
            string cardNumber = string.Empty;

            while (condition)
            {
                cardNumber = NumbersHandler.GenerateCardNumber();

                var dbCard = FindByNumber(cardNumber);

                if (dbCard == null)
                    condition = false;     
            }

            card.Number = cardNumber;
        }
    }
}
