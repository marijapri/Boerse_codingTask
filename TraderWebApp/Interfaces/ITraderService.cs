using TraderWebApp.Models;

namespace TraderWebApp.Interfaces
{
    public interface ITraderService
    {
        Task<List<OrderPlan>> CalculateBestPlanForUser(UserInputData userInputData);
    }
}
