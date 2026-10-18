using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Suivi;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Handlers des mutations de tâches : les règles restent dans SaisiesTachesService.</summary>
public sealed class DefinirStatutTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirStatutTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirStatutTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirStatutAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirAvancementTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirAvancementTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirAvancementTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirAvancementAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirResteAFaireTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirResteAFaireTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirResteAFaireTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirResteAFaireAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirDatesTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirDatesTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirDatesTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirDatesAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirLibelleTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirLibelleTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirLibelleTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirLibelleAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirCommentaireTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirCommentaireTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirCommentaireTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirCommentaireAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirClassementTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirClassementTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirClassementTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirClassementAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirBilanIaTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirBilanIaTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirBilanIaTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirBilanIaAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirEcheanceTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<DefinirEcheanceTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirEcheanceTacheCommand command, CancellationToken cancellationToken) =>
        service.DefinirEcheanceAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class SaisirConsommeTacheCommandHandler(SaisiesTachesService service)
    : ICommandHandler<SaisirConsommeTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SaisirConsommeTacheCommand command, CancellationToken cancellationToken) =>
        service.SaisirConsommeAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}
