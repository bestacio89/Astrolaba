using System.Reflection;
using Franz.Common.Mediator.Dispatchers;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Messages;
using Microsoft.AspNetCore.Mvc;
using PilotageForfaits.Api.Controleurs;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Application.Brouillons;
using PilotageForfaits.Application.Calcul;
using PilotageForfaits.Application.Commun;
using PilotageForfaits.Application.Exports;
using PilotageForfaits.Application.Import;
using PilotageForfaits.Contracts.Import;
using PilotageForfaits.Contracts.Exports;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Application.Securite;
using PilotageForfaits.Contracts.Moi;
using PilotageForfaits.Contracts.Brouillons;
using PilotageForfaits.Contracts.Projets;
using PilotageForfaits.Contracts.Suivi;
using PilotageForfaits.Application.Projets;
using Xunit;

namespace PilotageForfaits.Architecture.Tests;

public sealed class LayerDependencyTests
{
    private static readonly Assembly ApiAssembly = typeof(ProjetsController).Assembly;
    private static readonly Assembly ContractsAssembly = typeof(CreerProjetVideRequete).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(PilotageForfaits.Application.DependencyInjection).Assembly;
    private static readonly Assembly DomainAssembly = typeof(PilotageForfaits.Domain.Entites.Projet).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(PilotageForfaits.Infrastructure.DependencyInjection).Assembly;

    [Fact]
    public void Contracts_must_not_reference_application_or_runtime_layers()
    {
        AssertNoProjectReferences(ContractsAssembly,
            "PilotageForfaits.Api",
            "PilotageForfaits.Application",
            "PilotageForfaits.Domain",
            "PilotageForfaits.Infrastructure");
    }

    [Fact]
    public void Api_must_reference_the_contracts_boundary()
    {
        var references = ApiAssembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty);

        Assert.Contains("PilotageForfaits.Contracts", references);
    }

    [Fact]
    public void Application_must_reference_contracts_for_use_case_messages()
    {
        var references = ApplicationAssembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty);

        Assert.Contains("PilotageForfaits.Contracts", references);
    }

    [Fact]
    public void Api_must_not_reference_database_provider_or_ef_core_runtime()
    {
        var references = ApiAssembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty);

        Assert.DoesNotContain("Microsoft.EntityFrameworkCore", references);
        Assert.DoesNotContain("Microsoft.Data.SqlClient", references);
    }

    [Fact]
    public void Api_must_reference_application_directly()
    {
        var references = ApiAssembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty);

        Assert.Contains("PilotageForfaits.Application", references);
    }

    [Fact]
    public void Domain_must_not_reference_outer_application_layers()
    {
        AssertNoProjectReferences(DomainAssembly,
            "PilotageForfaits.Application",
            "PilotageForfaits.Infrastructure",
            "PilotageForfaits.Api");
    }

    [Fact]
    public void Application_must_not_reference_api_or_infrastructure()
    {
        AssertNoProjectReferences(ApplicationAssembly,
            "PilotageForfaits.Infrastructure",
            "PilotageForfaits.Api");
    }

    [Fact]
    public void Extracted_application_workflows_must_not_depend_on_the_legacy_context()
    {
        var workflows = new[]
        {
            typeof(ConstructeurProjetCalcul),
            typeof(UniciteProjet),
            typeof(ServiceAcces),
            typeof(ServiceJournal),
            typeof(ProjetsService),
            typeof(BrouillonsService),
            typeof(SuiviService),
            typeof(ImportTspService),
            typeof(ExportsService)
        };

        foreach (var workflow in workflows)
        {
            var constructor = Assert.Single(workflow.GetConstructors());
            Assert.DoesNotContain(
                constructor.GetParameters(),
                parameter => parameter.ParameterType == typeof(IContexteDonnees));
        }
    }

    [Fact]
    public void Workshop_and_deliverable_workflows_must_use_the_persistence_port()
    {
        var constructor = Assert.Single(typeof(AteliersLivrablesService).GetConstructors());
        Assert.Contains(
            constructor.GetParameters(),
            parameter => parameter.ParameterType == typeof(IAteliersLivrablesPersistence));
    }

    [Fact]
    public void Expense_workflows_must_use_the_persistence_port()
    {
        var constructor = Assert.Single(typeof(DepensesService).GetConstructors());
        Assert.Contains(
            constructor.GetParameters(),
            parameter => parameter.ParameterType == typeof(IDepensesPersistence));
    }

    [Fact]
    public void Invoicing_workflows_must_use_the_persistence_port()
    {
        var constructor = Assert.Single(typeof(FacturationService).GetConstructors());
        Assert.Contains(
            constructor.GetParameters(),
            parameter => parameter.ParameterType == typeof(IFacturationPersistence));
    }

    [Fact]
    public void Infrastructure_must_not_reference_api()
    {
        AssertNoProjectReferences(InfrastructureAssembly, "PilotageForfaits.Api");
    }

    [Fact]
    public void Api_controllers_must_derive_from_controller_base()
    {
        var controllers = ApiAssembly.GetTypes()
            .Where(type => type.IsClass &&
                           type.Name.EndsWith("Controller", StringComparison.Ordinal) &&
                           type.Namespace == "PilotageForfaits.Api.Controleurs")
            .ToArray();

        Assert.NotEmpty(controllers);
        Assert.All(controllers, controller =>
            Assert.True(typeof(ControllerBase).IsAssignableFrom(controller),
                $"{controller.FullName} must derive from ControllerBase."));
    }

    [Fact]
    public void Api_controllers_must_dispatch_use_cases_instead_of_injecting_application_services()
    {
        var controllers = ApiAssembly.GetTypes()
            .Where(type => type.IsClass &&
                           type.Name.EndsWith("Controller", StringComparison.Ordinal) &&
                           type.Namespace == "PilotageForfaits.Api.Controleurs")
            .ToArray();

        Assert.NotEmpty(controllers);
        foreach (var controller in controllers)
        {
            var constructors = controller.GetConstructors();
            Assert.Single(constructors);
            var parameters = constructors[0].GetParameters();
            Assert.Single(parameters);
            Assert.Equal(typeof(IDispatcher), parameters[0].ParameterType);
        }
    }

    [Fact]
    public void Api_must_remain_the_composition_root_for_infrastructure()
    {
        var references = ApiAssembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty);

        Assert.Contains("PilotageForfaits.Infrastructure", references);
    }


    [Fact]
    public void Project_archive_command_must_use_the_Franz_mediator_contract()
    {
        var command = new ArchiverProjetCommand(Guid.NewGuid(), true);

        Assert.IsAssignableFrom<ICommand<bool>>(command);
    }

    [Fact]
    public void Project_archive_handler_must_implement_the_Franz_command_handler_contract()
    {
        Assert.True(typeof(ICommandHandler<ArchiverProjetCommand, bool>)
            .IsAssignableFrom(typeof(ArchiverProjetCommandHandler)));
    }

    [Fact]
    public void Migrated_application_commands_must_have_Franz_handlers()
    {
        Assert.True(typeof(ICommandHandler<CreerProjetVideMediatorCommand, Guid>)
            .IsAssignableFrom(typeof(CreerProjetVideCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerProjetCommand, bool>)
            .IsAssignableFrom(typeof(SupprimerProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<ImporterTspCommand, Guid>)
            .IsAssignableFrom(typeof(ImporterTspCommandHandler)));
        Assert.True(typeof(ICommandHandler<CreerBrouillonCommand, Guid>)
            .IsAssignableFrom(typeof(CreerBrouillonCommandHandler)));
        Assert.True(typeof(IQueryHandler<ObtenirBrouillonQuery, System.Text.Json.Nodes.JsonObject>)
            .IsAssignableFrom(typeof(ObtenirBrouillonQueryHandler)));
        Assert.True(typeof(ICommandHandler<EnregistrerBrouillonCommand, System.Text.Json.Nodes.JsonObject>)
            .IsAssignableFrom(typeof(EnregistrerBrouillonCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerBrouillonCommand, bool>)
            .IsAssignableFrom(typeof(SupprimerBrouillonCommandHandler)));
        Assert.True(typeof(ICommandHandler<LancerBrouillonCommand, Guid>)
            .IsAssignableFrom(typeof(LancerBrouillonCommandHandler)));
        Assert.True(typeof(IQueryHandler<ListerProjetsQuery, IReadOnlyList<ProjetResumeDto>>)
            .IsAssignableFrom(typeof(ListerProjetsQueryHandler)));
        Assert.True(typeof(IQueryHandler<ObtenirProjetQuery, ProjetDetailDto>)
            .IsAssignableFrom(typeof(ObtenirProjetQueryHandler)));
        Assert.True(typeof(IQueryHandler<AnalyserTspQuery, ApercuImportDto>)
            .IsAssignableFrom(typeof(AnalyserTspQueryHandler)));
        Assert.True(typeof(IQueryHandler<ListerBrouillonsQuery, IReadOnlyList<BrouillonResumeDto>>)
            .IsAssignableFrom(typeof(ListerBrouillonsQueryHandler)));
        Assert.True(typeof(ICommandHandler<ImporterBrouillonCommand, Guid>)
            .IsAssignableFrom(typeof(ImporterBrouillonCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirCoRedacteurBrouillonCommand, CoRedacteurReponse>)
            .IsAssignableFrom(typeof(DefinirCoRedacteurBrouillonCommandHandler)));
        Assert.True(typeof(IQueryHandler<ExporterBrouillonQuery, FichierExport>)
            .IsAssignableFrom(typeof(ExporterBrouillonQueryHandler)));
        Assert.True(typeof(ICommandHandler<ExporterCopilCommand, FichierExport>)
            .IsAssignableFrom(typeof(ExporterCopilCommandHandler)));
        Assert.True(typeof(ICommandHandler<ExporterCoprojCommand, FichierExport>)
            .IsAssignableFrom(typeof(ExporterCoprojCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirStatutTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirStatutTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirAvancementTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirAvancementTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirResteAFaireTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirResteAFaireTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirDatesTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirDatesTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirLibelleTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirLibelleTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirCommentaireTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirCommentaireTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirClassementTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirClassementTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirBilanIaTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirBilanIaTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirEcheanceTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirEcheanceTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<SaisirConsommeTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SaisirConsommeTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<CreerTacheProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(CreerTacheProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerTacheProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SupprimerTacheProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<AnnulerTacheProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(AnnulerTacheProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<RetablirTacheProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(RetablirTacheProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<AjouterProfilTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(AjouterProfilTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<ModifierChargeAjouteeCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ModifierChargeAjouteeCommandHandler)));
        Assert.True(typeof(ICommandHandler<RetirerChargeAjouteeCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(RetirerChargeAjouteeCommandHandler)));
        Assert.True(typeof(ICommandHandler<ReaffecterChargeTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ReaffecterChargeTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<AnnulerReaffectationTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(AnnulerReaffectationTacheCommandHandler)));
    }

    [Fact]
    public void Current_user_query_must_have_a_Franz_handler()
    {
        Assert.True(typeof(IQueryHandler<ObtenirMoiQuery, MoiDto>)
            .IsAssignableFrom(typeof(ObtenirMoiQueryHandler)));
    }

    [Fact]
    public void Tracking_read_query_must_have_a_Franz_handler()
    {
        Assert.True(typeof(IQueryHandler<ObtenirSuiviQuery, System.Text.Json.Nodes.JsonObject>)
            .IsAssignableFrom(typeof(ObtenirSuiviQueryHandler)));
    }

    [Fact]
    public void Remaining_tracking_workflow_handlers_must_implement_Franz_contracts()
    {
        Assert.True(typeof(ICommandHandler<DefinirDateAtelierCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirDateAtelierCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirHoraireAtelierCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirHoraireAtelierCommandHandler)));
        Assert.True(typeof(ICommandHandler<MarquerInvitationAtelierCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(MarquerInvitationAtelierCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirDateInvitationAtelierCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirDateInvitationAtelierCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirDestinatairesAtelierCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirDestinatairesAtelierCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirValidationClientLivrableCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirValidationClientLivrableCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirLienLivrableCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirLienLivrableCommandHandler)));
        Assert.True(typeof(ICommandHandler<EnregistrerPointHebdoSemaineCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(EnregistrerPointHebdoSemaineCommandHandler)));
        Assert.True(typeof(ICommandHandler<EnregistrerPointHebdoMoisCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(EnregistrerPointHebdoMoisCommandHandler)));
        Assert.True(typeof(ICommandHandler<ReglerPointHebdoCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ReglerPointHebdoCommandHandler)));
        Assert.True(typeof(ICommandHandler<AjouterMiseAJourTacheCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(AjouterMiseAJourTacheCommandHandler)));
        Assert.True(typeof(ICommandHandler<FigerReferenceProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(FigerReferenceProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirDeriveReferenceCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirDeriveReferenceCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirDateEcheanceCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirDateEcheanceCommandHandler)));
        Assert.True(typeof(ICommandHandler<LierPerimetreEcheanceCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(LierPerimetreEcheanceCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirEcheancePhaseCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirEcheancePhaseCommandHandler)));
        Assert.True(typeof(ICommandHandler<CreerActionRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(CreerActionRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<ModifierActionRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ModifierActionRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<RestaurerActionRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(RestaurerActionRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerActionRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SupprimerActionRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<CreerDecisionRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(CreerDecisionRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<ModifierDecisionRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ModifierDecisionRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerDecisionRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SupprimerDecisionRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<CreerRisqueRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(CreerRisqueRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<ModifierRisqueRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ModifierRisqueRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerRisqueRegistreCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SupprimerRisqueRegistreCommandHandler)));
        Assert.True(typeof(ICommandHandler<CreerAchatReelCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(CreerAchatReelCommandHandler)));
        Assert.True(typeof(ICommandHandler<ModifierAchatReelCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ModifierAchatReelCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerAchatReelCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SupprimerAchatReelCommandHandler)));
        Assert.True(typeof(ICommandHandler<CreerFraisReelCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(CreerFraisReelCommandHandler)));
        Assert.True(typeof(ICommandHandler<ModifierFraisReelCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(ModifierFraisReelCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerFraisReelCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SupprimerFraisReelCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirRegroupementProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirRegroupementProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirAlerteProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirAlerteProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirProfilIaProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirProfilIaProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirListeIaProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirListeIaProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirDatesProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirDatesProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<NommerAxeProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(NommerAxeProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<DefinirFicheProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(DefinirFicheProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<AjouterTypeProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(AjouterTypeProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<RenommerTypeProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(RenommerTypeProjetCommandHandler)));
        Assert.True(typeof(ICommandHandler<SupprimerTypeProjetCommand, ResultatSaisieDto>)
            .IsAssignableFrom(typeof(SupprimerTypeProjetCommandHandler)));
    }

    private static void AssertNoProjectReferences(Assembly assembly, params string[] forbiddenReferences)
    {
        var references = assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var forbiddenReference in forbiddenReferences)
        {
            Assert.DoesNotContain(forbiddenReference, references);
        }
    }
}
