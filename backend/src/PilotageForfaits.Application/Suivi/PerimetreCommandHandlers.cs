using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Suivi;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Handlers des modifications de périmètre ; les règles restent dans PerimetreService.</summary>
public sealed class CreerTacheProjetCommandHandler(PerimetreService service)
    : ICommandHandler<CreerTacheProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(CreerTacheProjetCommand command, CancellationToken cancellationToken) =>
        service.CreerTacheAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class SupprimerTacheProjetCommandHandler(PerimetreService service)
    : ICommandHandler<SupprimerTacheProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SupprimerTacheProjetCommand command, CancellationToken cancellationToken) =>
        service.SupprimerTacheAsync(command.ProjetId, command.Uid, cancellationToken);
}

public sealed class AnnulerTacheProjetCommandHandler(PerimetreService service)
    : ICommandHandler<AnnulerTacheProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(AnnulerTacheProjetCommand command, CancellationToken cancellationToken) =>
        service.AnnulerAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class RetablirTacheProjetCommandHandler(PerimetreService service)
    : ICommandHandler<RetablirTacheProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(RetablirTacheProjetCommand command, CancellationToken cancellationToken) =>
        service.RetablirAsync(command.ProjetId, command.Uid, cancellationToken);
}

public sealed class AjouterProfilTacheCommandHandler(PerimetreService service)
    : ICommandHandler<AjouterProfilTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(AjouterProfilTacheCommand command, CancellationToken cancellationToken) =>
        service.AjouterProfilAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class ModifierChargeAjouteeCommandHandler(PerimetreService service)
    : ICommandHandler<ModifierChargeAjouteeCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ModifierChargeAjouteeCommand command, CancellationToken cancellationToken) =>
        service.ModifierAjoutAsync(command.ProjetId, command.Uid, command.ProfilId, command.Donnees, cancellationToken);
}

public sealed class RetirerChargeAjouteeCommandHandler(PerimetreService service)
    : ICommandHandler<RetirerChargeAjouteeCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(RetirerChargeAjouteeCommand command, CancellationToken cancellationToken) =>
        service.RetirerAjoutAsync(command.ProjetId, command.Uid, command.ProfilId, cancellationToken);
}

public sealed class ReaffecterChargeTacheCommandHandler(PerimetreService service)
    : ICommandHandler<ReaffecterChargeTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ReaffecterChargeTacheCommand command, CancellationToken cancellationToken) =>
        service.ReaffecterAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class AnnulerReaffectationTacheCommandHandler(PerimetreService service)
    : ICommandHandler<AnnulerReaffectationTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(AnnulerReaffectationTacheCommand command, CancellationToken cancellationToken) =>
        service.AnnulerReaffectationAsync(command.ProjetId, command.Uid, command.ReaffectationId, cancellationToken);
}
