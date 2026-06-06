using System.Diagnostics;
using BaseLib.Config;
using Godot;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;

namespace AyaMod.Huoreimu.Config;

[ConfigHoverTipsByDefault]
public sealed class HuoreimuConfig : SimpleModConfig
{
    private static bool _uploadErrorLogs;
    private static bool _consentDialogOpen;

    public static bool UploadErrorLogs
    {
        get => _uploadErrorLogs;
        set
        {
            if (_consentDialogOpen) return;
            if (value == _uploadErrorLogs) return;
            if (!value)
            {
                _uploadErrorLogs = false;
                return;
            }
            if (NModalContainer.Instance == null)
            {
                _uploadErrorLogs = true;
                return;
            }
            _consentDialogOpen = true;
            _ = ShowConsentDialog();
        }
    }

    public override void SetupConfigUI(Control optionContainer)
    {
        var uploadProperty = GetType().GetProperty(nameof(UploadErrorLogs));
        Debug.Assert(uploadProperty != null, nameof(uploadProperty) + " != null");
        var row = CreateToggleOption(uploadProperty, addHoverTip: true);
        row.UniqueNameInOwner = true;
        optionContainer.AddChild(row, forceReadableName: false);

        AddRestoreDefaultsButton(optionContainer);
        SetupFocusNeighbors(optionContainer);
    }

    private static async Task ShowConsentDialog()
    {
        try
        {
            var thisConfig = ModConfigRegistry.Get<HuoreimuConfig>();
            if (thisConfig == null) { _consentDialogOpen = false; return; }

            var popup = NGenericPopup.Create();
            if (popup == null || NModalContainer.Instance == null)
            {
                _uploadErrorLogs = true;
                thisConfig.Changed();
                _consentDialogOpen = false;
                return;
            }

            NModalContainer.Instance.Add(popup);
            var result = await popup.WaitForConfirmation(
                new LocString("settings_ui", "AYAMOD-CONSENT_UPLOAD_LOGS.body"),
                new LocString("settings_ui", "AYAMOD-CONSENT_UPLOAD_LOGS.header"),
                new LocString("main_menu_ui", "GENERIC_POPUP.cancel"),
                new LocString("main_menu_ui", "GENERIC_POPUP.confirm"));

            if (result)
            {
                _uploadErrorLogs = true;
                thisConfig.Changed();
            }
            else
            {
                thisConfig.ConfigReloaded();
            }
        }
        finally
        {
            _consentDialogOpen = false;
        }
    }
}
