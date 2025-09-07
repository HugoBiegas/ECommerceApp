# 📚 BookStore - Application E-Commerce ASP.NET Core MVC

<!-- Badges du projet -->
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-purple?style=for-the-badge&logo=csharp)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-blueviolet?style=for-the-badge&logo=bootstrap)
![License](https://img.shields.io/badge/License-Academic-green?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Completed-success?style=for-the-badge)

<!-- Badges académiques -->
![ESTIAM](https://img.shields.io/badge/ESTIAM-2025-orange?style=flat-square&logo=graduation-cap)
![Course](https://img.shields.io/badge/Course-Retail%20E--commerce-lightblue?style=flat-square)
![Professor](https://img.shields.io/badge/Professor-Marcel%20Stefan%20Wagner%2C%20PhD-gold?style=flat-square)

---

## 📊 **RAPPORT DE PROJET**

> **📑 [Consultez le rapport complet en PDF](./Rapport_BookStore_ESTIAM.pdf)**
> 
> Le rapport détaillé contient :
> - 📸 **Captures d'écran** de toutes les fonctionnalités
> - 🔧 **Explications techniques** détaillées
> - 🏗️ **Architecture SOLID** implémentée
> - 🎨 **Tag Helpers personnalisés** (7 développés)
> - 👥 **Système de rôles** et sécurité
> - 🧪 **Tests et validation** complets

---

## 🎓 **Contexte Académique**
Projet développé dans le cadre du cours **"Retail (e-commerce)"** dispensé par **Prof. Marcel Stefan Wagner, PhD** à l'**ESTIAM**.

**Équipe de développement :**
- **Hugo Biegas** - Développeur Full-Stack
- **Naier Abassi** - Développeur Backend/Security
- **Yanne Iziglouch** - Développeur Frontend/UI

**Encadrement académique :** Prof. Marcel Stefan Wagner, PhD

---

## 🎯 **Description du Projet**

BookStore est une application web e-commerce sophistiquée de vente de livres en ligne, développée avec ASP.NET Core MVC 8.0. L'application implémente une architecture robuste respectant les principes SOLID, un système d'authentification multi-niveaux, et un ensemble complet de fonctionnalités e-commerce avec un système de crédits virtuels pour la démonstration.

### **Vision Produit**
Créer une plateforme e-commerce moderne et sécurisée permettant la gestion complète d'une librairie en ligne, de la consultation du catalogue à la finalisation des commandes, en passant par l'administration des contenus et des utilisateurs.

---

## ✨ **Fonctionnalités Principales**

### **🛒 Système E-Commerce Complet**
- **Catalogue interactif** : Navigation fluide avec recherche multi-critères
- **Panier temps réel** : Gestion dynamique avec validation instantanée
- **Processus de commande** : Workflow complet avec gestion des stocks
- **Historique des achats** : Suivi détaillé pour chaque utilisateur

### **👥 Authentification et Autorisation**
- **Système de rôles hiérarchique** : User → Librarian → Admin
- **Sessions sécurisées** : Protection CSRF et gestion d'état robuste
- **Demandes de promotion** : Workflow d'approbation Librarian
- **Gestion des permissions** : Contrôle d'accès granulaire

### **💳 Système de Crédits Virtuels**
- **Attribution automatique** : 100€ pour les nouveaux utilisateurs
- **Transactions sécurisées** : Validation temps réel et rollback automatique
- **Gestion administrative** : Ajustement manuel des soldes
- **Historique complet** : Traçabilité de toutes les opérations

### **🔧 Administration Avancée**
- **Dashboard analytics** : Métriques temps réel avec KPIs
- **Gestion utilisateurs** : CRUD complet avec modification des rôles
- **Gestion catalogue** : Interface d'administration pour livres/auteurs
- **Reporting** : Statistiques détaillées des ventes et utilisateurs

---

## 🏗️ **Architecture Technique**

### **Stack Technologique**
```
• Backend : ASP.NET Core MVC 8.0
• Frontend : Razor Views + Bootstrap 5 + JavaScript ES6+
• Authentification : Sessions ASP.NET Core sécurisées
• Stockage : En mémoire (conforme aux exigences du projet)
• Design System : Bootstrap 5 + Font Awesome + CSS personnalisé
• Security : Protection CSRF, validation robuste, sanitisation
```

### **Architecture SOLID Implémentée**

#### **Single Responsibility Principle (SRP)**
Chaque classe a une responsabilité unique :
- `Controllers` : Gestion des requêtes HTTP uniquement
- `Services` : Logique métier isolée
- `Models` : Représentation des données
- `ViewModels` : Données optimisées pour l'affichage
- `TagHelpers` : Composants UI réutilisables

#### **Open/Closed Principle (OCP)**
- **BaseController** : Extensible pour nouvelles fonctionnalités
- **Tag Helpers** : Modulaires et composables
- **Services** : Interfaces permettant l'extension sans modification

#### **Liskov Substitution Principle (LSP)**
- **Hiérarchie des rôles** : User → Librarian → Admin respecte la substitution
- **Interfaces de services** : Implémentations interchangeables

#### **Interface Segregation Principle (ISP)**
- **Services spécialisés** : `IBookService`, `IAuthService`, `ICartService`, etc.
- **Interfaces ciblées** : Pas de dépendances superflues

#### **Dependency Inversion Principle (DIP)**
- **Injection de dépendances** : Configuration dans `Program.cs`
- **Abstractions** : Controllers dépendent des interfaces, pas des implémentations

### **Structure du Projet**
```
ECommerceApp/
├── Controllers/              # Contrôleurs MVC (7 contrôleurs)
│   ├── BaseController.cs     # Contrôleur de base avec fonctionnalités communes
│   ├── HomeController.cs     # Page d'accueil et navigation
│   ├── AccountController.cs  # Authentification et gestion de compte
│   ├── BooksController.cs    # CRUD livres et catalogue
│   ├── AuthorsController.cs  # CRUD auteurs
│   ├── CartController.cs     # Gestion panier et checkout
│   ├── OrdersController.cs   # Gestion commandes et historique
│   └── AdminController.cs    # Panel d'administration
├── Models/                   # Modèles et structures de données
│   ├── Book.cs              # Entité livre avec métadonnées complètes
│   ├── User.cs              # Utilisateur avec rôles et crédits
│   ├── Author.cs            # Auteur avec biographie
│   ├── Order.cs             # Commande avec items et statuts
│   ├── Cart.cs              # Panier en session
│   ├── LibrarianRequest.cs  # Demandes de promotion
│   ├── Enums/               # Énumérations métier
│   │   ├── UserRole.cs      # Rôles utilisateur
│   │   ├── BookCategory.cs  # Catégories de livres
│   │   └── OrderStatus.cs   # Statuts de commande
│   └── ViewModels/          # ViewModels spécialisés
│       ├── BookListViewModel.cs
│       ├── BookCreateEditViewModel.cs
│       ├── DashboardViewModel.cs
│       ├── UserManagementViewModel.cs
│       ├── CheckoutViewModel.cs
│       ├── LoginViewModel.cs
│       └── RegisterViewModel.cs
├── Services/                # Services métier avec interfaces
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IBookService.cs
│   │   ├── IAuthorService.cs
│   │   ├── ICartService.cs
│   │   ├── IOrderService.cs
│   │   ├── IUserService.cs
│   │   └── ILibrarianRequestService.cs
│   └── Implementations/     # Implémentations concrètes
├── TagHelpers/              # 7 Tag Helpers personnalisés
│   ├── BookCardTagHelper.cs         # Cartes livres stylisées
│   ├── RoleBasedTagHelper.cs        # Affichage conditionnel par rôle
│   ├── ConfirmDeleteTagHelper.cs    # Modals de confirmation
│   ├── CreditDisplayTagHelper.cs    # Affichage formaté des crédits
│   ├── OrderStatusTagHelper.cs      # Badges de statut commande
│   ├── RoleBadgeTagHelper.cs        # Badges de rôles utilisateur
│   ├── StockIndicatorTagHelper.cs   # Indicateurs de stock
│   ├── CategoryBadgeTagHelper.cs    # Badges de catégories
│   └── PriceFormatterTagHelper.cs   # Formatage des prix
├── Views/                   # Vues Razor avec layout Bootstrap
│   ├── Shared/
│   │   ├── _Layout.cshtml           # Layout principal responsive
│   │   ├── _LoginPartial.cshtml     # Partial de connexion
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Home/                # Pages publiques
│   ├── Account/             # Authentification
│   ├── Books/               # Gestion catalogue
│   ├── Authors/             # Gestion auteurs
│   ├── Cart/                # Panier
│   ├── Orders/              # Commandes
│   └── Admin/               # Administration
└── wwwroot/                 # Ressources statiques
    ├── css/                 # Styles personnalisés
    ├── js/                  # JavaScript ES6+
    ├── lib/                 # Librairies (Bootstrap, jQuery)
    └── images/              # Assets images
```

---

## 🎨 **Tag Helpers Personnalisés (7 Requis)**

### **1. BookCardTagHelper**
```html
<book-card book="@book" 
           show-manage-buttons="true" 
           show-add-to-cart="true">
</book-card>
```
**Fonctionnalité :** Génère une carte livre stylisée avec actions contextuelles selon le rôle utilisateur.

### **2. RoleBasedTagHelper**
```html
<div asp-role="@UserRole.Librarian">
    Contenu visible uniquement aux Libraires et Admins
</div>
```
**Fonctionnalité :** Affichage conditionnel basé sur la hiérarchie des rôles.

### **3. ConfirmDeleteTagHelper**
```html
<confirm-delete item-name="ce livre" 
                delete-url="/Books/Delete/1" 
                button-class="btn btn-danger">
</confirm-delete>
```
**Fonctionnalité :** Modal Bootstrap de confirmation avec protection contre les suppressions accidentelles.

### **4. CreditDisplayTagHelper**
```html
<credit-display amount="@user.Credits" 
                size="large" 
                show-icon="true">
</credit-display>
```
**Fonctionnalité :** Affichage formaté des crédits avec icône Euro et couleurs conditionnelles.

### **5. OrderStatusTagHelper**
```html
<order-status status="@order.Status"></order-status>
```
**Fonctionnalité :** Badges Bootstrap colorés et animés selon le statut de commande.

### **6. RoleBadgeTagHelper**
```html
<role-badge role="@user.Role"></role-badge>
```
**Fonctionnalité :** Badges avec icônes Font Awesome pour identifier visuellement les rôles.

### **7. CategoryBadgeTagHelper**
```html
<category-badge category="@book.Category"></category-badge>
```
**Fonctionnalité :** Badges colorés thématiques pour les catégories de livres.

---

## 📊 **Modèles de Données et Types**

### **Conformité aux Exigences (Types de Données Variés)**

#### **Entité Book**
```csharp
public class Book
{
    public int Id { get; set; }                    // Integer
    public string Title { get; set; }              // String
    public string? ISBN { get; set; }              // String nullable
    public BookCategory Category { get; set; }     // Enum
    public decimal Price { get; set; }             // Decimal
    public DateTime? PublicationDate { get; set; } // DateTime nullable
    public bool IsAvailable { get; set; }          // Boolean
    public string? Description { get; set; }       // String
    public int Stock { get; set; }                 // Integer
    public string? ImageUrl { get; set; }          // String
    public int AuthorId { get; set; }              // Integer (FK)
}
```

#### **Entité User**
```csharp
public class User
{
    public int Id { get; set; }                    // Integer
    public string Username { get; set; }           // String
    public string Email { get; set; }              // String
    public string PasswordHash { get; set; }       // String
    public UserRole Role { get; set; }             // Enum
    public decimal Credits { get; set; }           // Decimal
    public DateTime CreatedAt { get; set; }        // DateTime
    public bool IsActive { get; set; }             // Boolean
    public string? FirstName { get; set; }         // String nullable
    public string? LastName { get; set; }          // String nullable
}
```

#### **Énumérations Métier**
```csharp
public enum UserRole { User = 1, Librarian = 2, Admin = 3 }
public enum BookCategory { Fiction, NonFiction, Science, History, Biography, /* ... */ }
public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }
```

---

## 👥 **Système de Rôles et Permissions**

### **🟢 User (Utilisateur Standard)**
**Permissions :**
- ✅ Consultation du catalogue avec recherche/filtres
- ✅ Gestion du panier personnel
- ✅ Passage de commandes avec système de crédits
- ✅ Consultation de l'historique personnel
- ✅ Demande de promotion Librarian
- ✅ Gestion du profil utilisateur

**Crédits par défaut :** 100€

### **🟡 Librarian (Libraire)**
**Permissions :** Toutes celles de User +
- ✅ CRUD complet sur les livres
- ✅ CRUD complet sur les auteurs
- ✅ Consultation de toutes les commandes
- ✅ Modification des statuts de commande
- ✅ Accès aux statistiques de vente

**Crédits par défaut :** 500€

### **🔴 Admin (Administrateur)**
**Permissions :** Toutes celles de Librarian +
- ✅ Gestion complète des utilisateurs
- ✅ Modification des rôles et crédits
- ✅ Approbation des demandes Librarian
- ✅ Accès au dashboard analytics complet
- ✅ Activation/désactivation des comptes
- ✅ Gestion des statistiques globales

**Crédits par défaut :** 1000€

---

## 🔒 **Sécurité et Validation**

### **Authentification Robuste**
- **Hashage des mots de passe** avec salt personnalisé
- **Sessions sécurisées** avec timeout automatique
- **Protection CSRF** sur toutes les actions sensibles
- **Validation de l'autorisation** à chaque niveau

### **Validation Multi-Niveaux**
```csharp
[Required(ErrorMessage = "Le titre est requis")]
[StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
public string Title { get; set; }

[Range(0.01, 999.99, ErrorMessage = "Le prix doit être entre 0.01€ et 999.99€")]
public decimal Price { get; set; }
```

### **Protection des Actions Sensibles**
- **Confirmation de suppression** obligatoire (conforme aux exigences)
- **Validation des stocks** avant commande
- **Vérification des crédits** en temps réel
- **Audit trail** des opérations administratives

---

## 🚀 **Installation et Configuration**

### **Prérequis Système**
- **Visual Studio 2022** ou VS Code avec extensions C#
- **.NET 8.0 SDK** ou version ultérieure
- **Navigateur moderne** (Chrome, Firefox, Edge, Safari)

### **Démarrage Rapide**
```bash
# 1. Cloner le repository
git clone [URL_DU_REPOSITORY]
cd ECommerceApp

# 2. Restaurer les dépendances
dotnet restore

# 3. Compiler le projet
dotnet build

# 4. Lancer l'application
dotnet run

# 5. Accéder à l'application
# https://localhost:7092
# http://localhost:5017
```

---

## 🧪 **Comptes de Test et Données**

### **Comptes Pré-configurés**
| Rôle | Email | Mot de passe | Crédits | Permissions |
|------|-------|--------------|---------|-------------|
| **Admin** | admin@bookstore.com | password | 1000€ | Toutes |
| **Librarian** | librarian@bookstore.com | password | 500€ | Gestion catalogue |
| **User** | user@bookstore.com | password | 100€ | E-commerce |

### **Jeu de Données de Test**
- **📚 8 livres** de catégories variées avec métadonnées complètes
- **✍️ 8 auteurs** avec biographies détaillées
- **🏷️ Stocks diversifiés** pour tester la gestion d'inventaire
- **💰 Prix échelonnés** de 12€ à 45€
- **📦 Commandes types** avec différents statuts

---

## 🧪 **Scénarios de Test Fonctionnels**

### **Tests Utilisateur Standard**
```
1. ✅ Inscription → Vérification des 100€ de crédits
2. ✅ Recherche catalogue → Filtres par catégorie/prix/auteur
3. ✅ Ajout panier → Validation stocks en temps réel
4. ✅ Modification quantités → Recalcul automatique
5. ✅ Checkout → Déduction crédits et stocks
6. ✅ Historique → Consultation commandes avec statuts
```

### **Tests Librarian**
```
1. ✅ Promotion depuis User → Nouvelles permissions
2. ✅ CRUD Auteur → Ajout avec biographie complète
3. ✅ CRUD Livre → Association auteur, catégorie, stock
4. ✅ Gestion commandes → Modification statuts
5. ✅ Validation suppressions → Modals de confirmation
```

### **Tests Admin**
```
1. ✅ Dashboard analytics → Métriques temps réel
2. ✅ Gestion utilisateurs → Modification rôles/crédits
3. ✅ Approbation demandes → Workflow Librarian
4. ✅ Statistiques avancées → Reporting complet
5. ✅ Activation/désactivation → Contrôle accès
```

---

## 📈 **Fonctionnalités Avancées**

### **Analytics et Reporting**
- **Dashboard temps réel** avec métriques KPI
- **Statistiques des ventes** par période/catégorie
- **Analyses utilisateurs** et comportements d'achat
- **Reporting automatisé** des performances

### **Gestion d'Inventaire**
- **Suivi stocks en temps réel** avec alertes
- **Validation automatique** avant commande
- **Gestion des ruptures** de stock
- **Historique des mouvements** de stock

### **Système de Crédits Avancé**
- **Transactions atomiques** avec rollback
- **Historique complet** des opérations
- **Validation multi-niveaux** avant déduction
- **Gestion administrative** fine des soldes

---

## 🎯 **Respect des Exigences ESTIAM**

### **✅ Exigences Techniques Validées**
- **CRUD Complet** : Create, Read, Update, Delete sur Books et Authors
- **Types de Données Variés** : String, DateTime, Enum, Decimal, Integer, Boolean
- **7 Tag Helpers Personnalisés** : Développés selon spécifications
- **Confirmation Suppressions** : Modals Bootstrap sur toutes les suppressions
- **Layout Bootstrap Responsive** : Design moderne et adaptatif
- **Pas de Base de Données** : Stockage en mémoire uniquement
- **Recherche Multi-Critères** : Recherche par propriétés du modèle
- **Architecture MVC** : Séparation stricte des responsabilités

### **✅ Exigences Fonctionnelles Validées**
- **Application Web Complète** : E-commerce fonctionnel de A à Z
- **Interface Utilisateur Riche** : Bootstrap 5 + JavaScript interactif
- **Système d'Authentification** : Multi-rôles avec sessions sécurisées
- **Gestion de Contenu** : CRUD administrateur avec permissions
- **Workflow E-Commerce** : Du catalogue au checkout complet

---

## 🛠️ **Technologies et Patterns Implémentés**

### **Design Patterns**
- **MVC (Model-View-Controller)** : Architecture principale
- **Repository Pattern** : Via les Services avec interfaces
- **Dependency Injection** : IoC Container ASP.NET Core
- **ViewModels Pattern** : Séparation modèles métier/présentation
- **Factory Pattern** : Pour la création des Tag Helpers

### **Bonnes Pratiques**
- **Clean Code** : Nommage explicite, fonctions courtes
- **SOLID Principles** : Respect rigoureux des 5 principes
- **DRY (Don't Repeat Yourself)** : Réutilisation via BaseController et Services
- **Security by Design** : Validation et autorisation à tous les niveaux
- **Responsive Design** : Mobile-first avec Bootstrap 5

---

## 📚 **Documentation et Ressources**

### **Technologies Utilisées**
- [ASP.NET Core MVC 8.0](https://docs.microsoft.com/en-us/aspnet/core/mvc/)
- [Bootstrap 5.3](https://getbootstrap.com/docs/5.3/)
- [Font Awesome 6](https://fontawesome.com/)
- [Razor Views](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor)

### **Références Architecturales**
- [Principes SOLID](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)
- [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)

---

## 🎖️ **Objectifs Pédagogiques Atteints**

- ✅ **Développement Web Moderne** : ASP.NET Core MVC avec C# 12
- ✅ **Architecture Robuste** : Principes SOLID et Clean Architecture
- ✅ **Sécurité Applicative** : Authentification, autorisation, validation
- ✅ **Interface Riche** : Tag Helpers personnalisés et JavaScript interactif
- ✅ **Gestion d'État Complexe** : Sessions, services, et workflow e-commerce
- ✅ **Design Responsive** : Bootstrap 5 avec adaptation mobile
- ✅ **Validation Robuste** : Côté client et serveur avec feedback UX
- ✅ **Séparation des Responsabilités** : MVC strict avec Services découplés

---

## 📄 **Livrables du Projet**

1. **📁 Code Source Complet** (.zip) - Tous les fichiers du projet Visual Studio 2022
2. **📋 Documentation Technique** - Ce README.md détaillé
3. **📊 Rapport de Projet** - Analyse fonctionnelle avec captures d'écran
4. **🗂️ Repository GitHub** - Historique de développement avec commits
5. **📖 Guide Utilisateur** - Manuel d'utilisation par rôle

---

## 🔮 **Évolutions Possibles**

### **Extensions Techniques**
- **Base de Données** : Migration vers SQL Server/PostgreSQL
- **API REST** : Développement d'endpoints pour mobile
- **Authentification OAuth** : Intégration Google/Facebook/Microsoft
- **Notifications** : Système d'emails/SMS pour confirmations
- **Caching** : Implémentation Redis pour les performances

### **Fonctionnalités Métier**
- **Système de Wishlist** : Listes de souhaits utilisateur
- **Recommandations** : IA pour suggestions personnalisées
- **Avis et Notes** : Système de review avec modération
- **Promotions** : Codes de réduction et campagnes marketing
- **Multi-langues** : Internationalisation complète

---

## 🤝 **Contribution et Maintenance**

### **Équipe de Développement**
- **Hugo Biegas** : Architecture, Backend, Sécurité
- **Naier Abassi** : Services, Business Logic, Testing
- **Yanne Iziglouch** : Frontend, UX/UI, Tag Helpers

### **Standards de Code**
- **Conventions C#** : Microsoft Guidelines
- **Architecture** : Clean Architecture + SOLID
- **Tests** : Couverture minimale 80%
- **Documentation** : XML Documentation obligatoire

---
*Développé avec ❤️ par l'équipe ESTIAM sous la supervision de Prof. Marcel Stefan Wagner, PhD - Septembre 2025*