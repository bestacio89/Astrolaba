using System.ComponentModel.DataAnnotations;

namespace PilotageForfaits.Contracts.Projets;

/// <summary>Contrat HTTP de création d'un projet vide.</summary>
public sealed class CreerProjetVideRequete
{
    [Required(ErrorMessage = "Le client est obligatoire."), MaxLength(200)]
    public string? Client { get; set; }

    [Required(ErrorMessage = "Le nom du projet est obligatoire."), MaxLength(200)]
    public string? Nom { get; set; }

    [MaxLength(100)]
    public string? Reference { get; set; }

    [Required(ErrorMessage = "La date de début est obligatoire.")]
    public DateOnly? Debut { get; set; }

    [Required(ErrorMessage = "La fin contractuelle est obligatoire.")]
    public DateOnly? FinContractuelle { get; set; }

    [MaxLength(50)]
    public string? TypeContrat { get; set; }
}
