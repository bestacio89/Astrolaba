using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirBilanIaTacheCommand(Guid ProjetId, string Uid, BilanIaCommande Donnees) : ICommand<ResultatSaisieDto>;
