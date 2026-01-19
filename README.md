# Company Manager - System Zarządzania Pracownikami

Kompleksowy system do zarządzania zasobami ludzkimi (HRMS) zbudowany w architekturze ASP.NET MVC.

## 🚀 Instalacja i Uruchomienie

### Wymagania systemowe
- .NET 8.0 SDK lub nowszy
- SQL Server (LocalDB w wersji mssqllocaldb)
- Visual Studio 2022 lub VS Code

### Kroki instalacji
1. Sklonuj repozytorium: `git clone https://github.com/Slawuu/Company_manager.git`
2. Otwórz projekt w Visual Studio lub folder w VS Code.
3. Przywróć pakiety NuGet: `dotnet restore`
4. Uruchom migracje bazy danych (jeśli nie zostały wykonane automatycznie): `dotnet ef database update`
5. Uruchom aplikację: `dotnet run` (lub F5 w Visual Studio).

## ⚙️ Konfiguracja

### Connection String
Połączenie z bazą danych skonfigurowane jest w pliku `appsettings.json`. Domyślnie korzysta z lokalnej bazy Microsoft SQL Server:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProjektDB;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### Testowi Użytkownicy
Aplikacja posiada system automatycznego seedowania danych. Przy pierwszym uruchomieniu tworzone są role oraz konto administratora:
- **Login:** `admin@hrms.com`
- **Hasło:** `admin123`

Inne dostępne role w systemie: `HR`, `Manager`, `Employee`.

## 🛠 Opis Działania Aplikacji

### Perspektywa Administratora / HR
- **Zarządzanie Pracownikami:** Pełny CRUD (dodawanie nowych osób, edycja danych, przypisywanie ról).
- **Struktura Organizacyjna:** Tworzenie i edycja działów (Departments).
- **Projekty:** Przypisywanie pracowników do projektów (relacja wiele-do-wielu).
- **Zatwierdzanie urlopów:** Przeglądanie wniosków wysłanych przez pracowników i ich akceptacja/odrzucenie.

### Perspektywa Pracownika (Użytkownik Zwykły)
- **Profil:** Podgląd własnych danych i edycja podstawowych informacji.
- **Lista Pracowników:** Przeglądanie współpracowników w obrębie danego działu.
- **Wnioski urlopowe:** Składanie wniosków o urlop (status: oczekujący).
- **Projekty:** Podgląd przypisanych projektów.

## 📡 API CRUD
System udostępnia endpointy API dla encji Employee pod adresem: 
`https://localhost:[port]/api/employees`

Obsługiwane metody:
- `GET` - Lista pracowników (wymaga logowania)
- `POST` - Dodanie nowego pracownika
- `PUT` - Edycja danych
- `DELETE` - Usuwanie pracownika

---
*Projekt przygotowany w ramach zaliczenia przedmiotu Programowanie baz danych / ASP.NET MVC.*
