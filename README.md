# 📚 BookStore - Application E-Commerce ASP.NET Core MVC

## 🎓 **Contexte Académique**
Projet développé dans le cadre du cours **"Retail (e-commerce)"** dispensé par **Prof. Marcel Stefan Wagner, PhD** à l'**ESTIAM**.

**Étudiants développeurs :**
- [Votre nom ici]
- [Nom du membre 2 si applicable]
- [Nom du membre 3 si applicable]

**Professeur :** Marcel Stefan Wagner, PhD

---

## 🎯 **Description du Projet**

BookStore est une application web e-commerce complète de vente de livres en ligne développée avec ASP.NET Core MVC 8.0. L'application implémente un système d'authentification multi-rôles, une gestion de panier interactive, et un système de crédits virtuels pour les démonstrations.

### **Fonctionnalités Principales**
- ✅ Système d'authentification avec 3 niveaux de droits (User, Librarian, Admin)
- ✅ Catalogue de livres avec recherche et filtrage avancés
- ✅ Panier d'achat interactif avec gestion temps réel
- ✅ Système de commandes complet avec suivi
- ✅ Panel d'administration pour la gestion des utilisateurs et contenus
- ✅ Tag Helpers personnalisés pour une interface riche
- ✅ Design moderne et responsive avec Bootstrap 5

---

## 🏗️ **Architecture Technique**

### **Stack Technologique**
- **Backend :** ASP.NET Core MVC 8.0
- **Frontend :** Razor Views + Bootstrap 5 + JavaScript
- **Stockage :** En mémoire (pas de base de données)
- **Authentification :** Sessions ASP.NET Core
- **Design :** Bootstrap 5 + Font Awesome + CSS personnalisé

### **Architecture SOLID**
Le projet respecte les principes SOLID avec :
- **Séparation des responsabilités** : Controllers → Services → Models
- **Injection de dépendances** configurée dans Program.cs
- **Interfaces** pour tous les services métier
- **Tag Helpers personnalisés** pour la réutilisabilité

### **Structure du Projet**
```
ECommerceApp/
├── Controllers/           # Contrôleurs MVC
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── BooksController.cs
│   ├── CartController.cs
│   ├── OrdersController.cs
│   ├── AdminController.cs
│   └── AuthorsController.cs
├── Models/               # Modèles et ViewModels
│   ├── Book.cs
│   ├── User.cs
│   ├── Order.cs
│   ├── Cart.cs
│   ├── Author.cs
│   ├── Enums/
│   └── ViewModels/
├── Services/             # Services métier
│   ├── Interfaces/
│   └── Implementations/
├── TagHelpers/           # Tag Helpers personnalisés
├── Views/               # Vues Razor
└── wwwroot/             # Assets statiques
```

---

## 👥 **Système de Rôles et Permissions**

### **🟢 User (Utilisateur Standard)**
**Capacités :**
- Parcourir le catalogue de livres
- Rechercher et filtrer les livres par titre, auteur, catégorie, prix
- Ajouter des livres au panier
- Passer des commandes (avec système de crédits)
- Consulter l'historique des commandes personnelles
- Gérer son profil utilisateur
- Demander une promotion au rôle Librarian

**Crédits par défaut :** 100€

### **🟡 Librarian (Libraire)**
**Capacités :** Toutes celles de User +
- Ajouter de nouveaux livres au catalogue
- Modifier les informations des livres existants
- Supprimer des livres du catalogue
- Gérer les auteurs (CRUD complet)
- Voir toutes les commandes de la boutique
- Changer le statut des commandes

**Crédits par défaut :** 500€

### **🔴 Admin (Administrateur)**
**Capacités :** Toutes celles de Librarian +
- Gérer les rôles des utilisateurs
- Approuver/rejeter les demandes de promotion Librarian
- Modifier les crédits des utilisateurs
- Activer/désactiver des comptes utilisateurs
- Accès au tableau de bord administrateur complet
- Voir les statistiques détaillées de la boutique

**Crédits par défaut :** 1000€

---

## 🛒 **Fonctionnalités Détaillées**

### **Catalogue et Recherche**
- **Affichage en grille** responsive avec cartes élégantes
- **Recherche textuelle** dans titre, auteur, description
- **Filtres multiples :** catégorie, prix minimum/maximum, disponibilité
- **Tri personnalisable :** titre, prix, date de publication, auteur
- **Pagination** pour une navigation fluide

### **Système de Panier**
- **Ajout temps réel** avec feedback visuel
- **Mise à jour des quantités** avec validation de stock
- **Calcul automatique** des totaux et sous-totaux
- **Validation des stocks** avant commande
- **Persistance en session** 

### **Processus de Commande**
- **Vérification des crédits** en temps réel
- **Confirmation de commande** avec récapitulatif
- **Déduction automatique** des crédits et stocks
- **Historique complet** avec statuts de suivi
- **Système d'annulation** avec remboursement

### **Interface d'Administration**
- **Tableau de bord** avec statistiques en temps réel
- **Gestion des utilisateurs** avec modification des rôles
- **Système de crédits** avec ajustement manuel
- **Validation des demandes** de promotion Librarian
- **Vue d'ensemble** des performances de la boutique

---

## 🎨 **Tag Helpers Personnalisés**

### **BookCardTagHelper**
```html
<book-card book="@book" 
           show-manage-buttons="true" 
           show-add-to-cart="true">
</book-card>
```
Affiche une carte livre stylisée avec actions contextuelles selon le rôle.

### **RoleBasedTagHelper**
```html
<div asp-role="@UserRole.Librarian">
    Contenu visible uniquement aux Libraires+
</div>
```
Affichage conditionnel basé sur les rôles utilisateur.

### **ConfirmDeleteTagHelper**
```html
<confirm-delete item-name="ce livre" 
                delete-url="/Books/Delete/1" 
                button-class="btn btn-danger">
</confirm-delete>
```
Modal de confirmation élégant pour les suppressions.

### **CreditDisplayTagHelper**
```html
<credit-display amount="@user.Credits" 
                size="large" 
                show-icon="true">
</credit-display>
```
Affichage formaté des crédits avec icône.

### **Autres Tag Helpers**
- **OrderStatusTagHelper** : Badges colorés pour les statuts de commandes
- **RoleBadgeTagHelper** : Badges pour les rôles utilisateur
- **StockIndicatorTagHelper** : Indicateurs visuels de stock
- **CategoryBadgeTagHelper** : Badges colorés pour les catégories
- **PriceFormatterTagHelper** : Formatage des prix avec devise

---

## 📊 **Modèles de Données**

### **Entités Principales**
```csharp
User        : Gestion des utilisateurs et authentification
Book        : Catalogue des livres avec métadonnées complètes
Author      : Informations sur les auteurs
Order       : Commandes avec items et statuts
Cart        : Panier d'achat en session
```

### **Types de Données Utilisés** (Conformément aux exigences)
- **String** : Titres, noms, descriptions, emails
- **DateTime** : Dates de publication, commandes, création comptes
- **Enum** : Catégories de livres, rôles utilisateur, statuts commandes
- **Decimal** : Prix des livres, crédits utilisateur
- **Integer** : Quantités, stocks, identifiants
- **Boolean** : Disponibilité, activation comptes

---

## 🚀 **Installation et Lancement**

### **Prérequis**
- Visual Studio 2022 ou VS Code
- .NET 8.0 SDK
- Navigateur web moderne

### **Étapes d'installation**
1. **Cloner le projet**
   ```bash
   git clone [URL_DU_REPOSITORY]
   cd ECommerceApp
   ```

2. **Restaurer les packages**
   ```bash
   dotnet restore
   ```

3. **Lancer l'application**
   ```bash
   dotnet run
   ```

4. **Accéder à l'application**
   - URL : `https://localhost:7092` ou `http://localhost:5017`
   - L'application se lance avec des données de test pré-chargées

---

## 🧪 **Comptes de Test**

### **Comptes Pré-configurés**
| Rôle | Email | Mot de passe | Crédits |
|------|-------|--------------|---------|
| **Admin** | admin@bookstore.com | password | 1000€ |
| **Librarian** | librarian@bookstore.com | password | 500€ |
| **User** | user@bookstore.com | password | 100€ |

### **Données de Test Incluses**
- **8 livres** de différentes catégories avec auteurs associés
- **8 auteurs** avec biographies complètes
- **Stocks variés** pour tester la gestion d'inventaire
- **Prix diversifiés** entre 12€ et 45€

---

## 🔒 **Sécurité Implémentée**

### **Authentification et Autorisation**
- **Hashage des mots de passe** avec salt personnalisé
- **Sessions sécurisées** pour maintenir l'authentification
- **Autorisation basée sur les rôles** à tous les niveaux
- **Protection CSRF** sur toutes les actions sensibles

### **Validation des Données**
- **Validation côté serveur** avec Data Annotations
- **Validation côté client** avec JavaScript et Bootstrap
- **Sanitisation des entrées** pour prévenir les injections
- **Gestion des erreurs** robuste avec messages utilisateur

### **Bonnes Pratiques**
- **Principle of Least Privilege** : accès minimal requis par rôle
- **Confirmation obligatoire** pour toutes les actions destructives
- **Messages d'erreur informatifs** sans exposition de données sensibles
- **Logs d'activité** pour traçabilité (en développement)

---

## 🎨 **Interface Utilisateur**

### **Design Moderne**
- **Bootstrap 5** avec thème personnalisé
- **Font Awesome** pour les icônes
- **Animations CSS** fluides et élégantes
- **Responsive design** pour tous les appareils

### **Expérience Utilisateur**
- **Navigation intuitive** avec breadcrumbs contextuels
- **Feedback visuel immédiat** pour toutes les actions
- **Messages toast** non-intrusifs
- **Loading states** pour les opérations asynchrones
- **Confirmations utilisateur** pour les actions importantes

### **Accessibilité**
- **Contrastes respectés** pour la lisibilité
- **Labels sémantiques** pour les lecteurs d'écran
- **Navigation au clavier** supportée
- **Textes alternatifs** sur toutes les images

---

## 🔧 **Fonctionnalités Techniques Avancées**

### **Tag Helpers Personnalisés**
- **7 Tag Helpers** développés selon les exigences
- **Réutilisabilité** maximale avec paramètres configurables
- **Intégration seamless** avec Razor syntax
- **Performance optimisée** avec rendu conditionnel

### **JavaScript Interactif**
- **AJAX** pour les opérations de panier
- **Validation en temps réel** des formulaires
- **Animations** et transitions fluides
- **Gestion d'erreurs** robuste côté client
- **Loading states** pour améliorer l'UX

### **Architecture Services**
- **Dependency Injection** pour tous les services
- **Interfaces clairement définies** pour la testabilité
- **Séparation des responsabilités** stricte
- **Gestion d'erreurs** centralisée

---

## 📝 **Workflows Utilisateur**

### **Parcours Client Standard**
1. **Inscription** → Compte User + 100€ crédits
2. **Navigation** → Parcours du catalogue avec filtres
3. **Sélection** → Ajout au panier avec validation temps réel
4. **Commande** → Checkout avec vérification crédits
5. **Suivi** → Consultation historique et statuts

### **Workflow Promotion Librarian**
1. **Demande** → Utilisateur fait une demande motivée
2. **Évaluation** → Admin examine la demande
3. **Décision** → Approbation/rejet avec notification
4. **Activation** → Nouveaux droits disponibles immédiatement

### **Gestion Administrative**
1. **Monitoring** → Tableau de bord avec métriques temps réel
2. **Gestion users** → Modification rôles et crédits
3. **Gestion catalogue** → CRUD complet livres/auteurs
4. **Analytics** → Statistiques détaillées et reporting

---

## 🧪 **Tests et Validation**

### **Scénarios de Test Recommandés**

#### **Test User**
1. Créer un nouveau compte utilisateur
2. Parcourir le catalogue et utiliser les filtres
3. Ajouter plusieurs livres au panier
4. Modifier les quantités dans le panier
5. Passer une commande et vérifier la déduction des crédits
6. Consulter l'historique des commandes

#### **Test Librarian**
1. Se connecter avec le compte libraire
2. Ajouter un nouvel auteur
3. Créer un nouveau livre avec cet auteur
4. Modifier un livre existant
5. Consulter toutes les commandes
6. Changer le statut d'une commande

#### **Test Admin**
1. Se connecter avec le compte admin
2. Consulter le tableau de bord
3. Modifier les crédits d'un utilisateur
4. Approuver une demande de promotion libraire
5. Consulter les statistiques détaillées
6. Désactiver/réactiver un compte utilisateur

### **Validation des Exigences du Projet**
- ✅ **CRUD complet** : Create, Read, Update, Delete sur Books et Authors
- ✅ **Types de données variés** : String, Date, Enum, Decimal, Integer, Boolean
- ✅ **Tag Helpers** : 7 Tag Helpers personnalisés développés
- ✅ **Confirmation suppression** : Modal de confirmation pour toutes les suppressions
- ✅ **Layout Bootstrap** : Design moderne et responsive
- ✅ **Pas de base de données** : Stockage en mémoire uniquement

---

## 📱 **Pages et Fonctionnalités**

### **Pages Publiques**
- **Accueil** : Présentation + livres à la une
- **Catalogue** : Grille complète avec recherche/filtres
- **Détails livre** : Informations complètes + ajout panier
- **Connexion/Inscription** : Authentification sécurisée

### **Espace Utilisateur**
- **Mon Panier** : Gestion interactive des articles
- **Mes Commandes** : Historique avec détails et tracking
- **Mon Profil** : Gestion informations personnelles
- **Demande Librarian** : Formulaire de promotion

### **Interface Librarian** (hérite de User +)
- **Gestion Livres** : CRUD complet avec validation
- **Gestion Auteurs** : Ajout/modification avec biographies
- **Toutes Commandes** : Vue globale avec gestion statuts

### **Panel Admin** (hérite de Librarian +)
- **Tableau de bord** : Métriques et KPIs en temps réel
- **Gestion Utilisateurs** : CRUD users avec modification rôles
- **Gestion Crédits** : Ajustement manuel des soldes
- **Demandes Librarian** : Processus d'approbation
- **Statistiques** : Analytics détaillées avec graphiques

---

## 💳 **Système de Crédits**

### **Fonctionnement**
- **Attribution automatique** : 100€ à l'inscription
- **Déduction automatique** lors des achats
- **Validation temps réel** : vérification avant commande
- **Remboursement automatique** en cas d'annulation
- **Gestion admin** : ajustement manuel possible

### **Utilisations des Crédits**
- **Simulation d'achats** réaliste sans vraie monnaie
- **Tests complets** du processus e-commerce
- **Gestion de l'inventaire** avec déduction stocks
- **Workflow complet** commande → paiement → livraison

---

## 🔧 **Configuration et Personnalisation**

### **Settings Application**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### **Sessions Configuration**
- **Durée :** 2 heures d'inactivité
- **Cookies sécurisés** : HttpOnly activé
- **Protection CSRF** : Tokens automatiques

---

## 🐛 **Problèmes Connus et Limitations**

### **Limitations Actuelles**
- **Stockage temporaire** : Données perdues au redémarrage
- **Upload d'images** : URLs externes uniquement (pas d'upload local)
- **Emails** : Pas d'envoi réel d'emails de notification
- **Paiement** : Système de crédits uniquement (pas de vraie passerelle)

### **Améliorations Possibles**
- **Base de données** : Migration vers SQL Server/PostgreSQL
- **Authentification externe** : Google/Facebook login
- **Notifications** : Email/SMS pour confirmations
- **Reporting** : Export PDF des commandes/statistiques
- **API REST** : Endpoints pour applications mobiles

---

## 📚 **Ressources et Documentation**

### **Technologies Utilisées**
- [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/)
- [Bootstrap 5](https://getbootstrap.com/)
- [Font Awesome](https://fontawesome.com/)
- [Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/)

### **Patterns Implémentés**
- **MVC (Model-View-Controller)** : Séparation des responsabilités
- **Repository Pattern** : Via les Services
- **Dependency Injection** : IoC Container ASP.NET Core
- **ViewModels** : Séparation Models métier/présentation

---

## 🎯 **Objectifs Pédagogiques Atteints**

- ✅ **Développement web moderne** avec ASP.NET Core MVC
- ✅ **Architecture propre** respectant les principes SOLID
- ✅ **Sécurité applicative** avec authentification/autorisation
- ✅ **Interface utilisateur riche** avec Tag Helpers personnalisés
- ✅ **Gestion d'état** complexe avec sessions et services
- ✅ **Validation robuste** côté client et serveur
- ✅ **Design responsive** avec Bootstrap 5
- ✅ **JavaScript moderne** pour l'interactivité

---

## 📄 **Livrables du Projet**

1. **Code source complet** (.zip) avec tous les fichiers du projet
2. **Documentation technique** (ce README)
3. **Rapport projet** avec captures d'écran et explications
4. **Repository GitHub** avec historique de développement
5. **Guide utilisateur** pour chaque rôle

---

## 👨‍🏫 **Remerciements**

Projet réalisé sous la supervision de **Prof. Marcel Stefan Wagner, PhD** dans le cadre du cours "Retail (e-commerce)" à l'**ESTIAM**.

Merci pour l'encadrement technique et les conseils méthodologiques qui ont permis la réalisation de cette application complète et professionnelle.

---

## 📞 **Support et Contact**

Pour toute question technique ou demande d'amélioration :
- **Email étudiant** : [votre.email@estiam.com]
- **Repository GitHub** : [URL_DU_REPOSITORY]
- **Institution** : ESTIAM - École Supérieure des Technologies de l'Information Appliquées au Management

---

*Développé avec ❤️ par les étudiants ESTIAM sous la direction de Prof. Marcel Stefan Wagner, PhD*