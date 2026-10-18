# Moteur de la version de référence (POC v65)

Ce dossier contient le cœur du POC (fichiers `poc/core.js` et `poc/xlsx-lite.js`, extraits tels quels de la version de référence) et les scripts qui en tirent les **résultats attendus** des tests du serveur.

| Fichier | Rôle |
|---|---|
| `poc/core.js` | Lecture du TSP (`parseTSP`) et moteur de calcul (`compute`) du POC — **ne pas modifier**, sauf écart volontaire documenté (voir plus bas) : c'est la spécification |
| `poc/xlsx-lite.js` | Lecture des fichiers Excel du POC |
| `loadcore.cjs` | Charge le moteur dans Node.js, avec une date du jour fixe (`TODAY`, par défaut 2026-10-06) |
| `scenarios.cjs` | Construit les scénarios (entrée + résultats attendus) dans `backend/tests/Donnees` |
| `anonymise.py` | Fabrique une copie anonymisée d'un TSP réel (noms remplacés, macros retirées, valeurs conservées) |

## Régénérer les résultats attendus

Prérequis : Node.js 22.

```bash
TODAY=2026-10-06 node outils/reference-poc/scenarios.cjs
```

L'intégration continue refait ce calcul à chaque envoi et vérifie que les fichiers du dépôt n'ont pas été modifiés à la main.

## Ajouter un cas de test

1. Ajouter (ou compléter) un scénario dans `scenarios.cjs` : on part d'un TSP lu par le POC et on ajoute des saisies (consommé, statuts, annulations…).
2. Régénérer les fichiers.
3. Ajouter le nom du scénario dans `MoteurCalculReferenceTests` (projet `PilotageForfaits.Domain.Tests`).

## Faire évoluer une règle de calcul

Si une règle doit changer par rapport au POC, on ne modifie pas `core.js` : on documente la nouvelle règle dans `docs/regles-metier.md`, on écrit un test dédié dans `PilotageForfaits.Domain.Tests`, puis on adapte le moteur C#. Les scénarios de référence concernés sont alors mis à jour en connaissance de cause.

Quand la nouvelle règle change les résultats d'un scénario de référence, elle est reportée dans `core.js`, à cet endroit seulement, avec un commentaire « ÉCART VOLONTAIRE À LA V65 » qui renvoie à `docs/regles-metier.md` : la régénération (et le contrôle de l'intégration continue) reste ainsi reproductible. Écarts actuels :

- **Frais (règle du 09/10/2026)** : refacturables et à notre charge comptés à part (atterrissage, effet sur la marge, point de vigilance).
