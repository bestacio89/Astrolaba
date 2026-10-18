using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Suivi;
using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Handlers des workflows de suivi ; les règles restent dans les services applicatifs existants.</summary>
public sealed class DefinirDateAtelierCommandHandler(AteliersLivrablesService ateliersLivrablesService)
    : ICommandHandler<DefinirDateAtelierCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirDateAtelierCommand command, CancellationToken cancellationToken) =>
        ateliersLivrablesService.DefinirDateAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirHoraireAtelierCommandHandler(AteliersLivrablesService ateliersLivrablesService)
    : ICommandHandler<DefinirHoraireAtelierCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirHoraireAtelierCommand command, CancellationToken cancellationToken) =>
        ateliersLivrablesService.DefinirHoraireAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class MarquerInvitationAtelierCommandHandler(AteliersLivrablesService ateliersLivrablesService)
    : ICommandHandler<MarquerInvitationAtelierCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(MarquerInvitationAtelierCommand command, CancellationToken cancellationToken) =>
        ateliersLivrablesService.MarquerInvitationAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirDateInvitationAtelierCommandHandler(AteliersLivrablesService ateliersLivrablesService)
    : ICommandHandler<DefinirDateInvitationAtelierCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirDateInvitationAtelierCommand command, CancellationToken cancellationToken) =>
        ateliersLivrablesService.DefinirDateInvitationAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirDestinatairesAtelierCommandHandler(AteliersLivrablesService ateliersLivrablesService)
    : ICommandHandler<DefinirDestinatairesAtelierCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirDestinatairesAtelierCommand command, CancellationToken cancellationToken) =>
        ateliersLivrablesService.DefinirDestinatairesAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirValidationClientLivrableCommandHandler(AteliersLivrablesService ateliersLivrablesService)
    : ICommandHandler<DefinirValidationClientLivrableCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirValidationClientLivrableCommand command, CancellationToken cancellationToken) =>
        ateliersLivrablesService.DefinirValidationClientAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirLienLivrableCommandHandler(AteliersLivrablesService ateliersLivrablesService)
    : ICommandHandler<DefinirLienLivrableCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirLienLivrableCommand command, CancellationToken cancellationToken) =>
        ateliersLivrablesService.DefinirLienAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class EnregistrerPointHebdoSemaineCommandHandler(PointsHebdoService pointsHebdoService)
    : ICommandHandler<EnregistrerPointHebdoSemaineCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(EnregistrerPointHebdoSemaineCommand command, CancellationToken cancellationToken) =>
        pointsHebdoService.EnregistrerAsync(command.ProjetId, command.CleSemaine, command.Donnees, cancellationToken);
}

public sealed class EnregistrerPointHebdoMoisCommandHandler(PointsHebdoService pointsHebdoService)
    : ICommandHandler<EnregistrerPointHebdoMoisCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(EnregistrerPointHebdoMoisCommand command, CancellationToken cancellationToken) =>
        pointsHebdoService.EnregistrerAsync(command.ProjetId, command.CleMois, command.Donnees, cancellationToken);
}

public sealed class ReglerPointHebdoCommandHandler(PointsHebdoService pointsHebdoService)
    : ICommandHandler<ReglerPointHebdoCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ReglerPointHebdoCommand command, CancellationToken cancellationToken) =>
        pointsHebdoService.RegleAsync(command.ProjetId, command.Champ, command.Donnees, cancellationToken);
}

public sealed class AjouterMiseAJourTacheCommandHandler(MisesAJourService misesAJourService)
    : ICommandHandler<AjouterMiseAJourTacheCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(AjouterMiseAJourTacheCommand command, CancellationToken cancellationToken) =>
        misesAJourService.AjouterAsync(command.ProjetId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class FigerReferenceProjetCommandHandler(ReferencesService referencesService)
    : ICommandHandler<FigerReferenceProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(FigerReferenceProjetCommand command, CancellationToken cancellationToken) =>
        referencesService.FigerAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class DefinirDeriveReferenceCommandHandler(ReferencesService referencesService)
    : ICommandHandler<DefinirDeriveReferenceCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirDeriveReferenceCommand command, CancellationToken cancellationToken) =>
        referencesService.DefinirDeriveAsync(command.ProjetId, command.ReferenceId, command.Uid, command.Donnees, cancellationToken);
}

public sealed class DefinirDateEcheanceCommandHandler(FacturationService facturationService)
    : ICommandHandler<DefinirDateEcheanceCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirDateEcheanceCommand command, CancellationToken cancellationToken) =>
        facturationService.DefinirDateAsync(command.ProjetId, command.Numero, command.Donnees, cancellationToken);
}

public sealed class LierPerimetreEcheanceCommandHandler(FacturationService facturationService)
    : ICommandHandler<LierPerimetreEcheanceCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(LierPerimetreEcheanceCommand command, CancellationToken cancellationToken) =>
        facturationService.LierPerimetreAsync(command.ProjetId, command.Numero, command.Donnees, cancellationToken);
}

public sealed class DefinirEcheancePhaseCommandHandler(FacturationService facturationService)
    : ICommandHandler<DefinirEcheancePhaseCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirEcheancePhaseCommand command, CancellationToken cancellationToken) =>
        facturationService.DefinirEcheancePhaseAsync(command.ProjetId, command.Code, command.Donnees, cancellationToken);
}

public sealed class CreerActionRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<CreerActionRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(CreerActionRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.CreerActionAsync(command.ProjetId, cancellationToken);
}

public sealed class ModifierActionRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<ModifierActionRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ModifierActionRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.ModifierActionAsync(command.ProjetId, command.ActionId, command.Donnees, cancellationToken);
}

public sealed class RestaurerActionRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<RestaurerActionRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(RestaurerActionRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.RestaurerActionAsync(command.ProjetId, command.ActionId, command.Donnees, cancellationToken);
}

public sealed class SupprimerActionRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<SupprimerActionRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SupprimerActionRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.SupprimerActionAsync(command.ProjetId, command.ActionId, cancellationToken);
}

public sealed class CreerDecisionRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<CreerDecisionRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(CreerDecisionRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.CreerDecisionAsync(command.ProjetId, cancellationToken);
}

public sealed class ModifierDecisionRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<ModifierDecisionRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ModifierDecisionRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.ModifierDecisionAsync(command.ProjetId, command.DecisionId, command.Donnees, cancellationToken);
}

public sealed class SupprimerDecisionRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<SupprimerDecisionRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SupprimerDecisionRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.SupprimerDecisionAsync(command.ProjetId, command.DecisionId, cancellationToken);
}

public sealed class CreerRisqueRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<CreerRisqueRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(CreerRisqueRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.CreerRisqueAsync(command.ProjetId, cancellationToken);
}

public sealed class ModifierRisqueRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<ModifierRisqueRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ModifierRisqueRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.ModifierRisqueAsync(command.ProjetId, command.RisqueId, command.Donnees, cancellationToken);
}

public sealed class SupprimerRisqueRegistreCommandHandler(RegistresService registresService)
    : ICommandHandler<SupprimerRisqueRegistreCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SupprimerRisqueRegistreCommand command, CancellationToken cancellationToken) =>
        registresService.SupprimerRisqueAsync(command.ProjetId, command.RisqueId, cancellationToken);
}

public sealed class CreerAchatReelCommandHandler(DepensesService depensesService)
    : ICommandHandler<CreerAchatReelCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(CreerAchatReelCommand command, CancellationToken cancellationToken) =>
        depensesService.CreerAchatAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class ModifierAchatReelCommandHandler(DepensesService depensesService)
    : ICommandHandler<ModifierAchatReelCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ModifierAchatReelCommand command, CancellationToken cancellationToken) =>
        depensesService.ModifierAchatAsync(command.ProjetId, command.AchatId, command.Donnees, cancellationToken);
}

public sealed class SupprimerAchatReelCommandHandler(DepensesService depensesService)
    : ICommandHandler<SupprimerAchatReelCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SupprimerAchatReelCommand command, CancellationToken cancellationToken) =>
        depensesService.SupprimerAchatAsync(command.ProjetId, command.AchatId, cancellationToken);
}

public sealed class CreerFraisReelCommandHandler(DepensesService depensesService)
    : ICommandHandler<CreerFraisReelCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(CreerFraisReelCommand command, CancellationToken cancellationToken) =>
        depensesService.CreerFraisAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class ModifierFraisReelCommandHandler(DepensesService depensesService)
    : ICommandHandler<ModifierFraisReelCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(ModifierFraisReelCommand command, CancellationToken cancellationToken) =>
        depensesService.ModifierFraisAsync(command.ProjetId, command.FraisId, command.Donnees, cancellationToken);
}

public sealed class SupprimerFraisReelCommandHandler(DepensesService depensesService)
    : ICommandHandler<SupprimerFraisReelCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SupprimerFraisReelCommand command, CancellationToken cancellationToken) =>
        depensesService.SupprimerFraisAsync(command.ProjetId, command.FraisId, cancellationToken);
}

public sealed class DefinirRegroupementProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<DefinirRegroupementProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirRegroupementProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.DefinirRegroupementAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class DefinirAlerteProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<DefinirAlerteProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirAlerteProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.DefinirAlerteAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class DefinirProfilIaProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<DefinirProfilIaProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirProfilIaProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.DefinirProfilIaAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class DefinirListeIaProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<DefinirListeIaProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirListeIaProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.DefinirListeIaAsync(command.ProjetId, command.Type == ListeIaType.Levier ? ListeIa.Levier : ListeIa.Cause, command.Donnees, cancellationToken);
}

public sealed class DefinirDatesProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<DefinirDatesProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirDatesProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.DefinirDateAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class NommerAxeProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<NommerAxeProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(NommerAxeProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.NommerAxeAsync(command.ProjetId, command.Code, command.Donnees, cancellationToken);
}

public sealed class DefinirFicheProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<DefinirFicheProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(DefinirFicheProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.DefinirFicheAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class AjouterTypeProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<AjouterTypeProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(AjouterTypeProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.AjouterTypeAsync(command.ProjetId, command.Donnees, cancellationToken);
}

public sealed class RenommerTypeProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<RenommerTypeProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(RenommerTypeProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.RenommerTypeAsync(command.ProjetId, command.Code, command.Donnees, cancellationToken);
}

public sealed class SupprimerTypeProjetCommandHandler(ParametresService parametresService)
    : ICommandHandler<SupprimerTypeProjetCommand, ResultatSaisieDto>
{
    public Task<ResultatSaisieDto> Handle(SupprimerTypeProjetCommand command, CancellationToken cancellationToken) =>
        parametresService.SupprimerTypeAsync(command.ProjetId, command.Code, cancellationToken);
}
