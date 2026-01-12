using Ordering.Application.Features.Orders.Queries.ViewModels;
using Ordering.Domain.Aggregates;

namespace Ordering.Application.Contracts.Factories;

public interface IOrderViewModelFactory
{
    OrderViewModel CreateViewModel(Order order);
}