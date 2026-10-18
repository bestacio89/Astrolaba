using System.Reflection;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Messages;
using Microsoft.AspNetCore.Mvc;
using PilotageForfaits.Api.Controleurs;
using PilotageForfaits.Application.Brouillons;
using PilotageForfaits.Application.Import;
using PilotageForfaits.Contracts.Import;
using PilotageForfaits.Contracts.Brouillons;
using PilotageForfaits.Contracts.Projets;
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
