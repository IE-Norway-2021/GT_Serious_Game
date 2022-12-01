-   Poser des valeurs/listes des possibilités
    -   Bâtiments disponibles, combien ils coûtent, combien ils rapportent,...
    -   Actions disponibles, exactement lesquelles
-   Pour générer l’île, il y a un générateur de terrain dans unity :
    -   Add -> 3D Object -> Terrain
    -   ça va ouvrir un éditeur de terrain qui permet d'ajouter des montagnes,...
-   Essayer de prendre le plus possible d'assets stores

Structure :

Dans le dossier scripts, deux dossiers :

-   un dossier modèle, représente le modèle logique de l'application (aucune interface graphique)
-   un dossier pour toute la partie graphique/liée à l'affichage (classes qui héritent de MonoBehaviour)
-   Quelques classes qui font le liens entre les deu

# Reunion prof

-   Caméra : déplacer la caméra avec touches wasd, limiter le zoom, pouvoir changer orientation de la caméra haut bas, et droite gauche
-   Génération de l'île : utiliser un heightmap. Génère une carte d'une certaine taille avec des informations sur l'hauteur pour chaque point. On convertit ça en tileStack et on génère l'île avec ça, plus joli et donne un relief plus réaliste.
    -   Rendre la position des arbres aléatoire sur la tuille elle même
-   Interface : Deux barres, une argent l'autre CO2, se remplissent. SI CO2 plein on gagne, sinon construire l'hotel. Les usines coutent de l'argent, renvoient de l'argent et CO2
-   Update pour la génération des ressources : utiliser dans update, calculer le temps écoulé depuis le dernier update. A chaque fin de "secondes", le manager calcule le nombre de ressources générées, puis préviens à l'aide d'un event l'interface.
