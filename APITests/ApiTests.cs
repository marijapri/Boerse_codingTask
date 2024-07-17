using Moq;
using TraderWebApp;
using TraderWebApp.Interfaces;
using TraderWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using TraderWebApp.Utils;
using TraderWebApp.Models;

namespace APITests
{
    public class Tests
    {
        private Mock<ITraderService> _traderService;
        private OrderController _controller;


        [SetUp]
        public void SetUp()
        {
            _traderService = new Mock<ITraderService>();
            _controller = new OrderController(_traderService.Object);
            
        }

        [Test]
        public async Task ReturnsBadRequest_WhenModelIsNull()
        {
            var result = await _controller.GetBestExecutionPlan(null);
            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }


        [Test]
        public void ReturnsException_When_OrdersIsNullOrEmpty()
        {

            var ex = Assert.Throws<Exception>(() =>
            {
                ExecutionPlanUtils.GetFinalExecutionPlan(OrderType.Buy, new List<Order>(), 5);
            });

            Assert.That(ex.Message, Is.EqualTo("We can't give you a plan."));
             ex = Assert.Throws<Exception>(() =>
            {
                ExecutionPlanUtils.GetFinalExecutionPlan(OrderType.Buy, null, 5);
            });

            Assert.That(ex.Message, Is.EqualTo("We can't give you a plan."));
        }
    }
}