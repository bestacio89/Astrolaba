using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de modification du périmètre d'un projet.</summary>
public sealed record SupprimerTacheProjetCommand(Guid ProjetId, string Uid) : ICommand<ResultatSaisieDto>;
