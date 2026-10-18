using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de saisie sur une tâche du suivi.</summary>
public sealed record DefinirEcheanceTacheCommand(Guid ProjetId, string Uid, EcheanceTacheCommande Donnees) : ICommand<ResultatSaisieDto>;
