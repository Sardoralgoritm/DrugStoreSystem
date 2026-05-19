using DrugstoreSystem.Domain.Enums;

namespace DrugstoreSystem.Application.Helpers;

public static class DosageFormHelper
{
    private static readonly Dictionary<DosageForm, string> _uz = new()
    {
        { DosageForm.Tablet,      "Tabletkа"        },
        { DosageForm.Capsule,     "Kapsulа"         },
        { DosageForm.Syrup,       "Sharbat"         },
        { DosageForm.Injection,   "In'eksiya"       },
        { DosageForm.Cream,       "Krem"            },
        { DosageForm.Drops,       "Tomchilar"       },
        { DosageForm.Powder,      "Kukun"           },
        { DosageForm.Suppository, "Suppozitoriy"    },
        { DosageForm.Patch,       "Yopishqoq latta" },
        { DosageForm.Solution,    "Eritma"          },
    };

    public static string ToUzbek(this DosageForm form) =>
        _uz.TryGetValue(form, out var name) ? name : form.ToString();

    public static string? ToUzbek(this DosageForm? form) =>
        form is null ? null : _uz.TryGetValue(form.Value, out var name) ? name : form.ToString();
}
