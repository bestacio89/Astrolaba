using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de modification du périmètre d'un projet.</summary>
public sealed record ReaffecterChargeTacheCommand(Guid ProjetId, string Uid, ReaffectationCommande Donnees) : ICommand<ResultatSaisieDto>;
