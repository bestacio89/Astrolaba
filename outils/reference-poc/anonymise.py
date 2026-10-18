"""Produit une copie anonymisée d'un TSP pour les tests : noms de personnes et client remplacés,
macros retirées (.xlsm -> .xlsx). Les valeurs mémorisées des formules sont conservées (on touche au XML)."""
import sys, zipfile, re
src, dst = sys.argv[1], sys.argv[2]
REMPL = [
  ('OPXXXXX - SYNERGIE  - ME7 - TSP - V3', 'OP00000 - CLIENT EXEMPLE - TSP - V3'),
  ('SYNERGIE', 'CLIENT EXEMPLE'), ('Synergie', 'Client Exemple'), ('ECIF - ME7', 'Projet exemple'), ('ME7', 'PX1'),
  ('DEMON Sullivan', 'MARTIN Alex'), ('Sulivan DEMON', 'Alex MARTIN'), ('Sullivan', 'Alex'), ('DEMON', 'MARTIN'),
  ('Yann LECHLECH', 'Camille CLIENT'), ('LECHLECH', 'CLIENT'), ('Noel ROBERT', 'Directeur AGENCE'), ('Chloé DESBOIS', 'Responsable SL'),
  ('Rowann SERAFON', 'Commercial EXEMPLE'), ('Robin LE BOURHIS', 'Lou BERNARD'), ('Marc ZAGDHA', 'Sam PETIT'),
  ('Mathis BOURIAUD', 'Noa DURAND'), ('>MRT<', '>CDP<'),
  ('Nantes', 'Agence Ouest'),
]
zin = zipfile.ZipFile(src)
zout = zipfile.ZipFile(dst, 'w', zipfile.ZIP_DEFLATED)
for it in zin.infolist():
    n = it.filename
    if n.endswith('vbaProject.bin') or n.endswith('vbaProjectSignature.bin'):
        continue
    data = zin.read(n)
    if n.endswith('.xml') or n.endswith('.rels'):
        s = data.decode('utf-8')
        for a, b in REMPL: s = s.replace(a, b)
        s = re.sub(r'<dc:creator>[^<]*', '<dc:creator>Auteur', s)
        s = re.sub(r'<cp:lastModifiedBy>[^<]*', '<cp:lastModifiedBy>Auteur', s)
        s = re.sub(r'displayName="[^"]*"', 'displayName="Auteur"', s)
        s = re.sub(r'userId="[^"]*"', 'userId="auteur"', s)
        if n == '[Content_Types].xml':
            s = re.sub(r'<Override[^>]*vbaProject[^>]*/>', '', s)
            s = re.sub(r'<Default[^>]*Extension="bin"[^>]*/>', '', s)
            s = s.replace('application/vnd.ms-excel.sheet.macroEnabled.main+xml', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml')
        if n == 'xl/_rels/workbook.xml.rels':
            s = re.sub(r'<Relationship[^>]*vbaProject[^>]*/>', '', s)
        data = s.encode('utf-8')
    zout.writestr(it, data)
zout.close()
