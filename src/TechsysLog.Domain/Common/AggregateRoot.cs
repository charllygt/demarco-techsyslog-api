namespace TechsysLog.Domain.Common;

public abstract class AggregateRoot<TId>(TId id) : Entity<TId>(id)
    where TId : notnull
{
    // Lazy-init: persistência (ex.: MongoDB) cria a instância via FormatterServices
    // sem invocar field initializers; sem isso, _domainEvents seria null pós-rehidratação
    // e o primeiro Raise() lançaria NRE. Inicializamos sob demanda.
    private List<IDomainEvent>? _domainEvents;

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        (_domainEvents ?? []).AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) =>
        (_domainEvents ??= []).Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents?.Clear();
}
