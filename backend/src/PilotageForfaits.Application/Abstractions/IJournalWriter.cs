using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Abstractions;

/// <summary>
/// Appends a domain audit entry to the current persistence unit of work without committing it.
/// The entry must be saved atomically with the business change it describes.
/// </summary>
public interface IJournalWriter
{
    void Ajouter(EntreeJournal entree);
}
