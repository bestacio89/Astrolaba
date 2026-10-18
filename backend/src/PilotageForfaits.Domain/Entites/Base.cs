using Franz.Common.Business.Domain;

namespace PilotageForfaits.Domain.Entites;

/// <summary>
/// Astrolaba's persisted entity base, built on Franz's audited IEntity<Guid> contract.
/// IDs remain generated when an entity is created; EF can materialize the private-set ID
/// through the mapped inherited property. Version remains Astrolaba's optimistic concurrency token.
/// </summary>
public abstract class Entite : Entity<Guid>
{
    protected Entite() : base(Guid.NewGuid())
    {
    }

    /// <summary>Numéro de version, augmenté automatiquement à chaque enregistrement d'une modification.</summary>
    public int Version { get; set; }
}

/// <summary>Objet qui appartient à un projet. La clé du projet sert à filtrer les données selon les habilitations.</summary>
public abstract class EntiteDeProjet : Entite
{
    public Guid ProjetId { get; set; }
}
