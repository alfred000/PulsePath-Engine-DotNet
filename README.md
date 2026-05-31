# PulsePath Engine - Moteur Logique & API REST 🚀

Bienvenue sur le dépôt Back-End de **PulsePath Engine**, un système expert prescriptif d'aide à la décision métabolique. Ce moteur calcule, ajuste et corrige dynamiquement les trajectoires énergétiques des utilisateurs sous contraintes physiologiques strictes.

## 🏗️ Architecture Multi-Stack & Parité Technique
Pour démontrer une flexibilité technique maximale, le moteur logique a été développé en parallèle sur deux écosystèmes majeurs, tous deux testés rigoureusement en TDD :
* **Écosystème .NET** : C# / .NET 8, ASP.NET Core Web API, Entity Framework Core, SQLite, xUnit.
* **Écosystème Node.js** : JavaScript (ES6+), Express.js, SQLite3 (Async/Await), Jest.

## 🛠️ Règles Métier Embarquées (Sélection)
* **RM-COR-01 (Moteur de Rattrapage Intelligent)** : Algorithme inspiré du Contrôle Prédictif (MPC) répartissant l'effort sur 7 jours (40% Alimentation / 60% Activité) avec barrières physiologiques inviolables (limite stricte au BMR de survie).
* **RM-MET-01** : Calcul dynamique du TDEE basé sur l'équation de Mifflin-St Jeor couplée au volume de pas réels.

## 🕸️ Écosystème du Projet (Toile d'Araignée)
Ce dépôt fait partie d'un triptyque interconnecté simulant un cycle SDLC complet :
* 📊 **[PulsePath-Analyse-Metier](https://github.com/alfred000/PulsePath-Engine-BA-Case-Study)** : Cadrage, processus BPMN, Backlog MoSCoW et User Stories.
* 💻 **[PulsePath-Engine-API](https://github.com/alfred000/PulsePath-Engine-DotNet)** : *Vous êtes ici* — Moteur logique, base de données SQLite et API REST.
* 🎨 **[PulsePath-Web-Angular](https://github.com/alfred000/PulsePath-Web-Angular)** : Interface client de calcul et tableau de bord dynamique à 3 blocs.


