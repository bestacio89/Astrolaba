using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using PilotageForfaits.Api.Controleurs;
using PilotageForfaits.Contracts.Projets;
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
