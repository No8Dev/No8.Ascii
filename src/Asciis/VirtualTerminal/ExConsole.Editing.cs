namespace No8.Ascii.VirtualTerminal;

using static TerminalSeq;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming


/// <summary>
///     Terminal Constants and Control string creation
/// </summary>

public partial class ExConsole : ExConsole.IExConsoleEditing
{
    
    public interface IExConsoleEditing
    {
        void CharacterDelete(int n = 1);
        void CharacterErase(int n = 1);
        void CharacterInsert(int n = 1);

        void ColumnDelete(int n = 1);
        void ColumnInsert(int n = 1);
        
        void LineDelete(int n = 1);
        void LineInsert(int n = 1);


        void EraseCursorToEndOfDisplay();
        void EraseCursorToEndOfLine();
        
        void EraseTopOfDisplayToCursor();
        void EraseStartOfLineToCursor();

        void EraseRectangle(int top, int left, int bottom, int right);
        void FillRectangle(int top, int left, int bottom, int right, char ch);

        /// <summary>
        ///     Set of clear all character attributes in a given rectangle 
        /// </summary>
        void RectangleAttributes(int top, int left, int bottom, int right,
            params FillCharacterAttributes[] flags);
    }
        void IExConsoleEditing.CharacterDelete(int n) => Write(TermInfo.DeleteCharacter ?? EditingControlFunctions.DeleteCharacter(n));
        void IExConsoleEditing.CharacterErase(int n) => Write(TermInfo.EraseChars ?? EditingControlFunctions.EraseCharacter(n));
        void IExConsoleEditing.CharacterInsert(int n) => Write(TermInfo.InsertCharacter ?? EditingControlFunctions.InsertCharacter(n));
        
        
        void IExConsoleEditing.ColumnDelete(int n) => Write(EditingControlFunctions.DeleteColumn(n));
        void IExConsoleEditing.ColumnInsert(int n) => Write(EditingControlFunctions.InsertColumn(n));
        
        void IExConsoleEditing.LineDelete(int n) => Write(TermInfo.DeleteLine ?? EditingControlFunctions.DeleteLine(n));
        void IExConsoleEditing.LineInsert(int n) => Write(TermInfo.InsertLine ?? EditingControlFunctions.InsertLine(n));


        void IExConsoleEditing.EraseCursorToEndOfDisplay() => Write(TermInfo.ClrEos ?? EditingControlFunctions.EraseInDisplay(0));
        void IExConsoleEditing.EraseCursorToEndOfLine() => Write(TermInfo.ClrEol ?? EditingControlFunctions.EraseInLine(0));
        
        void IExConsoleEditing.EraseTopOfDisplayToCursor() => Write(EditingControlFunctions.EraseInDisplay(1));
        void IExConsoleEditing.EraseStartOfLineToCursor() => Write(TermInfo.ClrBol ?? EditingControlFunctions.EraseInLine(1));

        void IExConsoleEditing.EraseRectangle(int top, int left, int bottom, int right) =>
            Write(RectangleAreaProcessing.EraseRectangularArea(top, left, bottom, right));
        void IExConsoleEditing.FillRectangle(int top, int left, int bottom, int right, char ch) =>
            Write(RectangleAreaProcessing.FillRectangleArea(ch, top, left, bottom, right));

        /// <summary>
        ///     Set of clear all character attributes in a given rectangle 
        /// </summary>
        void IExConsoleEditing.RectangleAttributes(int top, int left, int bottom, int right, params FillCharacterAttributes[] flags)
        {
            if (!flags.Any())
            {
                Write(TerminalSeq.RectangleAreaProcessing.ChangeAttributeInRectangle(
                    top, left, bottom, right,
                    (int)FillCharacterAttributes.Off));
                return;
            }

            Write(RectangleAreaProcessing.ChangeAttributeInRectangle(
                top, left, bottom, right, 
                flags.OfType<int>().ToArray()));
        }
}

