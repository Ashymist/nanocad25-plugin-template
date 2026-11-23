namespace NanoCadPlugin
{

    internal static class PluginInfo
    {

        internal const string IdPrefix = "MYPLUGIN";

        internal const string TabText = "Мой плагин";
        internal const string ButtonText = "Открыть окно расчёта";
        internal const string WindowTitle = "Мой плагин: расчёт";
        internal const string KeyTip = "M";

        internal const string IconDll = "mcsmenu.dll";
        internal const string IconName = "I285";

        internal const string CommandName = IdPrefix + "_RUN";
        internal const string InstallCommandName = IdPrefix + "_RIBBON";
        internal const string UninstallCommandName = IdPrefix + "_UNINSTALL";

        internal const string PanelUid = "ID_Panel" + IdPrefix;
        internal const string TabUid = "ID_Tab" + IdPrefix;
        internal const string ButtonUid = "RBN_" + IdPrefix + "_RUN";
        internal const string TabReferenceUid = "RBNU_" + IdPrefix + "_REF";
        internal const string RowUid = "RBNUR_" + IdPrefix;
        internal const string RowPanelUid = "RBNURP_" + IdPrefix;
        internal const string InnerRowUid = "RBNURR_" + IdPrefix;

        internal const string Marker = "<!--" + IdPrefix + "_RIBBON-->";

        internal const string BackupSuffix = ".before-" + IdPrefix + ".bak";

        internal const string CfgComment = "; " + IdPrefix + ": команда, иконка и подсказка для ленты.";

        internal const string ToolTipText = ButtonText;
        internal const string StatusText = TabText + " — окно расчёта";
        internal const string UserRibbonFile = "user_ribbon.cfg";
    }
}
