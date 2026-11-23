using System;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.Runtime;

[assembly: ExtensionApplication(typeof(NanoCadPlugin.PluginApplication))]

namespace NanoCadPlugin
{

    public class Commands
    {

        [CommandMethod(PluginInfo.CommandName)]
        public void Run()
        {
            FormLauncher.ShowWindow();
        }

        [CommandMethod(PluginInfo.InstallCommandName)]
        public void InstallRibbonCommand()
        {
            bool ok = InstallRibbon(false);
            Editor ed = GetEditor();
            if (ed == null)
                return;

            ed.WriteMessage(ok
                ? "\n" + PluginInfo.TabText + ": вкладка установлена. Если иконка/подписи не обновились — перезапустите nanoCAD."
                : "\n" + PluginInfo.TabText + ": не удалось найти папку config профиля nanoCAD.");
        }

        [CommandMethod(PluginInfo.UninstallCommandName)]
        public void UninstallRibbonCommand()
        {
            bool ok = UninstallRibbon(false);
            Editor ed = GetEditor();
            if (ed == null)
                return;

            ed.WriteMessage(ok
                ? "\n" + PluginInfo.TabText + ": записи плагина удалены из профиля."
                : "\n" + PluginInfo.TabText + ": не удалось найти папку config профиля nanoCAD.");
        }

        internal static bool InstallRibbon(bool silent)
        {
            try
            {
                string configDir = RibbonInstaller.FindConfigDir();
                if (configDir == null)
                    return false;

                bool changed = RibbonInstaller.EnsureInstalled(configDir);

                Editor ed = GetEditor();
                if (!silent && ed != null && changed)
                    ed.WriteMessage("\n" + PluginInfo.TabText + ": обновляю ленту...");

                Document doc = Application.DocumentManager.MdiActiveDocument;
                if (doc != null)
                {

                    doc.SendStringToExecute("RELOADRIBBON\n", true, false, false);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                Editor ed = GetEditor();
                if (ed != null)
                    ed.WriteMessage("\n" + PluginInfo.TabText + ": ошибка установки ленты: " + ex.Message);
                return false;
            }
        }

        internal static bool UninstallRibbon(bool silent)
        {
            try
            {
                string configDir = RibbonInstaller.FindConfigDir();
                if (configDir == null)
                    return false;

                bool changed = RibbonInstaller.Uninstall(configDir);

                Editor ed = GetEditor();
                if (!silent && ed != null && changed)
                    ed.WriteMessage("\n" + PluginInfo.TabText + ": удаляю записи из профиля...");

                Document doc = Application.DocumentManager.MdiActiveDocument;
                if (doc != null)
                    doc.SendStringToExecute("RELOADRIBBON\n", true, false, false);

                return true;
            }
            catch (System.Exception ex)
            {
                Editor ed = GetEditor();
                if (ed != null)
                    ed.WriteMessage("\n" + PluginInfo.TabText + ": ошибка удаления: " + ex.Message);
                return false;
            }
        }

        private static Editor GetEditor()
        {
            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                return doc == null ? null : doc.Editor;
            }
            catch
            {
                return null;
            }
        }
    }

    public class PluginApplication : IExtensionApplication
    {
        public void Initialize()
        {

            Commands.InstallRibbon(true);
        }

        public void Terminate()
        {
        }
    }
}
