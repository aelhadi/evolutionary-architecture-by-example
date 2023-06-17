namespace EvolutionaryArchitecture.Fitnet.Passes.RegisterPass;

using Contracts.SignContract.Events;
using Data;
using Data.Database;
using Events;
using Shared.Events;
using Shared.Events.EventBus;
using Shared.SystemClock;

internal sealed class ContractSignedEventHandler : IIntegrationEventHandler<ContractSignedEvent>
{
    private readonly PassesPersistence _persistence;
    private readonly IEventBus _eventBus;

    public ContractSignedEventHandler(
        PassesPersistence persistence,
        ISystemClock systemClock,
        IEventBus eventBus)
    {
        _persistence = persistence;
        _eventBus = eventBus;
    }

    public async Task Handle(ContractSignedEvent @event, CancellationToken cancellationToken)
    {
        var pass = Pass.Register(@event.ContractCustomerId, @event.ValidityFrom, @event.ValidityTo);
        await _persistence.Passes.AddAsync(pass, cancellationToken);
        await _persistence.SaveChangesAsync(cancellationToken);

        var passRegisteredEvent = PassRegisteredEvent.Create(pass.Id);
        await _eventBus.PublishAsync(passRegisteredEvent, cancellationToken);
    }
}