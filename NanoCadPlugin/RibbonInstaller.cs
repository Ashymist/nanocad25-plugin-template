using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NanoCadPlugin
{

    internal static class RibbonInstaller
    {

        internal static string FindConfigDir()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string nanosoft = Path.Combine(appData, "Nanosoft");
            if (!Directory.Exists(nanosoft))
                return null;

            var candidates = Directory.GetDirectories(nanosoft)
                .Select(d => Path.Combine(d, "config"))
                .Where(c => File.Exists(Path.Combine(c, "RibbonTabsAndPanels.xml"))
                         && File.Exists(Path.Combine(c, "RibbonWS.xml")))
                .ToList();

            if (candidates.Count == 0)
                return null;

            string preferred = null;
            try
            {
                var fvi = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
                if (fvi.ProductMajorPart > 0)
                    preferred = "nanoCAD x64 " + fvi.ProductMajorPart + "." + fvi.ProductMinorPart;
            }
            catch
            {

            }

            if (!string.IsNullOrEmpty(preferred))
            {
                string exact = candidates.FirstOrDefault(c =>
                    string.Equals(Path.GetFileName(Path.GetDirectoryName(c)), preferred,
                                  StringComparison.OrdinalIgnoreCase));
                if (exact != null)
                    return exact;
            }

            return candidates.OrderByDescending(Directory.GetLastWriteTime).First();
        }

        internal static bool EnsureInstalled(string configDir)
        {
            bool changed = false;
            changed |= PatchContent(Path.Combine(configDir, "RibbonTabsAndPanels.xml"));
            changed |= PatchWorkspace(Path.Combine(configDir, "RibbonWS.xml"));
            changed |= PatchCommands(Path.Combine(configDir, "RibbonCmds.xml"));
            changed |= EnsureUserRibbonCommand(configDir);
            return changed;
        }

        internal static bool Uninstall(string configDir)
        {
            bool changed = false;
            changed |= RemoveContent(Path.Combine(configDir, "RibbonTabsAndPanels.xml"));
            changed |= RemoveWorkspace(Path.Combine(configDir, "RibbonWS.xml"));
            changed |= RemoveCommands(Path.Combine(configDir, "RibbonCmds.xml"));
            changed |= RemoveUserRibbonCommand(configDir);
            return changed;
        }

        private static bool PatchContent(string path)
        {
            if (!File.Exists(path))
                return false;

            bool hasBom;
            string text = ReadText(path, out hasBom);
            string q = "\"";
            string panelUid = Regex.Escape(PluginInfo.PanelUid);
            string tabUid = Regex.Escape(PluginInfo.TabUid);

            int panelCount = Regex.Matches(text, Regex.Escape("UID=" + q + PluginInfo.PanelUid + q)).Count;
            int tabCount = Regex.Matches(text, Regex.Escape("UID=" + q + PluginInfo.TabUid + q)).Count;
            bool upToDate = panelCount == 1 && tabCount == 1
                            && text.IndexOf(PluginInfo.Marker, StringComparison.Ordinal) >= 0;
            if (upToDate)
                return false;

            string original = text;
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";

            text = Regex.Replace(text,
                @"\s*(?:" + Regex.Escape(PluginInfo.Marker) + @"\s*)?<RibbonPanelSource UID=" + q + panelUid + q + @".*?</RibbonPanelSource>",
                string.Empty, RegexOptions.Singleline);
            text = Regex.Replace(text,
                @"\s*<RibbonTabSource[^>]*UID=" + q + tabUid + q + @".*?</RibbonTabSource>",
                string.Empty, RegexOptions.Singleline);
            text = Regex.Replace(text,
                @"\s*<RibbonPanelSourceReference[^>]*PanelId=" + q + panelUid + q + @"[^>]*/>",
                string.Empty, RegexOptions.Singleline);
            text = text.Replace(PluginInfo.Marker, string.Empty);

            int panelsEnd = text.LastIndexOf("</RibbonPanelSourceCollection>", StringComparison.Ordinal);
            int tabsEnd = text.LastIndexOf("</RibbonTabSourceCollection>", StringComparison.Ordinal);
            if (panelsEnd < 0 || tabsEnd < 0)
                return false;

            text = text.Insert(panelsEnd, BuildPanelXml(nl));
            tabsEnd = text.LastIndexOf("</RibbonTabSourceCollection>", StringComparison.Ordinal);
            text = text.Insert(tabsEnd, BuildTabXml(nl));

            return WriteIfChanged(path, original, text, hasBom);
        }

        private static bool RemoveContent(string path)
        {
            if (!File.Exists(path))
                return false;

            bool hasBom;
            string text = ReadText(path, out hasBom);
            string original = text;
            string q = "\"";

            text = Regex.Replace(text,
                @"\s*(?:" + Regex.Escape(PluginInfo.Marker) + @"\s*)?<RibbonPanelSource UID=" + q + Regex.Escape(PluginInfo.PanelUid) + q + @".*?</RibbonPanelSource>",
                string.Empty, RegexOptions.Singleline);
            text = Regex.Replace(text,
                @"\s*<RibbonTabSource[^>]*UID=" + q + Regex.Escape(PluginInfo.TabUid) + q + @".*?</RibbonTabSource>",
                string.Empty, RegexOptions.Singleline);
            text = Regex.Replace(text,
                @"\s*<RibbonPanelSourceReference[^>]*PanelId=" + q + Regex.Escape(PluginInfo.PanelUid) + q + @"[^>]*/>",
                string.Empty, RegexOptions.Singleline);
            text = text.Replace(PluginInfo.Marker, string.Empty);

            return WriteIfChanged(path, original, text, hasBom);
        }

        private static string BuildPanelXml(string nl)
        {
            return
                PluginInfo.Marker + nl +
                "  <RibbonPanelSource UID=\"" + PluginInfo.PanelUid + "\" Text=\"" + EscapeXml(PluginInfo.TabText) + "\" HiddenInEditor=\"false\">" + nl +
                "    <RibbonRow UID=\"" + PluginInfo.RowUid + "\">" + nl +
                "      <RibbonRowPanel UID=\"" + PluginInfo.RowPanelUid + "\" ResizeStyle=\"None\" ResizePriority=\"100\" TopJustify=\"True\">" + nl +
                "        <RibbonRow UID=\"" + PluginInfo.InnerRowUid + "\">" + nl +
                "          <RibbonCommandButton UID=\"" + PluginInfo.ButtonUid + "\" Id=\"RibbonCommandButton\" Text=\"" + EscapeXml(PluginInfo.ButtonText) + "\" ButtonStyle=\"LargeWithText\" MenuMacroID=\"" + EscapeXml(PluginInfo.CommandName) + "\" KeyTip=\"" + EscapeXml(PluginInfo.KeyTip) + "\"/>" + nl +
                "        </RibbonRow>" + nl +
                "      </RibbonRowPanel>" + nl +
                "    </RibbonRow>" + nl +
                "  </RibbonPanelSource>" + nl;
        }

        private static string BuildTabXml(string nl)
        {
            return
                "  <RibbonTabSource Text=\"" + EscapeXml(PluginInfo.TabText) + "\" UID=\"" + PluginInfo.TabUid + "\">" + nl +
                "    <RibbonPanelSourceReference UID=\"" + PluginInfo.TabReferenceUid + "\" PanelId=\"" + PluginInfo.PanelUid + "\" ResizeStyle=\"Default\"/>" + nl +
                "  </RibbonTabSource>" + nl;
        }

        private static bool PatchWorkspace(string path)
        {
            if (!File.Exists(path))
                return false;

            bool hasBom;
            string text = ReadText(path, out hasBom);
            string original = text;
            string q = "\"";

            text = Regex.Replace(text,
                @"\s*<WSRibbonTabSourceReference[^>]*TabId=" + q + Regex.Escape(PluginInfo.TabUid) + q + @".*?</WSRibbonTabSourceReference>",
                string.Empty, RegexOptions.Singleline);

            if (text.IndexOf("TabId=" + q + PluginInfo.TabUid + q, StringComparison.Ordinal) < 0)
            {
                string nl = text.Contains("\r\n") ? "\r\n" : "\n";
                string reference =
                    "      <WSRibbonTabSourceReference TabId=\"" + PluginInfo.TabUid + "\" Show=\"true\" IsActive=\"false\">" + nl +
                    "      </WSRibbonTabSourceReference>" + nl + "    ";

                int rootEnd = text.LastIndexOf("</WSRibbonRoot>", StringComparison.Ordinal);
                if (rootEnd < 0)
                    return false;
                text = text.Insert(rootEnd, reference);
            }

            return WriteIfChanged(path, original, text, hasBom);
        }

        private static bool RemoveWorkspace(string path)
        {
            if (!File.Exists(path))
                return false;

            bool hasBom;
            string text = ReadText(path, out hasBom);
            string original = text;
            string q = "\"";

            text = Regex.Replace(text,
                @"\s*<WSRibbonTabSourceReference[^>]*TabId=" + q + Regex.Escape(PluginInfo.TabUid) + q + @".*?</WSRibbonTabSourceReference>",
                string.Empty, RegexOptions.Singleline);

            return WriteIfChanged(path, original, text, hasBom);
        }

        private static bool PatchCommands(string path)
        {
            if (!File.Exists(path))
                return false;

            bool hasBom;
            string text = ReadText(path, out hasBom);
            string original = text;

            if (text.IndexOf("UID=\"" + PluginInfo.CommandName + "\"", StringComparison.Ordinal) >= 0)
                return false;

            int end = text.LastIndexOf("</Ribboncmds>", StringComparison.OrdinalIgnoreCase);
            if (end < 0)
                return false;

            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            text = text.Insert(end, BuildCommandEntry(nl));

            return WriteIfChanged(path, original, text, hasBom);
        }

        private static bool RemoveCommands(string path)
        {
            if (!File.Exists(path))
                return false;

            bool hasBom;
            string text = ReadText(path, out hasBom);
            string original = text;

            text = Regex.Replace(text,
                @"\s*<Ribboncmd[^>]*UID=" + "\"" + Regex.Escape(PluginInfo.CommandName) + "\"" + @".*?</Ribboncmd>",
                string.Empty, RegexOptions.Singleline);

            return WriteIfChanged(path, original, text, hasBom);
        }

        private static string BuildCommandEntry(string nl)
        {
            return
                "  <Ribboncmd UID=\"" + EscapeXml(PluginInfo.CommandName) + "\" resolved=\"1\">" + nl +
                "    <Command>" + EscapeXml(PluginInfo.CommandName) + "</Command>" + nl +
                "  </Ribboncmd>" + nl;
        }

        private static bool EnsureUserRibbonCommand(string configDir)
        {
            string path = Path.Combine(configDir, PluginInfo.UserRibbonFile);
            string section = BuildCommandSection();
            string text = File.Exists(path) ? File.ReadAllText(path, Encoding.UTF8) : string.Empty;

            string cleaned = RemoveOwnCfgSection(text);
            string updated = cleaned.Length > 0 ? cleaned + "\r\n\r\n" + section : section;

            if (string.Equals(updated, text, StringComparison.Ordinal))
                return false;

            CopyFile(path, path + PluginInfo.BackupSuffix);
            File.WriteAllText(path, updated, new UTF8Encoding(true));
            return true;
        }

        private static bool RemoveUserRibbonCommand(string configDir)
        {
            string path = Path.Combine(configDir, PluginInfo.UserRibbonFile);
            if (!File.Exists(path))
                return false;

            string text = File.ReadAllText(path, Encoding.UTF8);
            string cleaned = RemoveOwnCfgSection(text);
            string updated = cleaned.Length > 0 ? cleaned + "\r\n" : string.Empty;

            if (string.Equals(updated, text, StringComparison.Ordinal))
                return false;

            CopyFile(path, path + PluginInfo.BackupSuffix);
            File.WriteAllText(path, updated, new UTF8Encoding(true));
            return true;
        }

        private static string RemoveOwnCfgSection(string text)
        {

            string pattern = @"(?ms)^\[\\configman\\commands\\" + Regex.Escape(PluginInfo.CommandName) + @"\]\s*\r?\n.*?(?=^\[|\z)";
            string cleaned = Regex.Replace(text, pattern, string.Empty);

            cleaned = Regex.Replace(cleaned,
                @"(?m)^;[^\r\n]*" + Regex.Escape(PluginInfo.IdPrefix) + @"[^\r\n]*\r?\n",
                string.Empty);

            return cleaned.TrimEnd();
        }

        private static string BuildCommandSection()
        {
            string nl = "\r\n";
            string cmd = PluginInfo.CommandName;

            return
                PluginInfo.CfgComment + nl +
                "[" + @"\configman\commands" + @"\" + cmd + "]" + nl +
                "weight=i30 |CmdType=i1" + nl +
                "intername=s" + cmd + nl +
                "BitmapDll=s" + SanitizeCfg(PluginInfo.IconDll) + " |Icon=s" + SanitizeCfg(PluginInfo.IconName) + nl +
                "DispName=s" + SanitizeCfg(PluginInfo.ButtonText) + nl +
                "ToolTipText=s" + SanitizeCfg(PluginInfo.ToolTipText) + nl +
                "StatusText=s" + SanitizeCfg(PluginInfo.StatusText) + nl;
        }

        private static string EscapeXml(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        private static string SanitizeCfg(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("\r", " ").Replace("\n", " ");
        }

        private static bool WriteIfChanged(string path, string original, string text, bool hasBom)
        {
            if (string.Equals(text, original, StringComparison.Ordinal))
                return false;

            CopyFile(path, path + PluginInfo.BackupSuffix);
            WriteText(path, text, hasBom);
            return true;
        }

        private static void CopyFile(string from, string to)
        {
            try
            {
                if (!File.Exists(to) && File.Exists(from))
                    File.Copy(from, to);
            }
            catch
            {

            }
        }

        private static string ReadText(string path, out bool hasBom)
        {
            byte[] bytes = File.ReadAllBytes(path);
            hasBom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            return hasBom
                ? Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3)
                : Encoding.UTF8.GetString(bytes);
        }

        private static void WriteText(string path, string text, bool withBom)
        {
            File.WriteAllText(path, text, new UTF8Encoding(withBom));
        }
    }
}
