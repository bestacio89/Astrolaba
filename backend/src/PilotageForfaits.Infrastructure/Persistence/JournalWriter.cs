using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// Adds domain audit entries to the scoped EF unit of work without saving early,
/// so the journal row commits or rolls back with the business mutation.
/// </summary>
internal sealed class JournalWriter(PilotageDbContext db) : IJournalWriter
{
    public void Ajouter(EntreeJournal entree)
    {
        ArgumentNullException.ThrowIfNull(entree);
        db.Journal.Add(entree);
    }
}
