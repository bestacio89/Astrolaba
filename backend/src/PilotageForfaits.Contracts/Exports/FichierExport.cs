namespace PilotageForfaits.Contracts.Exports;

/// <summary>Fichier produit par un cas d'usage, prêt à être retourné par l'API.</summary>
public sealed record FichierExport(byte[] Contenu, string Nom)
{
    public const string TypeXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
