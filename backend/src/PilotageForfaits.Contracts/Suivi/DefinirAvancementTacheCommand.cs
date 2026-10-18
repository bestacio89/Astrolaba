using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirAvancementTacheCommand(Guid ProjetId, string Uid, AvancementCommande Donnees) : ICommand<ResultatSaisieDto>;
