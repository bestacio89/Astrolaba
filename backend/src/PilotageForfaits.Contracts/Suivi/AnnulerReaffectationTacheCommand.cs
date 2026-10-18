using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Commande de modification du périmètre d'un projet.</summary>
public sealed record AnnulerReaffectationTacheCommand(Guid ProjetId, string Uid, Guid ReaffectationId) : ICommand<ResultatSaisieDto>;
