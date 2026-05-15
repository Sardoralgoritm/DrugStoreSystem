---
name: blazor-mudblazor-patterns
description: Blazor Server + MudBlazor patterns for DrugstoreSystem. Open before writing any .razor file.
---

# Blazor Server + MudBlazor Patterns — DrugstoreSystem

---

## 1. Page Template

```razor
@page "/example"
@attribute [Authorize(Roles = "Admin")]   @* Remove for public pages *@
@inject IExampleService ExampleService
@inject NavigationManager Nav
@inject ISnackbar Snackbar

<PageTitle>Sahifa nomi | DrugstoreSystem</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large" Class="mt-4">
    <MudText Typo="Typo.h5" Class="mb-4">Sahifa sarlavhasi</MudText>

    @if (_loading)
    {
        <MudProgressLinear Indeterminate Color="Color.Primary" />
    }
    else if (_items.Count == 0)
    {
        <EmptyState Message="Ma'lumot topilmadi" />
    }
    else
    {
        @* page content *@
    }
</MudContainer>

@code {
    private bool _loading = true;
    private List<ExampleDto> _items = [];

    protected override async Task OnInitializedAsync()
    {
        _items = (await ExampleService.GetAllAsync()).ToList();
        _loading = false;
    }
}
```

---

## 2. Public Page Template (no auth)

```razor
@page "/"
@inject ISearchService SearchService
@inject IJSRuntime JS

@* No @attribute [Authorize] *@
```

---

## 3. MudTable with Search Filter

```razor
<MudTextField @bind-Value="_searchText"
              Placeholder="Qidirish..."
              Adornment="Adornment.Start"
              AdornmentIcon="@Icons.Material.Filled.Search"
              Immediate="true"
              Class="mb-3" />

<MudTable Items="@FilteredItems" Hover Striped Dense>
    <HeaderContent>
        <MudTh>Nomi</MudTh>
        <MudTh>Manzil</MudTh>
        <MudTh>Amallar</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.Name</MudTd>
        <MudTd>@context.Address</MudTd>
        <MudTd>
            <MudIconButton Icon="@Icons.Material.Filled.Edit"
                           Size="Size.Small"
                           OnClick="@(() => EditItem(context.Id))" />
        </MudTd>
    </RowTemplate>
</MudTable>

@code {
    private string _searchText = "";
    private IEnumerable<PharmacyDto> FilteredItems =>
        _items.Where(i => string.IsNullOrEmpty(_searchText) ||
                          i.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
}
```

---

## 4. Confirm Dialog

```razor
@inject IDialogService DialogService

@code {
    private async Task DeleteAsync(int id)
    {
        var result = await DialogService.ShowMessageBox(
            "O'chirishni tasdiqlang",
            "Bu ma'lumotni o'chirishni xohlaysizmi?",
            yesText: "Ha, o'chirish",
            cancelText: "Bekor qilish");

        if (result == true)
        {
            await ExampleService.DeleteAsync(id);
            Snackbar.Add("Muvaffaqiyatli o'chirildi", Severity.Success);
            await LoadDataAsync();
        }
    }
}
```

---

## 5. Form with Validation

```razor
<EditForm Model="_request" OnValidSubmit="SaveAsync">
    <FluentValidationValidator />

    <MudTextField @bind-Value="_request.Name"
                  Label="Nomi"
                  For="@(() => _request.Name)"
                  Immediate="true" />

    <MudTextField @bind-Value="_request.Address"
                  Label="Manzil"
                  For="@(() => _request.Address)" />

    <MudButton ButtonType="ButtonType.Submit"
               Variant="Variant.Filled"
               Color="Color.Primary"
               Disabled="_saving">
        @if (_saving) { <MudProgressCircular Size="Size.Small" Indeterminate Class="mr-2" /> }
        Saqlash
    </MudButton>
</EditForm>

@code {
    private CreatePharmacyRequest _request = new("", "", 0, 0, null, null, "", "");
    private bool _saving;

    private async Task SaveAsync()
    {
        _saving = true;
        try
        {
            await PharmacyService.CreateAsync(_request);
            Snackbar.Add("Muvaffaqiyatli saqlandi", Severity.Success);
            Nav.NavigateTo("/admin/pharmacies");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Xatolik: {ex.Message}", Severity.Error);
        }
        finally { _saving = false; }
    }
}
```

---

## 6. MudAutocomplete (Shared Catalog)

```razor
<MudAutocomplete T="MedicineAutocompleteDto?"
                 Label="Dori nomini qidiring..."
                 @bind-Value="_selectedMedicine"
                 SearchFunc="@SearchMedicinesAsync"
                 ToStringFunc="@(m => m?.Name ?? "")"
                 ShowProgressIndicator="true"
                 MinCharacters="2"
                 DebounceInterval="300"
                 ResetValueOnEmptyText="true"
                 Clearable="true">
    <ItemTemplate Context="item">
        <MudText>@item.Name</MudText>
        @if (item.GenericName is not null)
        {
            <MudText Typo="Typo.caption" Class="ml-2 mud-text-secondary">@item.GenericName</MudText>
        }
        @if (item.DosageForm is not null)
        {
            <MudChip Size="Size.Small" Class="ml-2">@item.DosageForm</MudChip>
        }
    </ItemTemplate>
    <NoItemsTemplate>
        <MudText Class="px-4 py-2">Topilmadi — <b>yangi dori qo'shish</b></MudText>
    </NoItemsTemplate>
</MudAutocomplete>
```

---

## 7. MudToggleGroup (Sort Mode)

```razor
<MudToggleGroup T="SortMode"
                @bind-Value="_sortMode"
                SelectionMode="SelectionMode.SingleSelection"
                Color="Color.Primary"
                CheckMark
                Class="mb-3">
    <MudToggleItem Value="SortMode.Distance">
        <MudIcon Icon="@Icons.Material.Filled.NearMe" Size="Size.Small" Class="mr-1" />
        Yaqinlik
    </MudToggleItem>
    <MudToggleItem Value="SortMode.Price">
        <MudIcon Icon="@Icons.Material.Filled.AttachMoney" Size="Size.Small" Class="mr-1" />
        Narx
    </MudToggleItem>
</MudToggleGroup>
```

---

## 8. MudStepper (Pharmacy Create — 2 steps)

```razor
<MudStepper @ref="_stepper" Linear NonEditable>
    <ChildContent>
        <MudStep Title="Dorixona ma'lumotlari" HasError="@_step1HasError">
            @* pharmacy form fields *@
        </MudStep>
        <MudStep Title="Farmatsevt hisobi">
            @* account form fields *@
        </MudStep>
    </ChildContent>
    <ActionContent Context="stepper">
        <MudButton OnClick="@stepper.PreviousStepAsync" Disabled="@(stepper.ActiveIndex == 0)">Orqaga</MudButton>
        @if (stepper.ActiveIndex < stepper.Steps.Count - 1)
        {
            <MudButton OnClick="@stepper.NextStepAsync" Color="Color.Primary">Keyingi</MudButton>
        }
        else
        {
            <MudButton OnClick="SaveAsync" Color="Color.Primary" Variant="Variant.Filled">Saqlash</MudButton>
        }
    </ActionContent>
</MudStepper>
```

---

## 9. Google Maps IconButton

```razor
<MudIconButton Icon="@Icons.Material.Filled.Map"
               Size="Size.Small"
               Color="Color.Primary"
               Title="Xaritada ko'rish"
               Href="@pharmacy.MapsUrl"
               Target="_blank" />
```

---

## 10. Active Status Toggle (Admin pharmacy list)

```razor
<MudSwitch T="bool"
           Value="@context.IsActive"
           ValueChanged="@(async v => await ToggleActiveAsync(context.Id, v))"
           Color="Color.Success"
           Size="Size.Small" />

@code {
    private async Task ToggleActiveAsync(int id, bool isActive)
    {
        await PharmacyService.SetActiveAsync(id, isActive);
        var msg = isActive ? "Faollashtirildi" : "O'chirildi";
        Snackbar.Add(msg, Severity.Info);
    }
}
```

---

## 11. Low Stock Badge

```razor
@if (item.Quantity < 5)
{
    <MudChip Color="Color.Warning" Size="Size.Small" Icon="@Icons.Material.Filled.Warning">
        Kam qoldi
    </MudChip>
}
```

---

## 12. MudExpansionPanel for Search Results

```razor
<MudExpansionPanels MultiExpansion>
    @foreach (var result in _searchResults)
    {
        <MudExpansionPanel>
            <TitleContent>
                <MudText><b>@result.Medicine.Name</b></MudText>
                @if (result.Medicine.CategoryName is not null)
                {
                    <MudChip Size="Size.Small" Class="ml-2">@result.Medicine.CategoryName</MudChip>
                }
                <MudText Typo="Typo.caption" Class="ml-2">
                    @result.Pharmacies.Count ta dorixona
                </MudText>
            </TitleContent>
            <ChildContent>
                @if (result.Pharmacies.Count == 0)
                {
                    <MudAlert Severity="Severity.Info">Bu dori hozirda mavjud emas.</MudAlert>
                }
                else
                {
                    <MudTable Items="@result.Pharmacies" Dense>
                        @* pharmacy table *@
                    </MudTable>
                }
            </ChildContent>
        </MudExpansionPanel>
    }
</MudExpansionPanels>
```

---

## 13. Rules and Gotchas

- Always `StateHasChanged()` after async JS interop updates
- `OnAfterRenderAsync(bool firstRender)` — JS interop only works when `firstRender = true`
- Never use `@onclick` on table rows for navigation — use `NavigationManager.NavigateTo()`
- `[Parameter]` properties must be public
- Avoid `Thread.Sleep` — use `Task.Delay` inside async methods
- `MudAutocomplete` with `DebounceInterval` requires `MinCharacters` to avoid empty queries
