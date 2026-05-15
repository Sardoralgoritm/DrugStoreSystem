# UI Pages — DrugstoreSystem

All pages are Blazor Server components using MudBlazor. UI strings visible to users are in Uzbek. This document defines every page: its route, access level, components, and behavior.

---

## 1. Page Map

```
/ (public)                  — Search page (main page)
/medicine/{id} (public)     — Medicine detail
/pharmacy/{id} (public)     — Pharmacy detail

/auth/login                 — Login (Admin + Pharmacist)

/admin/dashboard            — Admin dashboard
/admin/pharmacies           — Pharmacy list
/admin/pharmacies/create    — Create pharmacy + pharmacist account
/admin/pharmacies/{id}/edit — Edit pharmacy

/pharmacist/dashboard       — Pharmacist dashboard
/pharmacist/profile         — Edit own pharmacy profile
/pharmacist/inventory       — Inventory list
/pharmacist/inventory/add   — Add medicine to inventory
```

---

## 2. Public Pages (no auth)

### 2.1 `/` — Search Page

**The main page of the application.** This is what every guest sees first.

**Components:**
- `MudTextField` — search input (placeholder: "Dori nomini kiriting...")
- Location bar: shows detected location or manual input field + "Mening joylashuvim" button
- Sort toggle: `MudToggleGroup` — "Yaqinlik" | "Narx"
- `MudButton` — "Qidirish"
- Results area:
  - `MudProgressLinear` — while searching
  - One `MudExpansionPanel` per matching medicine (collapsed by default)
    - Header: medicine name + category badge + match indicator
    - Body: `MudTable` of ranked pharmacies (Name | Address | Price | Distance | Map icon)
  - Empty state: "Dori topilmadi. Boshqacha nom bilan qidirib ko'ring."

**Behavior:**
1. On page load: request geolocation via JS interop → populate location bar
2. If geolocation denied: show manual input field
3. User types ≥ 2 chars + clicks search (or presses Enter) → call `ISearchService.SearchAsync`
4. Each result row has a Google Maps icon (`MudIconButton` with `Icons.Material.Filled.Map`) → opens `MapsUrl` in new tab
5. Sort toggle change → re-sort in-memory (no new DB call)
6. Location change → re-run search with new coords

**State machine:**
```
Idle → Searching → Results (or Empty)
                ↑ user changes query/location/sort
```

---

### 2.2 `/medicine/{id}` — Medicine Detail

**Components:**
- Breadcrumb: "Bosh sahifa > [Medicine Name]"
- `MudCard` — medicine info:
  - Name (H1), Generic name, Russian name
  - Dosage form badge, Category chip, Manufacturer
  - Description
  - Synonyms list (if any): "Shuningdek: Panadol, Para-500"
- Section: "Bu dorini qayerdan topish mumkin?"
  - Same ranked pharmacy table as search results (sorted by distance by default)
  - Sort toggle available here too
- If no pharmacy has it: `MudAlert` — "Bu dori hozirda hech bir dorixonada mavjud emas."

---

### 2.3 `/pharmacy/{id}` — Pharmacy Detail

**Components:**
- Breadcrumb: "Bosh sahifa > [Pharmacy Name]"
- `MudCard` — pharmacy info:
  - Name (H1), Address
  - Phone (clickable `tel:` link)
  - Working hours
  - `MudButton` "Xaritada ko'rish" → Google Maps redirect (opens new tab)
- Section: "Mavjud dorilar" — `MudTable` of inventory items (Medicine Name | Price | Quantity)
  - Search within table via `MudTextField` filter
- If inactive: `MudAlert` warning — "Bu dorixona hozirda faol emas."

---

## 3. Auth Page

### 3.1 `/auth/login`

**Access:** Anonymous (redirected here when unauthenticated admin/pharmacist tries to access protected page)

**Components:**
- `MudCard` centered — "Tizimga kirish"
- `MudTextField` — Email
- `MudTextField` (InputType.Password) — Parol
- `MudButton` — "Kirish"
- Error: `MudAlert Severity.Error` — "Email yoki parol noto'g'ri"

**Behavior:** On success → redirect to role-appropriate dashboard (`/admin/dashboard` or `/pharmacist/dashboard`)

---

## 4. Admin Pages (`[Authorize(Roles = "Admin")]`)

### 4.1 `/admin/dashboard`

**Components:**
- 4x `MudCard` stat cards: Total Pharmacies | Active Pharmacies | Total Medicines | Total Inventory Items
- `MudTable` — recent pharmacies (last 5 created)

---

### 4.2 `/admin/pharmacies` — Pharmacy List

**Components:**
- `MudButton` "Yangi dorixona qo'shish" (→ `/admin/pharmacies/create`)
- `MudTextField` — search/filter by name
- `MudTable`:
  - Columns: Name | Address | Phone | Status (Active/Inactive) | Actions
  - Actions: Edit icon | Toggle active switch | Delete (with confirm dialog)
- `MudSwitch` per row — toggles `is_active` via `IPharmacyService.SetActiveAsync`

---

### 4.3 `/admin/pharmacies/create`

**Purpose:** Creates a pharmacy AND its pharmacist login account in one flow.

**Components:**
- `MudStepper` (2 steps):
  - **Step 1: Dorixona ma'lumotlari** — Name, Address, Latitude, Longitude (with helper text: "Google Mapsdan koordinatalarni oling"), Phone, Working Hours
  - **Step 2: Farmatsevt hisobi** — Email, Password, Confirm Password
- `MudButton` "Saqlash" — calls `IAdminService.CreatePharmacistAccountAsync` which creates both Pharmacy + AppUser atomically
- Validation errors shown under each field

---

### 4.4 `/admin/pharmacies/{id}/edit`

**Components:** Same form as Create (Step 1 only — no account fields). Pre-populated with existing data.

---

## 5. Pharmacist Pages (`[Authorize(Roles = "Pharmacist")]`)

### 5.1 `/pharmacist/dashboard`

**Components:**
- Pharmacy name as page title
- 2x stat cards: Total medicines in inventory | Low stock alerts (quantity < 5)
- `MudTable` — top 10 medicines by price (most expensive first — informational)

---

### 5.2 `/pharmacist/profile`

**Components:**
- `MudTextField` for each editable field: Name, Address, Latitude, Longitude, Phone, Working Hours
- `MudAlert Info` — "Koordinatalarni Google Mapsdan olish uchun: Maps → O'ng klik → koordinatlarni nusxalash"
- `MudButton` "Saqlash" — calls `IPharmacyService.UpdateAsync`

---

### 5.3 `/pharmacist/inventory` — Inventory List

**Components:**
- `MudButton` "Dori qo'shish" (→ `/pharmacist/inventory/add`)
- `MudTextField` — in-table filter
- `MudTable`:
  - Columns: Medicine | Dosage Form | Price (UZS) | Quantity | Last Updated | Actions
  - Actions: Edit quantity/price (inline edit) | Remove (with confirm)
- `MudChip` badge for low stock (quantity < 5): "Kam qoldi"

---

### 5.4 `/pharmacist/inventory/add` — Add Medicine

**This page implements the shared-catalog autocomplete-then-create flow.**

**Components:**
- `MudAutocomplete<MedicineAutocompleteDto>` — "Dori nomini qidiring..."
  - Calls `IMedicineService.AutocompleteAsync(query)` on each keystroke (debounced 300ms)
  - Shows: Name / GenericName / DosageForm in list item
  - "Topilmadi — yangi dori yaratish" option at bottom if no results
- **If medicine selected:**
  - Show selected medicine info card (read-only)
  - `MudNumericField` — Narx (UZS)
  - `MudNumericField` — Miqdor (dona)
  - `MudButton` "Inventarga qo'shish"
- **If "create new" chosen:**
  - Expand create-medicine form:
    - Name (required), Generic Name, Russian Name, Dosage Form (`MudSelect`), Category (`MudSelect`), Manufacturer, Description
    - Synonyms: `MudChipSet` + add chip input (user types synonym + Enter to add)
  - Then Price + Quantity fields
  - `MudButton` "Yaratib inventarga qo'shish"

---

## 6. Shared Components

### `NavMenu.razor`
- **Guest**: Logo + "Dori qidirish" link only
- **Pharmacist**: Logo + "Bosh sahifa" + "Inventar" + "Profil" + Logout
- **Admin**: Logo + "Bosh sahifa" + "Dorixonalar" + Logout

### `MainLayout.razor`
- `MudThemeProvider`, `MudDialogProvider`, `MudSnackbarProvider`
- Top `MudAppBar` with role-based nav

### `PharmacyResultRow.razor`
- Reusable row for search results + medicine detail page
- Props: `RankedPharmacyDto Pharmacy`, `bool ShowDistance`

### `EmptyState.razor`
- Reusable empty state with icon + message
- Props: `string Message`, `string? SubMessage`

---

## 7. Error & Empty States

| Scenario | Display |
|---|---|
| Search returns no medicines | EmptyState: "Dori topilmadi. Boshqacha nom bilan qidirib ko'ring." |
| Medicine found, 0 pharmacies | MudAlert Info: "Bu dori hozirda hech bir dorixonada mavjud emas." |
| Geolocation denied | Inline input: "Manzilingizni kiriting" |
| Page not found | MudAlert: "Sahifa topilmadi" |
| Server error | MudAlert Error: "Xatolik yuz berdi. Sahifani yangilang." |

---

## 8. MudBlazor Theme

Primary color: `#1565C0` (blue — medicine/health association)
Secondary color: `#2E7D32` (green)
Error color: `#C62828`

```csharp
// Program.cs
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowCloseIcon = true;
});
```
