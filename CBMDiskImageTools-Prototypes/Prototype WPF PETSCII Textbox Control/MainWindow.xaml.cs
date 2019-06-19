using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prototype_WPF_PETSCII_Textbox_Control
{
    public partial class MainWindow : Window
    {
        characterSets currCharacterSet = characterSets.characterSet1;
        Dictionary<int, InputMappingItem> inputMapping = CreateInputMapping();
        public MainWindow()
        {
            InitializeComponent();
            rb_CharacterSet1.IsChecked = true;
            currCharacterSet = characterSets.characterSet1;

            InputMethod.SetIsInputMethodEnabled(tb_Input, false); // Das verhindert das Einfügen von "double byte chars" z.B. von Emoji's (Stichwort: UTF-32)
                                                                  // The High Surrogate(U+D800–U+DBFF) and Low Surrogate(U+DC00–U+DFFF) codes are reserved for encoding non-BMP characters in UTF - 16 by using a pair of 16 - bit codes: one High Surrogate and one Low Surrogate.
                                                                  // A single surrogate code point will never be assigned a character.
                                                                  // non-Basic Multilingual Plane (non-BMP)
                                                                  // https://en.wikipedia.org/wiki/Plane_(Unicode)
        }
        private enum characterSets
        {
            characterSet1 = 0xe0,
            characterSet2 = 0xe1
        }
        private enum copyMode
        {
            ascii7Bit,
            unicodeCharacterSetRange
        }
        private enum pasteMode
        {
            ascii7Bit,
            unicodeCharacterSetRange,
            auto
        }
        private copyMode GetCurrCopyMode()
        {
            if (rb_CopyMode_ascii7Bit.IsChecked == true)
            {
                return copyMode.ascii7Bit;
            }
            else if (rb_CopyMode_unicodeCharacterSetRange.IsChecked == true)
            {
                return copyMode.unicodeCharacterSetRange;
            }
            rb_CopyMode_ascii7Bit.IsChecked = true;
            return copyMode.ascii7Bit;
        }
        private pasteMode GetCurrPasteMode()
        {
            if (rb_PasteMode_ascii7Bit.IsChecked == true)
            {
                return pasteMode.ascii7Bit;
            }
            else if (rb_PasteMode_unicodeCharacterSetRange.IsChecked == true)
            {
                return pasteMode.unicodeCharacterSetRange;
            }
            else if (rb_PasteMode_auto.IsChecked == true)
            {
                return pasteMode.auto;
            }
            rb_PasteMode_auto.IsChecked = true;
            return pasteMode.auto;
        }
        private static Dictionary<int, InputMappingItem> CreateInputMapping()
        {
            // 87 zulässige Zeichen
            Dictionary<int, InputMappingItem> newInputMapping = new Dictionary<int, InputMappingItem>();
            addRange(32, 32, 0, ref newInputMapping);      // 1:1 1:1     WhiteSpace
            addRange(33, 47, 0, ref newInputMapping);      // 1:1 1:1     ! " # $ % & ' ( ) * + ´ - . /
            addRange(48, 57, 0, ref newInputMapping);      // 1:1 1:1     0 .. 9
            addRange(58, 64, 0, ref newInputMapping);      // 1:1 1:1     : ; < = > ? @
            addRange(65, 90, 32, ref newInputMapping);     // A=♠ A=A     A .. Z
            addRange(91, 91, 0, ref newInputMapping);      // [=[ [=[     [
            //addRange(92, 92, 0, ref newInputMapping);    // anders      \
            addRange(93, 93, 0, ref newInputMapping);      // ]=] ]=]     ]
            //addRange(94, 96, 0, ref newInputMapping);    // anders      ^ _ 
            addRange(97, 122, -32, ref newInputMapping);   // a=A a=a     a .. z
            //addRange(123, 126, 0, ref newInputMapping);  // anders      { | } ~
            return newInputMapping;
        }
        private static void addRange(int from, int to, int targetByInputOffset, ref Dictionary<int, InputMappingItem> newInputMapping)
        {
            for (int i = from; i <= to; i++)
            {
                newInputMapping.Add(i, new InputMappingItem(i, i + targetByInputOffset));
            }
        }
        private void Tb_Input_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            // ACHTUNG: SPACE löst das Event nicht aus --> das PreviewKeyDown Event muss SPACE behandeln

            if (e.TextComposition.Text.Length == 1)
            {
                char c = e.TextComposition.Text[0];
                int ic = (int)c;
                if (ic != 32) // 32 = Space wird PreviewKeyDown Event behandelt
                {
                    if (inputMapping.ContainsKey(ic))
                    {
                        byte[] ba = new byte[1];
                        ba[0] = (byte)inputMapping[ic].TargetByInput;
                        string st = GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(ba, currCharacterSet);
                        TbRaiseEvent((TextBox)sender, e.Device, st);
                    }
                }
            }
            e.Handled = true;
        }
        private void Tb_Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            popUp.IsOpen = false;
            if (e.Key == Key.Space)
            {
                byte[] ba = new byte[] { 32 };
                string st = GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(ba, currCharacterSet);
                TbRaiseEvent((TextBox)sender, e.Device, st);
                e.Handled = true;
            }
        }
        private void TbRaiseEvent(TextBox tb, InputDevice inDevice, string txtToSend)
        {
            TextComposition tc = new TextComposition(InputManager.Current, tb, txtToSend);
            TextCompositionEventArgs args = new TextCompositionEventArgs(inDevice, tc);
            args.RoutedEvent = TextCompositionManager.TextInputEvent;
            tb.RaiseEvent(args);
        }
        private void Rb_CharacterSet1_Checked(object sender, RoutedEventArgs e)
        {
            currCharacterSet = characterSets.characterSet1;
            ChangeCharacterSetInTextBox(tb_Input, currCharacterSet);
        }
        private void Rb_CharacterSet2_Checked(object sender, RoutedEventArgs e)
        {
            currCharacterSet = characterSets.characterSet2;
            ChangeCharacterSetInTextBox(tb_Input, currCharacterSet);
        }
        private void ChangeCharacterSetInTextBox(TextBox tb, characterSets toCharacterSet)
        {
            int ss = tb.SelectionStart;
            int sl = tb.SelectionLength;
            tb.Text = ChangeCharacterSet(tb.Text, toCharacterSet);
            tb.SelectionStart = ss;
            tb.SelectionLength = sl;
        }
        private string ChangeCharacterSet(string str, characterSets toCharacterSet)
        {
            byte[] PETSCII = GetPETSCIIByteArrayByUnicodeString(str, false, true, false);
            return GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(PETSCII, toCharacterSet);
        }
        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            FlowDocument flowDocument = new FlowDocument();
            bool characterSkipped = false;
            if (e.DataObject.GetDataPresent("PETSCII"))
            {
                byte[] ba = ((byte[])e.DataObject.GetData("PETSCII"));
                DataObject d = new DataObject();
                d.SetData(DataFormats.UnicodeText, GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(ba, currCharacterSet));
                e.DataObject = d;
            }
            else if (e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                string textToPaste = (string)e.DataObject.GetData(DataFormats.UnicodeText);
                string str = "";
                pasteMode pm = GetCurrPasteMode();
                if (pm == pasteMode.ascii7Bit)
                {
                    byte[] ba = GetPETSCIIByteArrayByUnicodeString(textToPaste, true, false, false, out flowDocument, out characterSkipped);
                    str = GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(ba, currCharacterSet);
                }
                if (pm == pasteMode.unicodeCharacterSetRange)
                {
                    byte[] ba = GetPETSCIIByteArrayByUnicodeString(textToPaste, false, true, false, out flowDocument, out characterSkipped);
                    str = GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(ba, currCharacterSet);
                }
                if (pm == pasteMode.auto)
                {
                    byte[] ba = GetPETSCIIByteArrayByUnicodeString(textToPaste, true, true, false, out flowDocument, out characterSkipped);
                    str = GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(ba, currCharacterSet);
                }
                DataObject d = new DataObject();
                d.SetData(DataFormats.UnicodeText, str);
                e.DataObject = d;
                // Um die MessageBox innerhalb des Eventhandlers auszurufen ist das Dispatcher.BeginInvoke notwendig
                // Dispatcher.BeginInvoke((Action)delegate ()
                // {
                //     MessageBox.Show("Cannot Paste ...");
                // }, null);
                if (characterSkipped)
                {
                    rtb_Input_Popup_Text1.Document = flowDocument;
                    popUp.StaysOpen = false;
                    popUp.IsOpen = true;
                }
            }
            else e.CancelCommand();
        }
        private void CopyingHandler(object sender, DataObjectCopyingEventArgs e)
        {
            // Das DataObject kann hier nicht neu zugewiesen werden, da es keine SET Methode gibt
            // aber die Daten des vorhandenen DataObjects können abgeändert werden
            string str = (string)e.DataObject.GetData(DataFormats.UnicodeText);
            copyMode cm = GetCurrCopyMode();
            if (cm == copyMode.ascii7Bit)
            {
                e.DataObject.SetData(DataFormats.Text, Get7bitASCIIbyUnicodeCharacterSetRange(str), true);
                e.DataObject.SetData(DataFormats.UnicodeText, Get7bitASCIIbyUnicodeCharacterSetRange(str), true);
            }
            else if (cm == copyMode.unicodeCharacterSetRange)
            {
                e.DataObject.SetData(DataFormats.UnicodeText, str, true);
            }
            e.DataObject.SetData("PETSCII", GetPETSCIIByteArrayByUnicodeString(str, false, true, false));
        }
        private byte[] GetPETSCIIByteArrayByUnicodeString(string unicodeString, bool allow7BitASCII, bool allowCharacterSetRange, bool allowCharacterSetRange7BitOnly)
        {
            FlowDocument flowDocument;
            bool characterSkipped;
            return GetPETSCIIByteArrayByUnicodeString(unicodeString, allow7BitASCII, allowCharacterSetRange, allowCharacterSetRange7BitOnly, out flowDocument, out characterSkipped);
        }
        private byte[] GetPETSCIIByteArrayByUnicodeString(string unicodeString, bool allow7BitASCII, bool allowCharacterSetRange, bool allowCharacterSetRange7BitOnly, out FlowDocument flowDocument, out bool characterSkipped)
        {
            flowDocument = new FlowDocument();
            flowDocument.Blocks.Add(new Paragraph(new Run("Es wurden nicht alle Zeichen übernommen:")));
            characterSkipped = false;
            Paragraph para = new Paragraph();
            Paragraph para2 = new Paragraph();
            Paragraph para3 = new Paragraph();
            para2.FontFamily = new FontFamily("C64 Pro Mono");
            para3.FontFamily = new FontFamily("Courier New");
            byte[] b = new byte[2];
            List<byte> bl = new List<byte>();
            foreach (char c in unicodeString)
            {
                b = Encoding.BigEndianUnicode.GetBytes(c.ToString());
                Run run;
                Run run2;
                Run run3;
                run = new Run(c.ToString());
                run2 = new Run(c.ToString());
                run3 = new Run(b[0].ToString("X2") + b[1].ToString("X2") + " ");
                if (
                    (allowCharacterSetRange && (b[0] == (byte)characterSets.characterSet1 || b[0] == (byte)characterSets.characterSet2)) ||
                    (allow7BitASCII && b[0] == 0x0 && inputMapping.ContainsKey(b[1])) ||
                    (allowCharacterSetRange7BitOnly && (b[0] == (byte)characterSets.characterSet1 || b[0] == (byte)characterSets.characterSet2) && inputMapping.ContainsKey(b[1]))
                )
                {
                    bl.Add(b[1]);
                }
                else
                {
                    run.TextDecorations = TextDecorations.Strikethrough;
                    run2.TextDecorations = TextDecorations.Strikethrough;
                    run3.TextDecorations = TextDecorations.Strikethrough;
                    characterSkipped = true;
                }
                para.Inlines.Add(run);
                para2.Inlines.Add(run2);
                para3.Inlines.Add(run3);
            }
            flowDocument.Blocks.Add(para);
            flowDocument.Blocks.Add(para2);
            flowDocument.Blocks.Add(para3);
            return bl.ToArray();
        }
        private string GetUnicodeStringCharacterSetRangeByPETSCIIByteArray(byte[] PETSCII, characterSets toCharacterSet)
        {
            StringBuilder sb = new StringBuilder();
            byte[] b = new byte[2];
            foreach (byte currb in PETSCII)
            {
                b[0] = (byte)toCharacterSet;
                b[1] = currb;
                sb.Append(Encoding.BigEndianUnicode.GetString(b));
            }
            return sb.ToString();
        }
        private string Get7bitASCIIbyUnicodeCharacterSetRange(string unicodeCharacterSetRange)
        {
            byte[] ba = GetPETSCIIByteArrayByUnicodeString(unicodeCharacterSetRange, false, false, true);
            return Encoding.ASCII.GetString(ba);
        }

        private void Tb_Input_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("HEX: ");
            byte[] ba = Encoding.BigEndianUnicode.GetBytes(((TextBox)sender).Text);
            int i = 0;
            foreach (byte b in ba)
            {
                i++;
                sb.Append(b.ToString("X2"));
                if (i == 2)
                {
                    sb.Append(" ");
                    i = 0;
                }
            }
            sb.Append(Environment.NewLine);
            sb.Append("PETSCII: ");
            ba = GetPETSCIIByteArrayByUnicodeString(((TextBox)sender).Text, false, true, false);
            foreach (byte b in ba)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }
            tb_Input_ToolTip_Text.Text = sb.ToString(); ;
        }
    }
    public class InputMappingItem
    {
        public InputMappingItem(int targetValue, int targetByInput)
        {
            TargetValue = targetValue;
            TargetByInput = targetByInput;
        }
        public int TargetValue { get; set; }
        public int TargetByInput { get; set; }
    }
}
